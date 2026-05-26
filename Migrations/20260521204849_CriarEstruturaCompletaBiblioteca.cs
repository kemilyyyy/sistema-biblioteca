using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace sistema_biblioteca.Migrations
{
    /// <inheritdoc />
    public partial class CriarEstruturaCompletaBiblioteca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuncionariosSql",
                columns: table => new
                {
                    ID_FUNCIONARIO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NOME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CARGO = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    LOGIN = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    SENHA = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuncionariosSql", x => x.ID_FUNCIONARIO);
                });

            migrationBuilder.CreateTable(
                name: "LivrosSql",
                columns: table => new
                {
                    ID_LIVRO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TITULO = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    AUTOR = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    EDITORA = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ISBN = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ANO = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LivrosSql", x => x.ID_LIVRO);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    ID_USUARIO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NOME = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CPF = table.Column<string>(type: "TEXT", maxLength: 14, nullable: true),
                    EMAIL = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    TELEFONE = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    STATU = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DATA_CADASTRO = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.ID_USUARIO);
                });

            migrationBuilder.CreateTable(
                name: "Exemplares",
                columns: table => new
                {
                    ID_EXEMPLAR = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID_LIVRO = table.Column<int>(type: "INTEGER", nullable: false),
                    CODIGO_EXEMPLAR = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    STATUS = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exemplares", x => x.ID_EXEMPLAR);
                    table.ForeignKey(
                        name: "FK_Exemplares_LivrosSql_ID_LIVRO",
                        column: x => x.ID_LIVRO,
                        principalTable: "LivrosSql",
                        principalColumn: "ID_LIVRO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bloqueios",
                columns: table => new
                {
                    ID_BLOQUEIO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID_USUARIO = table.Column<int>(type: "INTEGER", nullable: false),
                    MOTIVO = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    DATA_INICIO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DATA_FIM = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bloqueios", x => x.ID_BLOQUEIO);
                    table.ForeignKey(
                        name: "FK_Bloqueios_Usuarios_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "Usuarios",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmprestimosSql",
                columns: table => new
                {
                    ID_EMPRESTIMO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID_USUARIO = table.Column<int>(type: "INTEGER", nullable: false),
                    ID_FUNCIONARIO = table.Column<int>(type: "INTEGER", nullable: true),
                    ID_EXEMPLAR = table.Column<int>(type: "INTEGER", nullable: false),
                    DATA_EMPRESTIMO = table.Column<DateTime>(type: "TEXT", nullable: true),
                    DATA_PREVISTA = table.Column<DateTime>(type: "TEXT", nullable: true),
                    STATUS = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmprestimosSql", x => x.ID_EMPRESTIMO);
                    table.ForeignKey(
                        name: "FK_EmprestimosSql_Exemplares_ID_EXEMPLAR",
                        column: x => x.ID_EXEMPLAR,
                        principalTable: "Exemplares",
                        principalColumn: "ID_EXEMPLAR",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmprestimosSql_FuncionariosSql_ID_FUNCIONARIO",
                        column: x => x.ID_FUNCIONARIO,
                        principalTable: "FuncionariosSql",
                        principalColumn: "ID_FUNCIONARIO",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmprestimosSql_Usuarios_ID_USUARIO",
                        column: x => x.ID_USUARIO,
                        principalTable: "Usuarios",
                        principalColumn: "ID_USUARIO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Devolucoes",
                columns: table => new
                {
                    ID_DEVOLUCAO = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID_EMPRESTIMO = table.Column<int>(type: "INTEGER", nullable: false),
                    DATA_DEVOLUCAO = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devolucoes", x => x.ID_DEVOLUCAO);
                    table.ForeignKey(
                        name: "FK_Devolucoes_EmprestimosSql_ID_EMPRESTIMO",
                        column: x => x.ID_EMPRESTIMO,
                        principalTable: "EmprestimosSql",
                        principalColumn: "ID_EMPRESTIMO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Multas",
                columns: table => new
                {
                    ID_MULTA = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ID_EMPRESTIMO = table.Column<int>(type: "INTEGER", nullable: false),
                    VALOR = table.Column<decimal>(type: "TEXT", nullable: false),
                    DIAS_ATRASO = table.Column<int>(type: "INTEGER", nullable: false),
                    STATUS_PAGAMENTO = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Multas", x => x.ID_MULTA);
                    table.ForeignKey(
                        name: "FK_Multas_EmprestimosSql_ID_EMPRESTIMO",
                        column: x => x.ID_EMPRESTIMO,
                        principalTable: "EmprestimosSql",
                        principalColumn: "ID_EMPRESTIMO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bloqueios_ID_USUARIO",
                table: "Bloqueios",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_Devolucoes_ID_EMPRESTIMO",
                table: "Devolucoes",
                column: "ID_EMPRESTIMO");

            migrationBuilder.CreateIndex(
                name: "IX_EmprestimosSql_ID_EXEMPLAR",
                table: "EmprestimosSql",
                column: "ID_EXEMPLAR");

            migrationBuilder.CreateIndex(
                name: "IX_EmprestimosSql_ID_FUNCIONARIO",
                table: "EmprestimosSql",
                column: "ID_FUNCIONARIO");

            migrationBuilder.CreateIndex(
                name: "IX_EmprestimosSql_ID_USUARIO",
                table: "EmprestimosSql",
                column: "ID_USUARIO");

            migrationBuilder.CreateIndex(
                name: "IX_Exemplares_ID_LIVRO",
                table: "Exemplares",
                column: "ID_LIVRO");

            migrationBuilder.CreateIndex(
                name: "IX_Multas_ID_EMPRESTIMO",
                table: "Multas",
                column: "ID_EMPRESTIMO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bloqueios");

            migrationBuilder.DropTable(
                name: "Devolucoes");

            migrationBuilder.DropTable(
                name: "Multas");

            migrationBuilder.DropTable(
                name: "EmprestimosSql");

            migrationBuilder.DropTable(
                name: "Exemplares");

            migrationBuilder.DropTable(
                name: "FuncionariosSql");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "LivrosSql");
        }
    }
}
