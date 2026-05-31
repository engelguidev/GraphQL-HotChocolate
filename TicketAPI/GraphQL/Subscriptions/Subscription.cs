using HotChocolate.Execution.Configuration;
using HotChocolate.Subscriptions;
using TicketAPI.GraphQL.Types;
using TicketAPI.Models;

namespace TicketAPI.GraphQL.Subscriptions;

/// <summary>
/// Classe que define todas as subscriptions GraphQL disponíveis
/// As subscriptions permitem que clientes recebam atualizações em tempo real
/// quando eventos específicos ocorrem no servidor (Server Push)
/// </summary>
public class Subscription
{
    /// <summary>
    /// Subscription que notifica sempre que um ticket com severidade CRITICA é criado
    /// 
    /// Fluxo de funcionamento:
    /// 1. Cliente WebSocket se conecta e subscrevê a este evento
    /// 2. Quando uma mutation CriarTicket é executada com Severidade = CRITICA
    /// 3. Um evento é publicado via ITopicEventSender no servidor
    /// 4. Todos os clientes conectados recebem a notificação em tempo real
    /// 5. O evento é continuado até que o cliente desconecte
    /// 
    /// Exemplo de uso GraphQL:
    /// subscription {
    ///   ticketCriticoCriado {
    ///     id
    ///     protocolo
    ///     nomeCliente
    ///     tipo
    ///     severidade
    ///     status
    ///     dataCriacao
    ///   }
    /// }
    /// 
    /// Como testar com GraphQL Banana Cake Pop (BCP):
    /// 1. Abra o BCP em http://localhost:5000/graphql
    /// 2. Crie uma aba nova e execute esta subscription
    /// 3. Em outra aba, execute a mutation CriarTicket com severidade CRITICA
    /// 4. A subscription receberá a notificação em tempo real na primeira aba
    /// </summary>
    /// <param name="eventReceiver">
    /// Receptor de eventos injetado que aguarda eventos no tópico "TicketCriticoCriado"
    /// </param>
    /// <returns>
    /// Um IAsyncEnumerable que emite TicketType sempre que um ticket crítico é criado
    /// A conexão persiste até o cliente desconectar ou um erro ocorrer
    /// </returns>
    [Subscribe]
    public async IAsyncEnumerable<TicketType> TicketCriticoCriado(
        [EventMessage] Ticket ticket)
    {
        // Converte o modelo Ticket para o tipo GraphQL e o emite para o cliente
        // Este método será chamado cada vez que um evento é publicado no tópico "TicketCriticoCriado"
        yield return TicketType.FromTicket(ticket);
    }

    /// <summary>
    /// Subscription de monitoramento que notifica sobre QUALQUER ticket criado (não apenas críticos)
    /// Útil para dashboards que precisam de visibilidade total
    /// 
    /// Exemplo de uso GraphQL:
    /// subscription {
    ///   ticketCriado {
    ///     protocolo
    ///     nomeCliente
    ///     severidade
    ///   }
    /// }
    /// </summary>
    /// <param name="eventReceiver">Receptor de eventos injetado</param>
    /// <returns>IAsyncEnumerable que emite todos os tickets criados</returns>
    [Subscribe]
    public async IAsyncEnumerable<TicketType> TicketCriado(
        [EventMessage] Ticket ticket)
    {
        yield return TicketType.FromTicket(ticket);
    }
}
