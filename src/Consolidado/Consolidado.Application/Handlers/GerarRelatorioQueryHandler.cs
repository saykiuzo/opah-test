using MediatR;
using Consolidado.Application.Queries;
using Consolidado.Domain.Services;

namespace Consolidado.Application.Handlers;

public class GerarRelatorioQueryHandler : IRequestHandler<GerarRelatorioQuery, byte[]>
{
    private readonly IRelatorioService _relatorioService;

    public GerarRelatorioQueryHandler(IRelatorioService relatorioService)
    {
        _relatorioService = relatorioService;
    }

    public async Task<byte[]> Handle(GerarRelatorioQuery request, CancellationToken cancellationToken)
    {
        return request.Formato.ToLower() switch
        {
            "csv" => await _relatorioService.GerarRelatorioCsvAsync(request.DataInicio, request.DataFim),
            "xlsx" => await _relatorioService.GerarRelatorioExcelAsync(request.DataInicio, request.DataFim),
            _ => throw new ArgumentException("Formato inválido. Use 'xlsx' ou 'csv'", nameof(request.Formato))
        };
    }
}