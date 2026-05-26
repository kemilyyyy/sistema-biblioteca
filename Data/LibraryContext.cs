using Microsoft.EntityFrameworkCore;
using sistema_biblioteca.Models;

namespace sistema_biblioteca.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options)
        : base(options)
    {
    }

    // When true, validations in SaveChanges are skipped (used for data seeding)
    public bool SkipValidation { get; set; } = false;

    public DbSet<Livro> Livros => Set<Livro>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Emprestimo> Emprestimos => Set<Emprestimo>();
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();

    // SQL-mapped entities (legacy schema)
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<FuncionarioSimple> FuncionariosSql => Set<FuncionarioSimple>();
    public DbSet<LivroSimple> LivrosSql => Set<LivroSimple>();
    public DbSet<ExemplarSimple> Exemplares => Set<ExemplarSimple>();
    public DbSet<EmprestimoSimple> EmprestimosSql => Set<EmprestimoSimple>();
    public DbSet<DevolucaoSimple> Devolucoes => Set<DevolucaoSimple>();
    public DbSet<MultaSimple> Multas => Set<MultaSimple>();
    public DbSet<BloqueioSimple> Bloqueios => Set<BloqueioSimple>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>()
            .HasIndex(c => c.Matricula)
            .IsUnique();

        modelBuilder.Entity<Livro>()
            .HasIndex(l => new { l.Titulo, l.Autor })
            .IsUnique();

        modelBuilder.Entity<Livro>()
            .HasIndex(l => l.ISBN)
            .IsUnique();

        modelBuilder.Entity<Emprestimo>()
            .HasOne(e => e.Cliente)
            .WithMany(c => c.Emprestimos)
            .HasForeignKey(e => e.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Emprestimo>()
            .HasOne(e => e.Livro)
            .WithMany(l => l.Emprestimos)
            .HasForeignKey(e => e.LivroId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Emprestimo>()
            .HasOne(e => e.Funcionario)
            .WithMany(f => f.Emprestimos)
            .HasForeignKey(e => e.FuncionarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Legacy schema relationships
        modelBuilder.Entity<ExemplarSimple>()
            .HasOne(e => e.Livro)
            .WithMany(l => l.Exemplares)
            .HasForeignKey(e => e.IdLivro)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmprestimoSimple>()
            .HasOne(e => e.Usuario)
            .WithMany(u => u.Emprestimos)
            .HasForeignKey(e => e.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmprestimoSimple>()
            .HasOne(e => e.Funcionario)
            .WithMany(f => f.Emprestimos)
            .HasForeignKey(e => e.IdFuncionario)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmprestimoSimple>()
            .HasOne(e => e.Exemplar)
            .WithMany(x => x.Emprestimos)
            .HasForeignKey(e => e.IdExemplar)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DevolucaoSimple>()
            .HasOne(d => d.Emprestimo)
            .WithMany(e => e.Devolucoes)
            .HasForeignKey(d => d.IdEmprestimo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MultaSimple>()
            .HasOne(m => m.Emprestimo)
            .WithMany(e => e.Multas)
            .HasForeignKey(m => m.IdEmprestimo)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BloqueioSimple>()
            .HasOne(b => b.Usuario)
            .WithMany(u => u.Bloqueios)
            .HasForeignKey(b => b.IdUsuario)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public override int SaveChanges()
    {
        if (!SkipValidation)
        {
            ValidateEmprestimos();
        }
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!SkipValidation)
        {
            ValidateEmprestimos();
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ValidateEmprestimos()
    {
        var pendingLoans = ChangeTracker.Entries<Emprestimo>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity)
            .ToList();

        if (!pendingLoans.Any())
        {
            return;
        }

        foreach (var loan in pendingLoans)
        {
            if (loan.DataPrevisaoDevolucao <= loan.DataRetirada)
            {
                throw new InvalidOperationException("A data de devolução deve ser posterior à data de empréstimo.");
            }

            var activeLoans = Emprestimos
                .Where(e => e.ClienteId == loan.ClienteId && e.DataDevolucao == null)
                .AsNoTracking()
                .ToList();

            if (loan.DataDevolucao == null && !activeLoans.Any(e => e.Id == loan.Id))
            {
                activeLoans.Add(loan);
            }

            // Verifica se o cliente está bloqueado explicitamente
            var cliente = Clientes.AsNoTracking().FirstOrDefault(c => c.Id == loan.ClienteId);
            if (cliente != null && cliente.Bloqueado && loan.DataDevolucao == null)
            {
                throw new InvalidOperationException("O cliente está bloqueado e não pode realizar novos empréstimos.");
            }

            // Verifica disponibilidade do livro no estoque
            var livro = Livros.AsNoTracking().FirstOrDefault(l => l.Id == loan.LivroId);
            if (livro == null)
            {
                throw new InvalidOperationException("Livro não encontrado.");
            }

            if (!livro.Ativo)
            {
                throw new InvalidOperationException("O livro está inativo e não pode ser emprestado.");
            }

            // Conta empréstimos ativos para este livro já no banco
            var existingActiveForBook = Emprestimos
                .Where(e => e.LivroId == loan.LivroId && e.DataDevolucao == null)
                .AsNoTracking()
                .Count();

            // Conta quantos novos empréstimos deste SaveChanges estão sendo adicionados para o mesmo livro
            var pendingNewForBook = ChangeTracker.Entries<Emprestimo>()
                .Where(e => e.State == EntityState.Added && e.Entity.LivroId == loan.LivroId && e.Entity.DataDevolucao == null)
                .Count();

            if (existingActiveForBook + pendingNewForBook > livro.QtdEstoque)
            {
                throw new InvalidOperationException("Não há cópias disponíveis deste livro no momento.");
            }

            if (activeLoans.Count > 5)
            {
                throw new InvalidOperationException("O cliente não pode ter mais de 5 livros emprestados ao mesmo tempo.");
            }

            if (activeLoans.Any(e => e.DataPrevisaoDevolucao < DateTime.UtcNow.Date))
            {
                throw new InvalidOperationException("O cliente está bloqueado por atraso e não pode pegar novos livros até devolver os empréstimos atrasados.");
            }

            if (activeLoans.Count(e => e.LivroId == loan.LivroId) > 1)
            {
                throw new InvalidOperationException("Não é permitido emprestar o mesmo livro repetidamente para o mesmo cliente antes da devolução.");
            }
        }
    }
}
