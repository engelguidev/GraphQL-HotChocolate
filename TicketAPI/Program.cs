using Microsoft.EntityFrameworkCore;
using TicketAPI.Data;
using TicketAPI.GraphQL.DataLoaders;
using TicketAPI.GraphQL.Mutations;
using TicketAPI.GraphQL.Queries;
using TicketAPI.GraphQL.Subscriptions;
using TicketAPI.GraphQL.Types;
using TicketAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ========================================

// Recupera a string de conexão do appsettings.json
// Para ambiente de desenvolvimento, você pode usar SQL Server LocalDB ou SqlExpress
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Adiciona o DbContext ao container de injeção de dependências
// DbContext gerencia a conexão e as operações com o banco de dados
builder.Services.AddDbContext<TicketDbContext>(options =>
    options.UseSqlServer(connectionString)
);

// ========================================
// CONFIGURAÇÃO DO HOT CHOCOLATE (GraphQL)
// ========================================

// Adiciona o Hot Chocolate GraphQL Server
builder.Services
    .AddGraphQLServer()
    // Adiciona o tipo Query (para leitura de dados)
    .AddQueryType<Query>()
    // Adiciona o tipo Mutation (para modificação de dados)
    .AddMutationType<Mutation>()
    // Adiciona o tipo Subscription (para eventos em tempo real via WebSocket)
    .AddSubscriptionType<Subscription>()
    // Adiciona suporte para tipos enumerados do .NET
    .AddEnumType<TipoReclamacao>()
    .AddEnumType<NivelSeveridade>()
    .AddEnumType<StatusTicket>()
    // Adiciona tipos GraphQL personalizados
    .AddType<TicketAPI.GraphQL.Types.TicketType>()
    .AddType<TicketAPI.GraphQL.Types.DepartamentoType>()
    // Adiciona DataLoader para evitar problemas N+1 nas queries
    .AddDataLoader<DepartamentoDataLoader>()
    // Adiciona suporte a subscriptions com WebSocket
    // Usa o sistema de tópicos em memória para pub/sub
    .AddInMemorySubscriptions();

// ========================================
// ADICIONA SERVIÇOS ADICIONAIS
// ========================================

// Adiciona suporte a CORS para que o GraphQL Banana Cake Pop possa fazer requisições
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// ========================================
// CONFIGURAÇÃO DA APLICAÇÃO
// ========================================

var app = builder.Build();

// Usa CORS
app.UseCors("AllowAll");

// ========================================
// DATABASE INITIALIZATION E SEEDING
// ========================================

// Executa as migrations e faz o seeding de dados iniciais
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TicketDbContext>();

        // Aplica as migrations pendentes
        // Se o banco não existir, será criado
        // Se existir, apenas as migrations não aplicadas serão executadas
        await context.Database.MigrateAsync();

        // Faz seed de dados iniciais (se a tabela estiver vazia)
        await context.SeedDataAsync();

        Console.WriteLine("✓ Banco de dados inicializado com sucesso!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Erro ao inicializar o banco de dados: {ex.Message}");
        throw;
    }
}

// ========================================
// ENDPOINTS
// ========================================

// Configura o endpoint do GraphQL
// Mapeia a rota /graphql para o servidor GraphQL
app.MapGraphQL("/graphql");

// Rota de status da aplicação (útil para health checks)
app.MapGet("/", () => 
    new { 
        message = "API GraphQL de Tickets - Bem-vindo!",
        graphql = "Acesse http://localhost:5000/graphql",
        version = "1.0.0"
    }
);

// ========================================
// INICIALIZAÇÃO DA APLICAÇÃO
// ========================================

Console.WriteLine("Iniciando API GraphQL de Tickets...");
Console.WriteLine("Disponível em: http://localhost:5000");
Console.WriteLine("GraphQL Playground: http://localhost:5000/graphql");
Console.WriteLine("Pronto para receber requisições!\n");

app.Run();
