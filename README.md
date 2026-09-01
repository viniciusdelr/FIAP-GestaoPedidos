# API de Gestão de Pedidos


Projeto desenvolvido para a atividade de pós-graduação em Arquitetura de Sistemas .NET. O objetivo é demonstrar, na prática, qualidade de software em um projeto ASP.NET Core (.NET 8): domínio rico modelado com **DDD tático**, organizado em **Clean Architecture**, com **testes automatizados** (cenários de sucesso e de erro) e uma **pipeline de CI** no GitHub Actions que executa os testes e publica a cobertura de código. O foco do trabalho está na arquitetura e no design do domínio, não em um simples CRUD.

## Objetivo da atividade

Construir uma API de gestão de pedidos que:

- Modele as regras de negócio dentro do domínio (agregado rico, value objects, máquina de estados), e não em controllers ou serviços anêmicos;
- Respeite rigorosamente as regras de dependência da Clean Architecture entre as camadas;
- Tenha cobertura de testes unitários para o domínio e para os casos de uso da aplicação;
- Possua uma pipeline de CI que rode os testes automaticamente e disponibilize um relatório de cobertura a cada push/PR.

## Arquitetura

```
PedidosApi.sln
├── src/
│   ├── Pedidos.Domain/          → entidades, VOs, regras de negócio, interfaces de repositório
│   ├── Pedidos.Application/     → casos de uso (handlers), DTOs, exceções de aplicação
│   ├── Pedidos.Infrastructure/  → EF Core (InMemory), implementação do repositório
│   └── Pedidos.Api/             → controllers, middleware, composição/DI (Program.cs)
└── tests/
    ├── Pedidos.Domain.Tests/        → testes puros do domínio (sem mocks)
    └── Pedidos.Application.Tests/   → testes dos handlers com repositório mockado (Moq)
```

### Regras de dependência

```
Domain  <───  Application  <───  Infrastructure
   ▲                                   │
   └───────────────  Api  ─────────────┘
                     (Application + Infrastructure)
```

- **Domain** não referencia nenhum outro projeto nem pacotes externos — é puro C#.
- **Application** referencia apenas **Domain**. Define os casos de uso como handlers injetados diretamente (sem MediatR), recebendo e retornando DTOs — as entidades de domínio nunca vazam para fora dessa camada.
- **Infrastructure** referencia **Domain** e **Application**. Implementa a persistência (EF Core InMemory) e a interface `IPedidoRepository` definida no Domain (Dependency Inversion: quem define o contrato é o Domain, quem implementa é a Infrastructure).
- **Api** referencia **Application** e **Infrastructure** (esta última apenas para registrar a injeção de dependência em `Program.cs`). Os controllers são finos: recebem o DTO, chamam o handler correspondente e devolvem o resultado — nenhuma regra de negócio mora na Api.

## Decisões de DDD

### Agregado `Pedido`

`Pedido` é o *aggregate root* que protege todos os seus invariantes. Não existem setters públicos: toda mutação passa por um método de negócio explícito (`AdicionarItem`, `RemoverItem`, `AplicarDesconto`, `Fechar`, `Enviar`, `Entregar`, `Cancelar`). A coleção de itens (`ItemPedido`) é privada e só é exposta como `IReadOnlyCollection<ItemPedido>`, garantindo que ninguém fora do agregado consiga adicionar ou remover itens sem passar pelas regras do `Pedido`.

### Value Objects

- **`Cpf`** — valida os dígitos verificadores com o algoritmo oficial (não apenas o formato) e armazena somente os números.
- **`Email`** — valida o formato do endereço.
- **`Dinheiro`** — encapsula um valor decimal não negativo e concentra as operações de soma e aplicação de desconto percentual, evitando que cálculos monetários fiquem espalhados pelo código.

Todos os VOs são imutáveis (`record`), validam suas invariantes no construtor (lançando `DomainException` quando inválidos) e têm igualdade por valor.

### Máquina de estados

```
Rascunho ──Fechar()──▶ Fechado ──Enviar()──▶ Enviado ──Entregar()──▶ Entregue
   │                       │
   └──Cancelar()──▶ Cancelado ◀──Cancelar()──┘
```

