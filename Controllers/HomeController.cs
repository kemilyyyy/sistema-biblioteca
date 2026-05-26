using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using sistema_biblioteca.Models;

namespace sistema_biblioteca.Controllers;

public class HomeController : AuthenticatedController
{
    public IActionResult Index()
    {
        var role = HttpContext.Session.GetString("Role");

        if (role == "Atendente")
        {
            return RedirectToAction("Index", "Atendente");
        }

        if (role == "Bibliotecario")
        {
            return RedirectToAction("Index", "Bibliotecario");
        }

        return RedirectToAction("Login", "Auth");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
