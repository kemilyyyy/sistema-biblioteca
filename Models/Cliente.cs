using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class Cliente : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Matricula { get; set; } = string.Empty;

    [StringLength(20)]
    public string? CPF { get; set; }

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Phone]
    [StringLength(20)]
    public string? Telefone { get; set; }

    [StringLength(250)]
    public string? Logradouro { get; set; }

    public bool Bloqueado { get; set; } = false;

    public ICollection<Emprestimo> Emprestimos { get; set; } = new HashSet<Emprestimo>();

    [NotMapped]
    public int EmprestimosAtivos => Emprestimos?.Count(e => e.DataDevolucao == null) ?? 0;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Email) && !new EmailAddressAttribute().IsValid(Email))
        {
            yield return new ValidationResult("Email inválido.", new[] { nameof(Email) });
        }

        if (!string.IsNullOrEmpty(Telefone) && !new PhoneAttribute().IsValid(Telefone))
        {
            yield return new ValidationResult("Telefone inválido.", new[] { nameof(Telefone) });
        }
    }
}
