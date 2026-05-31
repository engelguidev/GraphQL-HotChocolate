namespace TicketAPI.Models;

/// <summary>
/// Entidade que representa um departamento que gerencia tickets
/// Relacionamento 1:N com Ticket
/// </summary>
public class Departamento
{
    /// <summary>
    /// ID único do departamento
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do departamento (ex: "PIX", "Fraude", "Cartões")
    /// Deve ser único no banco de dados
    /// </summary>
    public required string Nome { get; set; }

    /// <summary>
    /// Email para contato do departamento
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Telefone para contato do departamento
    /// </summary>
    public required string Telefone { get; set; }

    /// <summary>
    /// Relacionamento: Um departamento pode ter muitos tickets
    /// </summary>
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
