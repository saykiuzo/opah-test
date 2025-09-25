using Microsoft.EntityFrameworkCore;
using Consolidado.Domain.Entities;

namespace Consolidado.Infrastructure.Data;

public class ConsolidadoDbContext : DbContext
{
    public ConsolidadoDbContext(DbContextOptions<ConsolidadoDbContext> options) : base(options)
    {
    }

    public DbSet<ConsolidadoDiario> ConsolidadosDiarios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ConsolidadoDiario>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.SaldoInicial)
                .HasColumnName("saldo_inicial")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.TotalDebitos)
                .HasColumnName("total_debitos")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.TotalCreditos)
                .HasColumnName("total_creditos")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.SaldoFinal)
                .HasColumnName("saldo_final")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.QuantidadeTransacoes)
                .HasColumnName("quantidade_transacoes")
                .IsRequired();

            entity.Property(e => e.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.HasIndex(e => e.Data)
                .IsUnique()
                .HasDatabaseName("IX_ConsolidadoDiario_Data");

            entity.HasIndex(e => e.DataAtualizacao)
                .HasDatabaseName("IX_ConsolidadoDiario_DataAtualizacao");
        });

        base.OnModelCreating(modelBuilder);
    }
}