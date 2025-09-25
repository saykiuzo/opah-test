namespace Consolidado.Domain.Services;

public interface IRelatorioService
{
    Task<byte[]> GerarRelatorioExcelAsync(DateTime dataInicio, DateTime dataFim);
    Task<byte[]> GerarRelatorioCsvAsync(DateTime dataInicio, DateTime dataFim);
}