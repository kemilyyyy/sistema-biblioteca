namespace sistema_biblioteca.ViewModels;

public class RoleDashboardViewModel
{
    public string RoleName { get; set; } = string.Empty;
    public int TotalLivros { get; set; }
    public int LivrosDisponiveis { get; set; }
    public int LivrosEmprestados { get; set; }
    public int LivrosInativos { get; set; }
    public int ClientesCadastrados { get; set; }
    public int UsuariosCadastrados { get; set; }
    public int EmprestimosAtrasados { get; set; }
}
