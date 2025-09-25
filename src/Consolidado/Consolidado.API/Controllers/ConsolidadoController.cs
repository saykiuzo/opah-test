using Microsoft.AspNetCore.Mvc;
using MediatR;
using Consolidado.Application.Queries;
using Consolidado.Application.DTOs;

namespace Consolidado.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ConsolidadoController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ConsolidadoController> _logger;

    public ConsolidadoController(IMediator mediator, ILogger<ConsolidadoController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{data:datetime}")]
    [ProducesResponseType(typeof(ConsolidadoDiarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsolidadoDiarioResponse>> ObterConsolidadoPorData(DateTime data)
    {
        var query = new ObterConsolidadoPorDataQuery { Data = data };
        var resultado = await _mediator.Send(query);

        if (resultado == null)
        {
            return NotFound(new { message = $"Consolidado não encontrado para a data {data:dd/MM/yyyy}" });
        }

        return Ok(resultado);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ConsolidadoDiarioResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<ConsolidadoDiarioResponse>>> ObterConsolidadoPorPeriodo(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim)
    {
        if (dataInicio > dataFim)
        {
            return BadRequest(new { message = "Data de início não pode ser maior que data de fim" });
        }

        var query = new ObterConsolidadoPorPeriodoQuery
        {
            DataInicio = dataInicio,
            DataFim = dataFim
        };

        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    [HttpGet("{data:datetime}/export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportarRelatorio(DateTime data, [FromQuery] string formato = "xlsx")
    {
        try
        {
            var query = new GerarRelatorioQuery
            {
                DataInicio = data,
                DataFim = data,
                Formato = formato
            };

            var arquivo = await _mediator.Send(query);

            var contentType = formato.ToLower() == "csv" 
                ? "text/csv" 
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var nomeArquivo = $"consolidado_{data:yyyyMMdd}.{formato}";

            var outputPath = Path.Combine("/app", "output");
            Directory.CreateDirectory(outputPath);
            
            var caminhoCompleto = Path.Combine(outputPath, nomeArquivo);
            await System.IO.File.WriteAllBytesAsync(caminhoCompleto, arquivo);
            
            _logger.LogInformation("Arquivo {Formato} salvo em: {Caminho}", formato.ToUpper(), caminhoCompleto);

            return File(arquivo, contentType, nomeArquivo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório para {Data} no formato {Formato}", data, formato);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("export")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportarRelatorioPorPeriodo(
        [FromQuery] DateTime dataInicio,
        [FromQuery] DateTime dataFim,
        [FromQuery] string formato = "xlsx")
    {
        try
        {
            if (dataInicio > dataFim)
            {
                return BadRequest(new { message = "Data de início não pode ser maior que data de fim" });
            }

            var query = new GerarRelatorioQuery
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                Formato = formato
            };

            var arquivo = await _mediator.Send(query);

            var contentType = formato.ToLower() == "csv" 
                ? "text/csv" 
                : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            var nomeArquivo = $"consolidado_{dataInicio:yyyyMMdd}_{dataFim:yyyyMMdd}.{formato}";

            var outputPath = Path.Combine("/app", "output");
            Directory.CreateDirectory(outputPath);
            
            var caminhoCompleto = Path.Combine(outputPath, nomeArquivo);
            await System.IO.File.WriteAllBytesAsync(caminhoCompleto, arquivo);
            
            _logger.LogInformation("Arquivo {Formato} período salvo em: {Caminho}", formato.ToUpper(), caminhoCompleto);

            return File(arquivo, contentType, nomeArquivo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório para período {DataInicio} - {DataFim} no formato {Formato}", 
                dataInicio, dataFim, formato);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("relatorio/{data:datetime}/excel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportarRelatorioExcel(DateTime data)
    {
        try
        {
            var query = new GerarRelatorioQuery
            {
                DataInicio = data,
                DataFim = data,
                Formato = "xlsx"
            };

            var arquivo = await _mediator.Send(query);
            var nomeArquivo = $"consolidado_{data:yyyyMMdd}.xlsx";

            var outputPath = Path.Combine("/app", "output");
            Directory.CreateDirectory(outputPath);
            
            var caminhoCompleto = Path.Combine(outputPath, nomeArquivo);
            await System.IO.File.WriteAllBytesAsync(caminhoCompleto, arquivo);
            
            _logger.LogInformation("Arquivo Excel salvo em: {Caminho}", caminhoCompleto);

            return File(arquivo, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nomeArquivo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório Excel para {Data}", data);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("relatorio/{data:datetime}/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportarRelatorioCsv(DateTime data)
    {
        try
        {
            var query = new GerarRelatorioQuery
            {
                DataInicio = data,
                DataFim = data,
                Formato = "csv"
            };

            var arquivo = await _mediator.Send(query);
            var nomeArquivo = $"consolidado_{data:yyyyMMdd}.csv";

            var outputPath = Path.Combine("/app", "output");
            Directory.CreateDirectory(outputPath);
            
            var caminhoCompleto = Path.Combine(outputPath, nomeArquivo);
            await System.IO.File.WriteAllBytesAsync(caminhoCompleto, arquivo);
            
            _logger.LogInformation("Arquivo CSV salvo em: {Caminho}", caminhoCompleto);

            return File(arquivo, "text/csv", nomeArquivo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar relatório CSV para {Data}", data);
            return StatusCode(500, new { message = "Erro interno do servidor" });
        }
    }

    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult HealthCheck()
    {
        return Ok(new { 
            status = "healthy", 
            timestamp = DateTime.UtcNow,
            service = "Consolidado.API"
        });
    }
}