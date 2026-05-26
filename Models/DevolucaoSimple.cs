using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class DevolucaoSimple
{
    [Key]
    [Column("ID_DEVOLUCAO")]
    public int IdDevolucao { get; set; }

    [Column("ID_EMPRESTIMO")]
    public int IdEmprestimo { get; set; }
    public EmprestimoSimple? Emprestimo { get; set; }

    [DataType(DataType.Date)]
    [Column("DATA_DEVOLUCAO")]
    public DateTime? DataDevolucao { get; set; }
}
