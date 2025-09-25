using System.Globalization;
using System.Text;
using CsvHelper;
using OfficeOpenXml;
using Consolidado.Domain.Repositories;
using Consolidado.Domain.Services;

namespace Consolidado.Infrastructure.Services;

public class RelatorioService : IRelatorioService
{
    private readonly IConsolidadoDiarioRepository _repository;

    public RelatorioService(IConsolidadoDiarioRepository repository)
    {
        _repository = repository;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public async Task<byte[]> GerarRelatorioExcelAsync(DateTime dataInicio, DateTime dataFim)
    {
        var consolidados = await _repository.ObterPorPeriodoAsync(dataInicio, dataFim);

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Consolidado Diário");

        worksheet.Cells[1, 1].Value = "Data";
        worksheet.Cells[1, 2].Value = "Saldo Inicial";
        worksheet.Cells[1, 3].Value = "Total Débitos";
        worksheet.Cells[1, 4].Value = "Total Créditos";
        worksheet.Cells[1, 5].Value = "Saldo Final";
        worksheet.Cells[1, 6].Value = "Quantidade Transações";
        worksheet.Cells[1, 7].Value = "Última Atualização";

        using (var range = worksheet.Cells[1, 1, 1, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightBlue);
        }

        var linha = 2;
        foreach (var consolidado in consolidados)
        {
            worksheet.Cells[linha, 1].Value = consolidado.Data.ToString("dd/MM/yyyy");
            worksheet.Cells[linha, 2].Value = consolidado.SaldoInicial;
            worksheet.Cells[linha, 3].Value = consolidado.TotalDebitos;
            worksheet.Cells[linha, 4].Value = consolidado.TotalCreditos;
            worksheet.Cells[linha, 5].Value = consolidado.SaldoFinal;
            worksheet.Cells[linha, 6].Value = consolidado.QuantidadeTransacoes;
            worksheet.Cells[linha, 7].Value = consolidado.DataAtualizacao.ToString("dd/MM/yyyy HH:mm:ss");

            worksheet.Cells[linha, 2].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[linha, 3].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[linha, 4].Style.Numberformat.Format = "#,##0.00";
            worksheet.Cells[linha, 5].Style.Numberformat.Format = "#,##0.00";

            linha++;
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
    }

    public async Task<byte[]> GerarRelatorioCsvAsync(DateTime dataInicio, DateTime dataFim)
    {
        var consolidados = await _repository.ObterPorPeriodoAsync(dataInicio, dataFim);

        using var stream = new MemoryStream();
        using var writer = new StreamWriter(stream, Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteField("Data");
        csv.WriteField("Saldo Inicial");
        csv.WriteField("Total Débitos");
        csv.WriteField("Total Créditos");
        csv.WriteField("Saldo Final");
        csv.WriteField("Quantidade Transações");
        csv.WriteField("Última Atualização");
        csv.NextRecord();

        foreach (var consolidado in consolidados)
        {
            csv.WriteField(consolidado.Data.ToString("dd/MM/yyyy"));
            csv.WriteField(consolidado.SaldoInicial.ToString("F2"));
            csv.WriteField(consolidado.TotalDebitos.ToString("F2"));
            csv.WriteField(consolidado.TotalCreditos.ToString("F2"));
            csv.WriteField(consolidado.SaldoFinal.ToString("F2"));
            csv.WriteField(consolidado.QuantidadeTransacoes);
            csv.WriteField(consolidado.DataAtualizacao.ToString("dd/MM/yyyy HH:mm:ss"));
            csv.NextRecord();
        }

        writer.Flush();
        return stream.ToArray();
    }
}