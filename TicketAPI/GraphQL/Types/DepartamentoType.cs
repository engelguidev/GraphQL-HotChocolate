using TicketAPI.Models;

namespace TicketAPI.GraphQL.Types;

/// <summary>
/// Tipo GraphQL para representar um Departamento
/// Expõe os dados do departamento para queries GraphQL
/// </summary>
public class DepartamentoType
{
    /// <summary>
    /// ID único do departamento
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do departamento
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Email do departamento
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Telefone do departamento
    /// </summary>
    public string Telefone { get; set; } = string.Empty;

    /// <summary>
    /// Converte uma entidade Departamento em DepartamentoType
    /// </summary>
    /// <param name="departamento">Entidade do banco de dados</param>
    /// <returns>Tipo GraphQL correspondente</returns>
    public static DepartamentoType FromDepartamento(Departamento departamento)
    {
        return new DepartamentoType
        {
            Id = departamento.Id,
            Nome = departamento.Nome,
            Email = departamento.Email,
            Telefone = departamento.Telefone
        };
    }
}
