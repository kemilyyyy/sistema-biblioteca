using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class Emprestimo : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    public int ClienteId { get; set; }

    [Required]
    public int LivroId { get; set; }

    [Required]
    public int FuncionarioId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DataRetirada { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime DataPrevisaoDevolucao { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataDevolucao { get; set; }

    public Cliente? Cliente { get; set; }
    public Livro? Livro { get; set; }
    public Funcionario? Funcionario { get; set; }

    [NotMapped]
    public bool EstaAtrasado => DataDevolucao == null && DataPrevisaoDevolucao < DateTime.UtcNow.Date;

    [NotMapped]
    public int DiasAtraso
    {
        get
        {
            var compareDate = DataDevolucao?.Date ?? DateTime.UtcNow.Date;
            var diff = (compareDate - DataPrevisaoDevolucao.Date).Days;
            return diff > 0 ? diff : 0;
        }
    }

    public decimal CalcularMulta(decimal valorPorDia)
    {
        if (valorPorDia <= 0) return 0m;
        return DiasAtraso * valorPorDia;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DataPrevisaoDevolucao <= DataRetirada)
        {
            yield return new ValidationResult(
                "A data de devolução deve ser posterior à data de retirada.",
                new[] { nameof(DataPrevisaoDevolucao) });
        }
    }
}
