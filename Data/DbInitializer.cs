using sistema_biblioteca.Models;
using Microsoft.EntityFrameworkCore;

namespace sistema_biblioteca.Data;

public static class DbInitializer
{
    public static void Initialize(LibraryContext context)
    {
        // Apply migrations (if any) and ensure database exists
        context.Database.Migrate();

        // If there is existing data, do not seed
        if (context.Clientes.Any() || context.Livros.Any() || context.Funcionarios.Any())
        {
            return;
        }

        // Seed Livros
        var livros = new List<Livro>
        {
            new Livro { Titulo = "Introdução à Programação", Autor = "Ana Souza", Genero = "Computação", ISBN = "9780001", QtdEstoque = 3, Ativo = true, DataPublicacao = new DateTime(2018,5,1) },
            new Livro { Titulo = "Algoritmos e Estruturas de Dados", Autor = "Carlos Pereira", Genero = "Computação", ISBN = "9780002", QtdEstoque = 2, Ativo = true, DataPublicacao = new DateTime(2016,3,10) },
            new Livro { Titulo = "História do Brasil", Autor = "Mariana Lima", Genero = "História", ISBN = "9780003", QtdEstoque = 1, Ativo = true, DataPublicacao = new DateTime(2010,7,20) },
            new Livro { Titulo = "Matemática Básica", Autor = "João Silva", Genero = "Matemática", ISBN = "9780004", QtdEstoque = 4, Ativo = true, DataPublicacao = new DateTime(2012,1,5) },
            new Livro { Titulo = "Design de Interfaces", Autor = "Clara Mendes", Genero = "Design", ISBN = "9780005", QtdEstoque = 2, Ativo = true, DataPublicacao = new DateTime(2019,11,11) },
            new Livro { Titulo = "Administração Pública", Autor = "Rui Oliveira", Genero = "Administração", ISBN = "9780006", QtdEstoque = 1, Ativo = true, DataPublicacao = new DateTime(2015,2,2) }
        };

        // Temporarily skip context validations during seeding
        context.SkipValidation = true;

        context.Livros.AddRange(livros);
        context.SaveChanges();

        // Seed Clientes
        var clientes = new List<Cliente>
        {
            new Cliente { Nome = "Lucas Almeida", Matricula = "2021001", CPF = "11122233344", Email = "lucas.almeida@example.com", Telefone = "(11) 91234-5678", Logradouro = "Rua A, 100" },
            new Cliente { Nome = "Marina Costa", Matricula = "2021002", CPF = "22233344455", Email = "marina.costa@example.com", Telefone = "(21) 99876-5432", Logradouro = "Av. B, 200" },
            new Cliente { Nome = "Pedro Santos", Matricula = "2021003", CPF = "33344455566", Email = "pedro.santos@example.com", Telefone = "(31) 97777-7777", Logradouro = "Praça C, 50" },
            new Cliente { Nome = "Ana Pereira", Matricula = "2021004", CPF = "44455566677", Email = "ana.pereira@example.com", Telefone = "(41) 96666-6666", Logradouro = "Trav. D, 12" },
            new Cliente { Nome = "Bruno Rocha", Matricula = "2021005", CPF = "55566677788", Email = "bruno.rocha@example.com", Telefone = "(51) 95555-5555", Logradouro = "Rua E, 89" }
        };

        context.Clientes.AddRange(clientes);
        context.SaveChanges();

        // Seed Funcionarios
        var funcionarios = new List<Funcionario>
        {
            new Funcionario { Nome = "Beatriz Gasparini", Cargo = "Atendente", Username = "beatriz", Role = "Atendente", Email = "beatriz@example.com" },
            new Funcionario { Nome = "Enzo Lima", Cargo = "Bibliotecário", Username = "enzo", Role = "Bibliotecario", Email = "enzo@example.com" }
        };

        context.Funcionarios.AddRange(funcionarios);
        context.SaveChanges();

        // Seed Emprestimos (some active, one overdue, one returned)
        var hoje = DateTime.UtcNow.Date;

        var emprestimos = new List<Emprestimo>
        {
            // active loan for Lucas
            new Emprestimo { ClienteId = clientes[0].Id, LivroId = livros[0].Id, FuncionarioId = funcionarios[0].Id, DataRetirada = hoje.AddDays(-2), DataPrevisaoDevolucao = hoje.AddDays(8) },

            // overdue loan for Marina
            new Emprestimo { ClienteId = clientes[1].Id, LivroId = livros[2].Id, FuncionarioId = funcionarios[0].Id, DataRetirada = hoje.AddDays(-15), DataPrevisaoDevolucao = hoje.AddDays(-5) },

            // returned loan for Pedro
            new Emprestimo { ClienteId = clientes[2].Id, LivroId = livros[1].Id, FuncionarioId = funcionarios[1].Id, DataRetirada = hoje.AddDays(-20), DataPrevisaoDevolucao = hoje.AddDays(-10), DataDevolucao = hoje.AddDays(-9) },

            // multiple active for Bruno
            new Emprestimo { ClienteId = clientes[4].Id, LivroId = livros[3].Id, FuncionarioId = funcionarios[1].Id, DataRetirada = hoje.AddDays(-1), DataPrevisaoDevolucao = hoje.AddDays(9) },
            new Emprestimo { ClienteId = clientes[4].Id, LivroId = livros[4].Id, FuncionarioId = funcionarios[1].Id, DataRetirada = hoje.AddDays(-3), DataPrevisaoDevolucao = hoje.AddDays(7) }
        };

        context.Emprestimos.AddRange(emprestimos);
        context.SaveChanges();

        // Restore validation
        context.SkipValidation = false;
    }
}
