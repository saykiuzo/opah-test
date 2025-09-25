using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MediatR;
using Serilog;
using Lancamentos.Infrastructure.Data;
using Lancamentos.Domain.Repositories;
using Lancamentos.Infrastructure.Repositories;
using FluxoCaixa.Shared.Messaging;
using Lancamentos.Application.Validators;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { 
        Title = "Lançamentos API", 
        Version = "v1",
        Description = "API para controle de lançamentos financeiros" 
    });
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Log.Information("Usando PostgreSQL");
builder.Services.AddDbContext<LancamentosDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<ILancamentoRepository, LancamentoRepository>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CriarLancamentoCommandValidator).Assembly));

builder.Services.AddValidatorsFromAssemblyContaining<CriarLancamentoCommandValidator>();

var rabbitMqConnection = builder.Configuration.GetConnectionString("RabbitMq");
var useInMemoryDemo = builder.Configuration.GetValue<bool>("UseInMemoryDemo", true);

if (useInMemoryDemo || string.IsNullOrEmpty(rabbitMqConnection))
{
    Log.Information("Usando In-Memory Message Bus para demonstração");
    builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();
}
else
{
    Log.Information("Usando RabbitMQ");
    builder.Services.AddSingleton<IMessageBus>(provider => 
    {
        var logger = provider.GetRequiredService<ILogger<RabbitMqMessageBus>>();
        var maxRetries = 5;
        var delay = TimeSpan.FromSeconds(5);
        
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                logger.LogInformation("Tentativa {Attempt} de conexão ao RabbitMQ (Lançamentos)", i + 1);
                return new RabbitMqMessageBus(rabbitMqConnection!);
            }
            catch (Exception ex) when (i < maxRetries - 1)
            {
                logger.LogWarning(ex, "Falha na tentativa {Attempt} de conexão ao RabbitMQ. Tentando novamente em {Delay}s", i + 1, delay.TotalSeconds);
                Thread.Sleep(delay);
            }
        }
        
        logger.LogError("Falhou ao conectar ao RabbitMQ após {MaxRetries} tentativas", maxRetries);
        throw new InvalidOperationException($"Não foi possível conectar ao RabbitMQ após {maxRetries} tentativas");
    });
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString!);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<LancamentosDbContext>();
    try
    {
        await context.Database.MigrateAsync();
        Log.Information("Database migration completed successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error occurred while migrating database");
    }
}

Log.Information("Lançamentos API starting...");

app.Run();

public partial class Program { } // Para testes de integração