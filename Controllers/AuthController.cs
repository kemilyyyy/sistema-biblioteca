using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sistema_biblioteca.ViewModels;

namespace sistema_biblioteca.Controllers;

public class AuthController : Controller
{
    private const string EmailAtendente = "atendente@biblioteca.com";
    private const string EmailBibliotecario = "bibliotecario@biblioteca.com";
    private const string SenhaAtendente = "atendente123";
    private const string SenhaBibliotecario = "bibliotecario123";

    public IActionResult Login()
    {
        if (HttpContext.Session.GetString("Role") == "Atendente")
        {
            return RedirectToAction("Index", "Atendente");
        }

        if (HttpContext.Session.GetString("Role") == "Bibliotecario")
        {
            return RedirectToAction("Index", "Bibliotecario");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var authResult = ValidateCredentials(model.Email, model.Senha);
        if (authResult.Type == ResultType.UserNotFound)
        {
            ModelState.AddModelError(string.Empty, "Usuário não cadastrado");
            return View(model);
        }

        if (authResult.Type == ResultType.WrongPassword)
        {
            ModelState.AddModelError(string.Empty, "Senha incorreta");
            return View(model);
        }

        HttpContext.Session.SetString("Role", authResult.Role ?? string.Empty);
        HttpContext.Session.SetString("UsuarioEmail", model.Email);

        TempData["Success"] = authResult.Role == "Bibliotecario"
            ? "Bem-vindo, bibliotecário!"
            : "Bem-vindo, atendente!";

        return RedirectToAction(authResult.Role == "Bibliotecario" ? "Index" : "Index", authResult.Role == "Bibliotecario" ? "Bibliotecario" : "Atendente");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["Info"] = "Você saiu do sistema.";
        return RedirectToAction(nameof(Login));
    }

    private static AuthResult ValidateCredentials(string email, string senha)
    {
        if (string.Equals(email, EmailAtendente, StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(senha, SenhaAtendente, StringComparison.Ordinal))
            {
                return AuthResult.Success("Atendente");
            }

            return AuthResult.WrongPassword();
        }

        if (string.Equals(email, EmailBibliotecario, StringComparison.OrdinalIgnoreCase))
        {
            if (string.Equals(senha, SenhaBibliotecario, StringComparison.Ordinal))
            {
                return AuthResult.Success("Bibliotecario");
            }

            return AuthResult.WrongPassword();
        }

        return AuthResult.UserNotFound();
    }

    private record AuthResult(string? Role, ResultType Type)
    {
        public static AuthResult Success(string role) => new(role, ResultType.Success);
        public static AuthResult WrongPassword() => new(null, ResultType.WrongPassword);
        public static AuthResult UserNotFound() => new(null, ResultType.UserNotFound);
    }

    private enum ResultType
    {
        Success,
        WrongPassword,
        UserNotFound
    }
}
