using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FinPulse.Enums;
using FinPulse.Models;

namespace FinPulse.Context;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    // Construtor correto:
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<ContaBancaria> ContasBancarias { get; set; }
    public DbSet<Transacao> Transacoes { get; set; }
    public DbSet<Orcamento> Orcamentos { get; set; }
    public DbSet<MetaFinanceira> MetasFinanceiras { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ⚠️ OBRIGATÓRIO para Identity: Configura as tabelas AspNetUsers, AspNetRoles, etc.
        base.OnModelCreating(modelBuilder);

        // Configurações de Precisão Decimal
        modelBuilder.Entity<Transacao>()
            .Property(t => t.Valor)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ContaBancaria>()
            .Property(c => c.Saldo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Orcamento>()
            .Property(o => o.ValorLimite)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Orcamento>()
            .Property(o => o.ValorGasto)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MetaFinanceira>()
            .Property(m => m.ValorAlvo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<MetaFinanceira>()
            .Property(m => m.ValorAtual)
            .HasPrecision(18, 2);

        // Relacionamentos e Delete Behavior
        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.Categoria)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transacao>()
            .HasOne(t => t.ContaBancaria)
            .WithMany(c => c.Transacoes)
            .HasForeignKey(t => t.ContaBancariaId)
            .OnDelete(DeleteBehavior.Restrict);

        // SEED DATA: Categorias Iniciais Padrão
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { CategoriaId = 1, Nome = "Alimentação", Tipo = TipoTransacao.Despesa, Icone = "utensils", Cor = "#EF4444" },
            new Categoria { CategoriaId = 2, Nome = "Transporte", Tipo = TipoTransacao.Despesa, Icone = "car", Cor = "#F97316" },
            new Categoria { CategoriaId = 3, Nome = "Moradia", Tipo = TipoTransacao.Despesa, Icone = "home", Cor = "#8B5CF6" },
            new Categoria { CategoriaId = 4, Nome = "Saúde", Tipo = TipoTransacao.Despesa, Icone = "heart-pulse", Cor = "#EC4899" },
            new Categoria { CategoriaId = 5, Nome = "Lazer", Tipo = TipoTransacao.Despesa, Icone = "gamepad", Cor = "#EAB308" },
            new Categoria { CategoriaId = 6, Nome = "Educação", Tipo = TipoTransacao.Despesa, Icone = "graduation-cap", Cor = "#3B82F6" },
            new Categoria { CategoriaId = 7, Nome = "Salário", Tipo = TipoTransacao.Receita, Icone = "money-bill-wave", Cor = "#10B981" },
            new Categoria { CategoriaId = 8, Nome = "Investimentos", Tipo = TipoTransacao.Receita, Icone = "chart-line", Cor = "#06B6D4" },
            new Categoria { CategoriaId = 9, Nome = "Freelance / Extra", Tipo = TipoTransacao.Receita, Icone = "laptop-code", Cor = "#6366F1" }
        );
    }
}