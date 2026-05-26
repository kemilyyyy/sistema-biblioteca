using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sistema_biblioteca.Data;
using sistema_biblioteca.Models;

namespace sistema_biblioteca.Controllers;

public class ClientesController : AuthenticatedController
{
    private readonly LibraryContext _context;

    public ClientesController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var clientes = await _context.Clientes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync();
        return View(clientes);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _context.Clientes
            .AsNoTracking()
            .Include(c => c.Emprestimos)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Matricula,CPF,Email,Telefone,Logradouro") ] Cliente cliente)
    {
        if (ModelState.IsValid)
        {
            // matrícula única
            var exists = await _context.Clientes.AnyAsync(c => c.Matricula == cliente.Matricula);
            if (exists)
            {
                ModelState.AddModelError(nameof(cliente.Matricula), "Já existe um cliente cadastrado com essa matrícula.");
                return View(cliente);
            }

            _context.Add(cliente);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cliente cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Matricula,CPF,Email,Telefone,Logradouro,Bloqueado")] Cliente cliente)
    {
        if (id != cliente.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                var matriculaExists = await _context.Clientes.AnyAsync(c => c.Matricula == cliente.Matricula && c.Id != cliente.Id);
                if (matriculaExists)
                {
                    ModelState.AddModelError(nameof(cliente.Matricula), "Outra matrícula idêntica já existe.");
                    return View(cliente);
                }

                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente atualizado com sucesso.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClienteExists(cliente.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(cliente);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        if (cliente == null) return NotFound();
        return View(cliente);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var cliente = await _context.Clientes.FindAsync(id);
        if (cliente != null)
        {
            var hasActive = await _context.Emprestimos.AnyAsync(e => e.ClienteId == id && e.DataDevolucao == null);
            if (hasActive)
            {
                ModelState.AddModelError(string.Empty, "Não é possível excluir cliente com empréstimos ativos. Resolva as devoluções primeiro.");
                return View(cliente);
            }

            try
            {
                cliente.Bloqueado = true;
                _context.Update(cliente);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cliente bloqueado.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Não é possível bloquear este cliente porque ele possui empréstimos vinculados.";
                return RedirectToAction(nameof(Index));
            }
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ClienteExists(int id)
    {
        return _context.Clientes.Any(e => e.Id == id);
    }
}
