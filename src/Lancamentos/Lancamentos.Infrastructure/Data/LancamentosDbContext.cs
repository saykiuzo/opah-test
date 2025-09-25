using Microsoft.EntityFrameworkCore;
using Lancamentos.Domain.Entities;

namespace Lancamentos.Infrastructure.Data;

public class LancamentosDbContext : DbContext
{
    public LancamentosDbContext(DbContextOptions<LancamentosDbContext> options) : base(options)
    {
    }

    public DbSet<Lancamento> Lancamentos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Lancamento>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Id)
                .HasColumnName("id");

            entity.Property(e => e.Data)
                .HasColumnName("data")
                .HasColumnType("date")
                .IsRequired();

            entity.Property(e => e.Valor)
                .HasColumnName("valor")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasConversion<int>()
                .IsRequired();

            entity.Property(e => e.Descricao)
                .HasColumnName("descricao")
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(e => e.DataCriacao)
                .HasColumnName("data_criacao")
                .HasColumnType("timestamp with time zone")
                .IsRequired();

            entity.Property(e => e.DataAtualizacao)
                .HasColumnName("data_atualizacao")
                .HasColumnType("timestamp with time zone");

            entity.HasIndex(e => e.Data);
            entity.HasIndex(e => new { e.Data, e.Tipo });
        });

        base.OnModelCreating(modelBuilder);
    }
}