using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistema_biblioteca.Data;
using sistema_biblioteca.ViewModels;

namespace sistema_biblioteca.Controllers;

public class AtendenteController : AuthenticatedController
{
    private readonly LibraryContext _context;

    public AtendenteController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("Role") != "Atendente")
        {
            TempData["Error"] = "Faça login como atendente para acessar esta área.";
            return RedirectToAction("Login", "Auth");
        }

        var livros = await _context.Livros.AsNoTracking().ToListAsync();
        var emprestimosAtivos = await _context.Emprestimos.AsNoTracking()
            .Where(e => e.DataDevolucao == null)
            .ToListAsync();

        var ativosPorLivro = emprestimosAtivos
            .GroupBy(e => e.LivroId)
            .ToDictionary(g => g.Key, g => g.Count());

        var vm = new RoleDashboardViewModel
        {
            RoleName = "Atendente",
            TotalLivros = livros.Count,
            LivrosDisponiveis = livros.Where(l => l.Ativo).Sum(l => Math.Max(0, l.QtdEstoque - (ativosPorLivro.TryGetValue(l.Id, out var count) ? count : 0))),
            LivrosEmprestados = emprestimosAtivos.Count,
            LivrosInativos = livros.Count(l => !l.Ativo),
            ClientesCadastrados = await _context.Clientes.CountAsync(),
            UsuariosCadastrados = await _context.Usuarios.CountAsync(),
            EmprestimosAtrasados = emprestimosAtivos.Count(e => e.DataPrevisaoDevolucao < DateTime.UtcNow.Date)
        };

        return View(vm);
    }
}
