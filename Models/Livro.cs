using System.ComponentModel.DataAnnotations;

namespace sistema_biblioteca.Models;

public class Livro
{
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Autor { get; set; } = string.Empty;

    [StringLength(20)]
    public string? ISBN { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DataPublicacao { get; set; }

    [StringLength(100)]
    public string? Genero { get; set; }

    [Required]
    [Range(0, 10000)]
    public int QtdEstoque { get; set; } = 1;

    public bool Ativo { get; set; } = true;

    public ICollection<Emprestimo> Emprestimos { get; set; } = new HashSet<Emprestimo>();
}
