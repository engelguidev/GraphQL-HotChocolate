using Microsoft.EntityFrameworkCore;
using TicketAPI.Models;

namespace TicketAPI.Data;

/// <summary>
/// DbContext para a aplicação de tickets
/// Gerencia a conexão com o banco de dados SQL Server e as migrations
/// </summary>
public class TicketDbContext : DbContext
{
    /// <summary>
    /// Construtor do DbContext que recebe as opções de configuração
    /// </summary>
    public TicketDbContext(DbContextOptions<TicketDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet para a entidade Ticket
    /// Permite consultas LINQ contra a tabela de tickets
    /// </summary>
    public DbSet<Ticket> Tickets { get; set; } = null!;

    /// <summary>
    /// DbSet para a entidade Departamento
    /// Permite consultas LINQ contra a tabela de departamentos
    /// </summary>
    public DbSet<Departamento> Departamentos { get; set; } = null!;

    /// <summary>
    /// Configuração do modelo de dados
    /// Define as constraints, índices e relacionamentos
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração da entidade Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            // Chave primária
            entity.HasKey(e => e.Id);

            // Configurações das propriedades
            entity.Property(e => e.Protocolo)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Protocolo");

            entity.Property(e => e.NomeCliente)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnName("NomeCliente");

            entity.Property(e => e.Tipo)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("Tipo");

            entity.Property(e => e.Severidade)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("Severidade");

            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasColumnName("Status")
                .HasDefaultValue(StatusTicket.ABERTO);

            entity.Property(e => e.DataCriacao)
                .IsRequired()
                .HasColumnName("DataCriacao");

            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .HasColumnName("Descricao");

            // Índice para buscas por Protocolo
            entity.HasIndex(e => e.Protocolo)
                .IsUnique();

            // Índice para filtros por Status e Severidade
            entity.HasIndex(e => new { e.Status, e.Severidade });

            // Índice para filtros por Data de Criação
            entity.HasIndex(e => e.DataCriacao);

            // Nome da tabela
            entity.ToTable("Tickets");
        });

        // Configuração da entidade Departamento
        modelBuilder.Entity<Departamento>(entity =>
        {
            // Chave primária
            entity.HasKey(e => e.Id);

            // Configurações das propriedades
            entity.Property(e => e.Nome)
                .IsRequired()
                .HasMaxLength(450)
                .HasColumnName("Nome");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(450)
                .HasColumnName("Email");

            entity.Property(e => e.Telefone)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Telefone");

            // Índice para buscas por Nome (Unique)
            entity.HasIndex(e => e.Nome)
                .IsUnique();

            // Nome da tabela
            entity.ToTable("Departamentos");
        });

        // Relacionamento: Ticket -> Departamento (Many:One)
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Departamento)
            .WithMany(d => d.Tickets)
            .HasForeignKey(t => t.DepartamentoId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    /// <summary>
    /// Seed de dados inicial com alguns tickets de exemplo
    /// Este método é chamado após as migrations serem aplicadas
    /// </summary>
    public async Task SeedDataAsync()
    {
        // Verifica se já existem departamentos
        if (await Departamentos.AnyAsync())
        {
            return; // Se houver departamentos, assume que os dados já foram seed
        }

        // Cria departamentos
        var departamentos = new List<Departamento>
        {
            new()
            {
                Nome = "PIX",
                Email = "pix@banco.com",
                Telefone = "(11) 3000-0001"
            },
            new()
            {
                Nome = "Fraude",
                Email = "fraude@banco.com",
                Telefone = "(11) 3000-0002"
            },
            new()
            {
                Nome = "Cartões",
                Email = "cartoes@banco.com",
                Telefone = "(11) 3000-0003"
            },
            new()
            {
                Nome = "Contas",
                Email = "contas@banco.com",
                Telefone = "(11) 3000-0004"
            },
            new()
            {
                Nome = "Empréstimos",
                Email = "emprestimos@banco.com",
                Telefone = "(11) 3000-0005"
            }
        };

        await Departamentos.AddRangeAsync(departamentos);
        await SaveChangesAsync();

        // Agora cria os tickets com departamentos
        var tickets = new List<Ticket>
        {
            new()
            {
                Protocolo = "PIX-2024-001",
                NomeCliente = "João Silva",
                Tipo = TipoReclamacao.PIX,
                Severidade = NivelSeveridade.CRITICA,
                Status = StatusTicket.ABERTO,
                DataCriacao = DateTime.UtcNow.AddDays(-5),
                Descricao = "Transferência PIX não recebida após 2 horas",
                DepartamentoId = 1 // PIX
            },
            new()
            {
                Protocolo = "FRAUDE-2024-002",
                NomeCliente = "Maria Santos",
                Tipo = TipoReclamacao.FRAUDE,
                Severidade = NivelSeveridade.CRITICA,
                Status = StatusTicket.EMANALISE,
                DataCriacao = DateTime.UtcNow.AddDays(-3),
                Descricao = "Movimento não autorizado na conta",
                DepartamentoId = 2 // Fraude
            },
            new()
            {
                Protocolo = "CARTAO-2024-003",
                NomeCliente = "Pedro Costa",
                Tipo = TipoReclamacao.CARTAO,
                Severidade = NivelSeveridade.ALTA,
                Status = StatusTicket.ABERTO,
                DataCriacao = DateTime.UtcNow.AddDays(-1),
                Descricao = "Cartão bloqueado sem motivo aparente",
                DepartamentoId = 3 // Cartões
            },
            new()
            {
                Protocolo = "CONTA-2024-004",
                NomeCliente = "Ana Oliveira",
                Tipo = TipoReclamacao.CONTA,
                Severidade = NivelSeveridade.MEDIA,
                Status = StatusTicket.ABERTO,
                DataCriacao = DateTime.UtcNow,
                Descricao = "Não consegue acessar a conta pela aplicação",
                DepartamentoId = 4 // Contas
            },
            new()
            {
                Protocolo = "EMPRESTIMO-2024-005",
                NomeCliente = "Carlos Mendes",
                Tipo = TipoReclamacao.EMPRESTIMO,
                Severidade = NivelSeveridade.BAIXA,
                Status = StatusTicket.RESOLVIDO,
                DataCriacao = DateTime.UtcNow.AddDays(-10),
                Descricao = "Dúvida sobre taxa de juros do empréstimo",
                DepartamentoId = 5 // Empréstimos
            }
        };

        await Tickets.AddRangeAsync(tickets);
        await SaveChangesAsync();
    }
}
