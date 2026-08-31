using Microsoft.AspNetCore.Mvc;
using Pedidos.Application.DTOs;
using Pedidos.Application.Handlers;

namespace Pedidos.Api.Controllers;

[ApiController]
[Route("api/pedidos")]
public class PedidosController : ControllerBase
{
    /// <summary>Cria um novo pedido em status Rascunho.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PedidoDto>> Criar(
        [FromBody] CriarPedidoDto dto,
        [FromServices] CriarPedidoHandler handler)
    {
        var pedido = await handler.Handle(dto);
        return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
    }

    /// <summary>Adiciona um item a um pedido em Rascunho.</summary>
    [HttpPost("{id:guid}/itens")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoDto>> AdicionarItem(
        Guid id,
        [FromBody] AdicionarItemDto dto,
        [FromServices] AdicionarItemHandler handler)
    {
        var pedido = await handler.Handle(id, dto);
        return Ok(pedido);
    }

    /// <summary>Aplica um percentual de desconto a um pedido em Rascunho.</summary>
    [HttpPost("{id:guid}/desconto")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoDto>> AplicarDesconto(
        Guid id,
        [FromBody] AplicarDescontoDto dto,
        [FromServices] AplicarDescontoHandler handler)
    {
        var pedido = await handler.Handle(id, dto);
        return Ok(pedido);
    }

    /// <summary>Fecha um pedido em Rascunho, tornando-o pronto para envio.</summary>
    [HttpPost("{id:guid}/fechar")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoDto>> Fechar(
        Guid id,
        [FromServices] FecharPedidoHandler handler)
    {
        var pedido = await handler.Handle(id);
        return Ok(pedido);
    }

    /// <summary>Cancela um pedido em Rascunho ou Fechado.</summary>
    [HttpPost("{id:guid}/cancelar")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoDto>> Cancelar(
        Guid id,
        [FromServices] CancelarPedidoHandler handler)
    {
        var pedido = await handler.Handle(id);
        return Ok(pedido);
    }

    /// <summary>Obtém um pedido pelo seu identificador.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<PedidoDto>> ObterPorId(
        Guid id,
        [FromServices] ObterPedidoHandler handler)
    {
        var pedido = await handler.Handle(id);
        return Ok(pedido);
    }

    /// <summary>Lista todos os pedidos cadastrados.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyCollection<PedidoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<PedidoDto>>> Listar(
        [FromServices] ListarPedidosHandler handler)
    {
        var pedidos = await handler.Handle();
        return Ok(pedidos);
    }
}
