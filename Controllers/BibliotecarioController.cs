using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistema_biblioteca.Data;
using sistema_biblioteca.Models;
using sistema_biblioteca.ViewModels;

namespace sistema_biblioteca.Controllers;

public class BibliotecarioController : AuthenticatedController
{
    private readonly LibraryContext _context;

    public BibliotecarioController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        if (HttpContext.Session.GetString("Role") != "Bibliotecario")
        {
            TempData["Error"] = "Faça login como bibliotecário para acessar esta área.";
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
            RoleName = "Bibliotecário",
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

    public async Task<IActionResult> Relatorios()
    {
        if (HttpContext.Session.GetString("Role") != "Bibliotecario")
        {
            TempData["Error"] = "Faça login como bibliotecário para acessar esta área.";
            return RedirectToAction("Login", "Auth");
        }

        var atrasados = await _context.Emprestimos.AsNoTracking()
            .Include(e => e.Cliente)
            .Include(e => e.Livro)
            .Where(e => e.DataDevolucao == null && e.DataPrevisaoDevolucao < DateTime.UtcNow.Date)
            .OrderBy(e => e.DataPrevisaoDevolucao)
            .ToListAsync();

        return View(atrasados);
    }
}
