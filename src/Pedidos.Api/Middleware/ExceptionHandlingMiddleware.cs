using Microsoft.AspNetCore.Mvc;
using Pedidos.Application.Exceptions;
using Pedidos.Domain.Exceptions;

namespace Pedidos.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await EscreverProblemDetailsAsync(context, StatusCodes.Status400BadRequest, "Regra de negócio violada", ex.Message);
        }
        catch (NotFoundException ex)
        {
            await EscreverProblemDetailsAsync(context, StatusCodes.Status404NotFound, "Recurso não encontrado", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao processar a requisição.");
            await EscreverProblemDetailsAsync(context, StatusCodes.Status500InternalServerError, "Erro interno", "Ocorreu um erro inesperado ao processar a requisição.");
        }
    }

    private static async Task EscreverProblemDetailsAsync(HttpContext context, int statusCode, string titulo, string detalhe)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = titulo,
            Detail = detalhe,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