- Só é possível adicionar/remover item ou aplicar desconto em um pedido **Rascunho**.
- `Fechar()` exige pelo menos um item.
- `Cancelar()` só é permitido em **Rascunho** ou **Fechado** — um pedido **Enviado** ou **Entregue** não pode mais ser cancelado.
- Qualquer transição fora dessas regras lança `DomainException` com uma mensagem clara em português.

### Exceções

- `DomainException` (Domain) representa violação de uma regra de negócio e é traduzida pela Api em **400 Bad Request** (ProblemDetails).
- `NotFoundException` (Application) é lançada pelos handlers quando um pedido não é encontrado e é traduzida em **404 Not Found**.
- Qualquer outra exceção não tratada vira **500 Internal Server Error**.

Essa tradução acontece em um middleware global (`ExceptionHandlingMiddleware`), mantendo os controllers livres de blocos try/catch.

## Como rodar a API localmente

Pré-requisito: .NET 8 SDK.

```bash
dotnet restore
dotnet run --project src/Pedidos.Api
```

A API sobe com Swagger habilitado em `/swagger`, com descrição dos endpoints. Os dados são persistidos em um banco EF Core **InMemory** (não requer nenhuma infraestrutura externa).

### Endpoints

| Método | Rota                          | Descrição                              |
|--------|-------------------------------|-----------------------------------------|
| POST   | `/api/pedidos`                | Cria um pedido (status Rascunho)        |
| POST   | `/api/pedidos/{id}/itens`     | Adiciona um item ao pedido              |
| POST   | `/api/pedidos/{id}/desconto`  | Aplica um percentual de desconto        |
| POST   | `/api/pedidos/{id}/fechar`    | Fecha o pedido (Rascunho → Fechado)     |
| POST   | `/api/pedidos/{id}/cancelar`  | Cancela o pedido                        |
| GET    | `/api/pedidos/{id}`           | Obtém um pedido pelo id                 |
| GET    | `/api/pedidos`                | Lista todos os pedidos                  |

## Como rodar os testes localmente

```bash
dotnet test
```

O projeto conta com **57 testes** (xUnit + FluentAssertions + Moq), todos verdes:

- **`Pedidos.Domain.Tests`** — testes puros de entidades e value objects (sem mocks), cobrindo cenários de sucesso e de erro: criação de pedido, adição/remoção de item, cálculo de total com e sem desconto, limites do desconto (0–30%), máquina de estados completa, validação de CPF/Email/Dinheiro e igualdade de VOs.
- **`Pedidos.Application.Tests`** — testes dos handlers com `IPedidoRepository` mockado via Moq, cobrindo o fluxo feliz (persistência via `AdicionarAsync`/`AtualizarAsync`) e os erros propagados (`DomainException` e `NotFoundException`).

Para gerar o relatório de cobertura localmente (o mesmo processo usado na pipeline):

```bash
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
dotnet tool install -g dotnet-reportgenerator-globaltool
reportgenerator -reports:"./TestResults/**/coverage.cobertura.xml" -targetdir:"./coverage-report" -reporttypes:Html
```

O relatório HTML fica disponível em `coverage-report/index.html`.

## Pipeline de CI (GitHub Actions)

O workflow `.github/workflows/ci.yml` roda em todo `push` e `pull_request` para a branch `main`:

1. Checkout do código e setup do .NET 8.
2. `dotnet restore` e `dotnet build -c Release`.
3. `dotnet test -c Release --collect:"XPlat Code Coverage"` — o Coverlet (`coverlet.collector`, referenciado nos dois projetos de teste) gera um `coverage.cobertura.xml` por projeto de teste. **A pipeline falha se qualquer teste falhar.**
4. O `dotnet-reportgenerator-globaltool` consolida os XMLs de cobertura em um relatório HTML e em um resumo Markdown.
5. O relatório HTML é publicado como artifact do workflow (`coverage-report`), disponível para download na página da execução no GitHub Actions.
6. O resumo de cobertura (percentual de linhas e branches, por assembly e por classe) é escrito no `$GITHUB_STEP_SUMMARY`, ficando visível diretamente na aba **Summary** da execução do workflow, sem precisar baixar nada.
