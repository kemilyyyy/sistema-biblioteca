using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using sistema_biblioteca.Data;
using sistema_biblioteca.Models;

namespace sistema_biblioteca.Controllers;

public class UsuariosController : AuthenticatedController
{
    private readonly LibraryContext _context;

    public UsuariosController(LibraryContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarios = await _context.Usuarios.AsNoTracking().OrderBy(u => u.Nome).ToListAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nome,Cpf,Email,Telefone")] Usuario usuario)
    {
        var emailAttr = new EmailAddressAttribute();
        var phoneAttr = new PhoneAttribute();

        if (!string.IsNullOrWhiteSpace(usuario.Email) && !emailAttr.IsValid(usuario.Email))
        {
            ModelState.AddModelError(nameof(usuario.Email), "E-mail em formato inválido.");
        }

        if (!string.IsNullOrWhiteSpace(usuario.Telefone) && !phoneAttr.IsValid(usuario.Telefone))
        {
            ModelState.AddModelError(nameof(usuario.Telefone), "Telefone em formato inválido.");
        }

        if (ModelState.IsValid)
        {
            if (!string.IsNullOrWhiteSpace(usuario.Cpf))
            {
                var exists = await _context.Usuarios.AnyAsync(u => u.Cpf == usuario.Cpf);
                if (exists)
                {
                    ModelState.AddModelError("CPF", "Este CPF já está cadastrado.");
                    return View(usuario);
                }
            }

            usuario.Status = string.IsNullOrWhiteSpace(usuario.Status) ? "ATIVO" : usuario.Status;
            usuario.DataCadastro = DateTime.Now;

            _context.Add(usuario);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Usuário cadastrado com sucesso.";
            return RedirectToAction(nameof(Index));
        }

        return View(usuario);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("IdUsuario,Nome,Cpf,Email,Telefone,Status,DataCadastro")] Usuario usuario)
    {
        if (id != usuario.IdUsuario) return NotFound();

        var emailAttr = new EmailAddressAttribute();
        var phoneAttr = new PhoneAttribute();

        if (!string.IsNullOrWhiteSpace(usuario.Email) && !emailAttr.IsValid(usuario.Email))
        {
            ModelState.AddModelError(nameof(usuario.Email), "E-mail em formato inválido.");
        }

        if (!string.IsNullOrWhiteSpace(usuario.Telefone) && !phoneAttr.IsValid(usuario.Telefone))
        {
            ModelState.AddModelError(nameof(usuario.Telefone), "Telefone em formato inválido.");
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(usuario.Cpf))
                {
                    var cpfExists = await _context.Usuarios.AnyAsync(u => u.Cpf == usuario.Cpf && u.IdUsuario != usuario.IdUsuario);
                    if (cpfExists)
                    {
                        ModelState.AddModelError(nameof(usuario.Cpf), "Outro usuário já possui este CPF.");
                        return View(usuario);
                    }
                }

                _context.Update(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuário atualizado com sucesso.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.IdUsuario)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        return View(usuario);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var usuario = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);
        if (usuario == null) return NotFound();
        return View(usuario);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario != null)
        {
            var hasLoans = await _context.EmprestimosSql.AnyAsync(e => e.IdUsuario == id);
            if (hasLoans)
            {
                ModelState.AddModelError(string.Empty, "Não é possível excluir usuário com empréstimos registrados.");
                return View(usuario);
            }

            try
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Usuário excluído.";
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Não é possível excluir este usuário porque ele possui empréstimos vinculados.";
                return RedirectToAction(nameof(Index));
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private bool UsuarioExists(int id)
    {
        return _context.Usuarios.Any(e => e.IdUsuario == id);
    }
}
