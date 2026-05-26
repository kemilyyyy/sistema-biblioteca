using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistema_biblioteca.Data;
using sistema_biblioteca.Models;

namespace sistema_biblioteca.Controllers;

public class FuncionariosController : AuthenticatedController
{
    private readonly LibraryContext _context;

    public FuncionariosController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var funcionarios = await _context.Funcionarios.AsNoTracking().OrderBy(f => f.Nome).ToListAsync();
        return View(funcionarios);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var funcionario = await _context.Funcionarios.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (funcionario == null) return NotFound();
        return View(funcionario);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Cargo,Email,Username,Role,CPF") ] Funcionario funcionario)
    {
        if (ModelState.IsValid)
        {
            // Username único
            var usernameExists = await _context.Funcionarios.AnyAsync(f => f.Username == funcionario.Username);
            if (usernameExists)
            {
                ModelState.AddModelError(nameof(funcionario.Username), "Este nome de usuário já está em uso.");
                return View(funcionario);
            }

            _context.Add(funcionario);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Funcionário cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        return View(funcionario);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario == null) return NotFound();
        return View(funcionario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Cargo,Email,Username,Role,CPF,PasswordHash")] Funcionario funcionario)
    {
        if (id != funcionario.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var usernameExists = await _context.Funcionarios.AnyAsync(f => f.Username == funcionario.Username && f.Id != funcionario.Id);
                if (usernameExists)
                {
                    ModelState.AddModelError(nameof(funcionario.Username), "Outro funcionário já usa este nome de usuário.");
                    return View(funcionario);
                }

                _context.Update(funcionario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Funcionário atualizado com sucesso.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FuncionarioExists(funcionario.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(funcionario);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var funcionario = await _context.Funcionarios.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
        if (funcionario == null) return NotFound();
        return View(funcionario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var funcionario = await _context.Funcionarios.FindAsync(id);
        if (funcionario != null)
        {
            // impedir exclusão se houver empréstimos vinculados
            var hasLoans = await _context.Emprestimos.AnyAsync(e => e.FuncionarioId == id);
            if (hasLoans)
            {
                ModelState.AddModelError(string.Empty, "Não é possível excluir funcionário com registros de empréstimos. Inative o registro em vez de excluir.");
                return View(funcionario);
            }

            _context.Funcionarios.Remove(funcionario);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Funcionário excluído.";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool FuncionarioExists(int id)
    {
        return _context.Funcionarios.Any(e => e.Id == id);
    }
}
