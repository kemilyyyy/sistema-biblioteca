using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sistema_biblioteca.Data;
using sistema_biblioteca.Models;

namespace sistema_biblioteca.Controllers;

public class EmprestimosController : AuthenticatedController
{
    private readonly LibraryContext _context;

    public EmprestimosController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var emprestimos = await _context.Emprestimos
            .AsNoTracking()
            .Include(e => e.Cliente)
            .Include(e => e.Livro)
            .Include(e => e.Funcionario)
            .OrderByDescending(e => e.DataRetirada)
            .ToListAsync();

        return View(emprestimos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var emprestimo = await _context.Emprestimos
            .AsNoTracking()
            .Include(e => e.Cliente)
            .Include(e => e.Livro)
            .Include(e => e.Funcionario)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (emprestimo == null) return NotFound();
        return View(emprestimo);
    }

    public async Task<IActionResult> Create()
    {
        await PopulateSelectLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ClienteId,LivroId,FuncionarioId")] Emprestimo emprestimo)
    {
        // set dates
        emprestimo.DataRetirada = DateTime.UtcNow.Date;
        emprestimo.DataPrevisaoDevolucao = emprestimo.DataRetirada.AddDays(10);

        // basic server-side checks for UX
        var cliente = await _context.Clientes.FindAsync(emprestimo.ClienteId);
        if (cliente == null)
        {
            ModelState.AddModelError(nameof(emprestimo.ClienteId), "Cliente inválido.");
        }

        var livro = await _context.Livros.FindAsync(emprestimo.LivroId);
        if (livro == null || !livro.Ativo)
        {
            ModelState.AddModelError(nameof(emprestimo.LivroId), "Livro indisponível.");
        }

        if (cliente != null && cliente.Bloqueado)
        {
            ModelState.AddModelError(string.Empty, "O cliente está bloqueado e não pode realizar empréstimos.");
        }

        // check max 5 active loans for cliente
        if (cliente != null)
        {
            var activeCount = await _context.Emprestimos.CountAsync(e => e.ClienteId == cliente.Id && e.DataDevolucao == null);
            if (activeCount >= 5)
            {
                ModelState.AddModelError(string.Empty, "O cliente já possui 5 empréstimos ativos.");
            }
        }

        // check livro availability
        if (livro != null)
        {
            var activeForBook = await _context.Emprestimos.CountAsync(e => e.LivroId == livro.Id && e.DataDevolucao == null);
            if (activeForBook >= livro.QtdEstoque)
            {
                ModelState.AddModelError(string.Empty, "Não há cópias disponíveis deste livro.");
            }
        }

        if (!ModelState.IsValid)
        {
            await PopulateSelectLists();
            return View(emprestimo);
        }

        try
        {
            _context.Emprestimos.Add(emprestimo);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Empréstimo registrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateSelectLists();
            return View(emprestimo);
        }
    }

    public async Task<IActionResult> Devolver(int? id)
    {
        if (id == null) return NotFound();
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (emprestimo == null) return NotFound();
        return View(emprestimo);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DevolverConfirmado(int id)
    {
        var emprestimo = await _context.Emprestimos
            .Include(e => e.Cliente)
            .Include(e => e.Livro)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (emprestimo == null) return NotFound();

        if (emprestimo.DataDevolucao != null)
        {
            TempData["Info"] = "Empréstimo já foi devolvido.";
            return RedirectToAction(nameof(Index));
        }

        emprestimo.DataDevolucao = DateTime.UtcNow.Date;

        // marcar bloqueio se houve atraso
        if (emprestimo.DataDevolucao > emprestimo.DataPrevisaoDevolucao)
        {
            emprestimo.Cliente!.Bloqueado = true;
        }

        await _context.SaveChangesAsync();
        TempData["Success"] = "Devolução registrada.";
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateSelectLists()
    {
        var clientes = await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        ViewBag.Clientes = new SelectList(clientes, "Id", "Nome");

        // livros que têm estoque disponível (estoque > ativo loans)
        var livros = await _context.Livros.AsNoTracking().Where(l => l.Ativo).ToListAsync();
        var livrosDisponiveis = livros.Where(l =>
        {
            var active = _context.Emprestimos.Count(e => e.LivroId == l.Id && e.DataDevolucao == null);
            return active < l.QtdEstoque;
        }).ToList();

        ViewBag.Livros = new SelectList(livrosDisponiveis, "Id", "Titulo");

        var funcionarios = await _context.Funcionarios.AsNoTracking().OrderBy(f => f.Nome).ToListAsync();
        ViewBag.Funcionarios = new SelectList(funcionarios, "Id", "Nome");
    }
}
