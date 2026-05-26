using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class FuncionarioSimple
{
    [Key]
    [Column("ID_FUNCIONARIO")]
    public int IdFuncionario { get; set; }

    [Required]
    [StringLength(100)]
    [Column("NOME")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(50)]
    [Column("CARGO")]
    public string? Cargo { get; set; }

    [StringLength(50)]
    [Column("LOGIN")]
    public string? Login { get; set; }

    [StringLength(100)]
    [Column("SENHA")]
    public string? Senha { get; set; }

    public ICollection<EmprestimoSimple>? Emprestimos { get; set; }
}
