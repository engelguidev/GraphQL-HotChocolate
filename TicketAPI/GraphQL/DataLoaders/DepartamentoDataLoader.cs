using HotChocolate.Execution.Configuration;
using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.Models;

namespace TicketAPI.GraphQL.DataLoaders;

/// <summary>
/// DataLoader para evitar o problema N+1 ao buscar departamentos
/// Agrupa múltiplas requisições de departamentos em uma única query de banco de dados
/// </summary>
public class DepartamentoDataLoader : BatchDataLoader<int, Departamento>
{
    private readonly IDbContextFactory<TicketDbContext> _contextFactory;

    /// <summary>
    /// Construtor do DataLoader
    /// </summary>
    /// <param name="contextFactory">Factory para criar instâncias do DbContext</param>
    /// <param name="batchScheduler">Scheduler para agendar o batch de carregamento</param>
    public DepartamentoDataLoader(
        IDbContextFactory<TicketDbContext> contextFactory,
        IBatchScheduler batchScheduler)
        : base(batchScheduler, new DataLoaderOptions())
    {
        _contextFactory = contextFactory;
    }

    /// <summary>
    /// Carrega múltiplos departamentos de uma vez (evita N+1)
    /// 
    /// Exemplo:
    /// - Sem DataLoader: 1 query para tickets + 100 queries para departamentos (N+1)
    /// - Com DataLoader: 1 query para tickets + 1 query para TODOS os departamentos
    /// </summary>
    /// <param name="keys">Lista de IDs de departamentos a carregar</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Dicionário dos departamentos mapeados por ID</returns>
    protected override async Task<IReadOnlyDictionary<int, Departamento>> LoadBatchAsync(
        IReadOnlyList<int> keys,
        CancellationToken cancellationToken)
    {
        // Cria uma nova instância do DbContext
        await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);

        // Carrega TODOS os departamentos com os IDs especificados em UMA ÚNICA query
        var departamentos = await context.Departamentos
            .AsNoTracking() // Sem rastreamento para melhor performance
            .Where(d => keys.Contains(d.Id))
            .ToListAsync(cancellationToken);

        // Retorna um dicionário de departamentos mapeados por ID
        // Isso permite que o GraphQL acesse rapidamente cada departamento
        return departamentos.ToDictionary(d => d.Id);
    }
}
