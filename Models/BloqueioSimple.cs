using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class BloqueioSimple
{
    [Key]
    [Column("ID_BLOQUEIO")]
    public int IdBloqueio { get; set; }

    [Column("ID_USUARIO")]
    public int IdUsuario { get; set; }
    public Usuario? Usuario { get; set; }

    [StringLength(200)]
    [Column("MOTIVO")]
    public string? Motivo { get; set; }

    [DataType(DataType.Date)]
    [Column("DATA_INICIO")]
    public DateTime? DataInicio { get; set; }

    [DataType(DataType.Date)]
    [Column("DATA_FIM")]
    public DateTime? DataFim { get; set; }
}
