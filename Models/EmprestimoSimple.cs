using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class EmprestimoSimple
{
    [Key]
    [Column("ID_EMPRESTIMO")]
    public int IdEmprestimo { get; set; }

    [Column("ID_USUARIO")]
    public int IdUsuario { get; set; }
    public Usuario? Usuario { get; set; }

    [Column("ID_FUNCIONARIO")]
    public int? IdFuncionario { get; set; }
    public FuncionarioSimple? Funcionario { get; set; }

    [Column("ID_EXEMPLAR")]
    public int IdExemplar { get; set; }
    public ExemplarSimple? Exemplar { get; set; }

    [DataType(DataType.Date)]
    [Column("DATA_EMPRESTIMO")]
    public DateTime? DataEmprestimo { get; set; }

    [DataType(DataType.Date)]
    [Column("DATA_PREVISTA")]
    public DateTime? DataPrevista { get; set; }

    [StringLength(20)]
    [Column("STATUS")]
    public string? Status { get; set; }

    public ICollection<DevolucaoSimple>? Devolucoes { get; set; }

    public ICollection<MultaSimple>? Multas { get; set; }
}
