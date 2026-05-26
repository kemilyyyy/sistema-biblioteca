using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sistema_biblioteca.Models;

public class LivroSimple
{
    [Key]
    [Column("ID_LIVRO")]
    public int IdLivro { get; set; }

    [Required]
    [StringLength(150)]
    [Column("TITULO")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(100)]
    [Column("AUTOR")]
    public string? Autor { get; set; }

    [StringLength(100)]
    [Column("EDITORA")]
    public string? Editora { get; set; }

    [StringLength(20)]
    [Column("ISBN")]
    public string? Isbn { get; set; }

    [Column("ANO")]
    public int? Ano { get; set; }

    public ICollection<ExemplarSimple>? Exemplares { get; set; }
}
