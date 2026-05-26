using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class Funcionario : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Cargo { get; set; } = string.Empty;

    [EmailAddress]
    [StringLength(200)]
    public string? Email { get; set; }

    [Required]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [StringLength(200)]
    public string? PasswordHash { get; set; }

    [StringLength(50)]
    public string? Role { get; set; }

    [StringLength(20)]
    public string? CPF { get; set; }

    public ICollection<Emprestimo> Emprestimos { get; set; } = new HashSet<Emprestimo>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(Email) && !new EmailAddressAttribute().IsValid(Email))
        {
            yield return new ValidationResult("Email inválido.", new[] { nameof(Email) });
        }
    }
}
