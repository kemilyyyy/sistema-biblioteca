using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistema_biblioteca.Data;
using sistema_biblioteca.Models;

namespace sistema_biblioteca.Controllers;

public class LivrosController : AuthenticatedController
{
    private readonly LibraryContext _context;

    public LivrosController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var livros = await _context.Livros.AsNoTracking().OrderBy(l => l.Titulo).ToListAsync();
        return View(livros);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var livro = await _context.Livros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
        if (livro == null) return NotFound();
        return View(livro);
    }

    public IActionResult Create()
    {
        if (!PodeCadastrarLivro())
        {
            TempData["Error"] = "Somente o bibliotecário pode cadastrar livros.";
            return RedirectToAction(nameof(Index));
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Titulo,Autor,ISBN,DataPublicacao,Genero,QtdEstoque,Ativo")] Livro livro)
    {
        if (!PodeCadastrarLivro())
        {
            TempData["Error"] = "Somente o bibliotecário pode cadastrar livros.";
            return RedirectToAction(nameof(Index));
        }

        if (ModelState.IsValid)
        {
            // checar duplicidade por ISBN quando informado
            if (!string.IsNullOrWhiteSpace(livro.ISBN))
            {
                var existsIsbn = await _context.Livros.AnyAsync(l => l.ISBN == livro.ISBN);
                if (existsIsbn)
                {
                    ModelState.AddModelError(nameof(livro.ISBN), "Já existe um livro com esse ISBN.");
                    return View(livro);
                }
            }

            // checar duplicidade por título+autor
            var exists = await _context.Livros.AnyAsync(l => l.Titulo == livro.Titulo && l.Autor == livro.Autor);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Já existe um livro cadastrado com mesmo título e autor.");
                return View(livro);
            }

            _context.Add(livro);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(livro);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var livro = await _context.Livros.FindAsync(id);
        if (livro == null) return NotFound();
        return View(livro);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Titulo,Autor,ISBN,DataPublicacao,Genero,QtdEstoque,Ativo")] Livro livro)
    {
        if (id != livro.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                // verificar duplicidade ISBN (outro id)
                if (!string.IsNullOrWhiteSpace(livro.ISBN))
                {
                    var isbnExists = await _context.Livros.AnyAsync(l => l.ISBN == livro.ISBN && l.Id != livro.Id);
                    if (isbnExists)
                    {
                        ModelState.AddModelError(nameof(livro.ISBN), "Outro livro já usa este ISBN.");
                        return View(livro);
                    }
                }

                var titleAuthorExists = await _context.Livros.AnyAsync(l => l.Titulo == livro.Titulo && l.Autor == livro.Autor && l.Id != livro.Id);
                if (titleAuthorExists)
                {
                    ModelState.AddModelError(string.Empty, "Outro livro já possui mesmo título e autor.");
                    return View(livro);
                }

                _context.Update(livro);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LivroExists(livro.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(livro);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var livro = await _context.Livros.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
        if (livro == null) return NotFound();
        return View(livro);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var livro = await _context.Livros.FindAsync(id);
        if (livro != null)
        {
            var hasActiveLoans = await _context.Emprestimos.AnyAsync(e => e.LivroId == id && e.DataDevolucao == null);
            if (hasActiveLoans)
            {
                ModelState.AddModelError(string.Empty, "Não é possível excluir o livro enquanto existirem empréstimos ativos.");
                return View(livro);
            }

            try
            {
                _context.Livros.Remove(livro);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Livro excluído.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Não é possível excluir este livro porque ele possui empréstimos vinculados ao histórico.";
                return RedirectToAction(nameof(Index));
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private bool LivroExists(int id)
    {
        return _context.Livros.Any(e => e.Id == id);
    }

    private bool PodeCadastrarLivro()
    {
        return HttpContext.Session.GetString("Role") == "Bibliotecario";
    }
}
