using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class MultaSimple
{
    [Key]
    [Column("ID_MULTA")]
    public int IdMulta { get; set; }

    [Column("ID_EMPRESTIMO")]
    public int IdEmprestimo { get; set; }
    public EmprestimoSimple? Emprestimo { get; set; }

    [Column("VALOR")]
    public decimal Valor { get; set; }

    [Column("DIAS_ATRASO")]
    public int DiasAtraso { get; set; }

    [StringLength(20)]
    [Column("STATUS_PAGAMENTO")]
    public string? StatusPagamento { get; set; }
}
