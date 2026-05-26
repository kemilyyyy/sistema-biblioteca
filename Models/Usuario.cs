using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class Usuario
{
    [Key]
    [Column("ID_USUARIO")]
    public int IdUsuario { get; set; }

    [Required]
    [StringLength(100)]
    [Column("NOME")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(14)]
    [Column("CPF")]
    public string? Cpf { get; set; }

    [EmailAddress]
    [StringLength(100)]
    [Column("EMAIL")]
    public string? Email { get; set; }

    [Phone]
    [StringLength(20)]
    [Column("TELEFONE")]
    public string? Telefone { get; set; }

    [StringLength(20)]
    [Column("STATU")]
    public string Status { get; set; } = "ATIVO";

    [DataType(DataType.Date)]
    [Column("DATA_CADASTRO")]
    public DateTime? DataCadastro { get; set; }

    public ICollection<EmprestimoSimple>? Emprestimos { get; set; }

    public ICollection<BloqueioSimple>? Bloqueios { get; set; }
}
