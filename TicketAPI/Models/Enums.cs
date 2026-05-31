namespace TicketAPI.Models;

/// <summary>
/// Enumeração dos tipos de reclamações suportadas
/// </summary>
public enum TipoReclamacao
{
    /// <summary>PIX - Reclamações relacionadas a transferências PIX</summary>
    PIX,
    
    /// <summary>CARTAO - Reclamações relacionadas a cartões</summary>
    CARTAO,
    
    /// <summary>CONTA - Reclamações relacionadas a conta bancária</summary>
    CONTA,
    
    /// <summary>FRAUDE - Reclamações de fraude</summary>
    FRAUDE,
    
    /// <summary>EMPRESTIMO - Reclamações sobre empréstimos</summary>
    EMPRESTIMO
}

/// <summary>
/// Enumeração dos níveis de severidade das reclamações
/// </summary>
public enum NivelSeveridade
{
    /// <summary>BAIXA - Reclamação com impacto baixo</summary>
    BAIXA,
    
    /// <summary>MEDIA - Reclamação com impacto médio</summary>
    MEDIA,
    
    /// <summary>ALTA - Reclamação com impacto alto</summary>
    ALTA,
    
    /// <summary>CRITICA - Reclamação com impacto crítico que exige atenção imediata</summary>
    CRITICA
}

/// <summary>
/// Enumeração dos possíveis status de um ticket
/// </summary>
public enum StatusTicket
{
    /// <summary>ABERTO - Ticket recém-criado, ainda não foi analisado</summary>
    ABERTO,
    
    /// <summary>EMANALISE - Ticket em processo de análise pela equipe</summary>
    EMANALISE,
    
    /// <summary>RESOLVIDO - Ticket foi resolvido e encerrado</summary>
    RESOLVIDO
}
