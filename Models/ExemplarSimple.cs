using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class ExemplarSimple
{
    [Key]
    [Column("ID_EXEMPLAR")]
    public int IdExemplar { get; set; }

    [Column("ID_LIVRO")]
    public int IdLivro { get; set; }

    public LivroSimple? Livro { get; set; }

    [Required]
    [StringLength(50)]
    [Column("CODIGO_EXEMPLAR")]
    public string CodigoExemplar { get; set; } = string.Empty;

    [StringLength(20)]
    [Column("STATUS")]
    public string Status { get; set; } = "DISPONIVEL";

    public ICollection<EmprestimoSimple>? Emprestimos { get; set; }
}
