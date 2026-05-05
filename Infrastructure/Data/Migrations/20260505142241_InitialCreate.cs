using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "site_deploy_config",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    projeto_iis = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    svn = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    destino = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    atualizada = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_deploy_config", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "site_origem_config",
                columns: table => new
                {
                    id = table.Column<string>(type: "TEXT", nullable: false),
                    site_deploy_config_id = table.Column<string>(type: "TEXT", nullable: false),
                    path = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    conteudo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_site_origem_config", x => x.id);
                    table.ForeignKey(
                        name: "FK_site_origem_config_site_deploy_config_site_deploy_config_id",
                        column: x => x.site_deploy_config_id,
                        principalTable: "site_deploy_config",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_site_origem_config_site_deploy_config_id",
                table: "site_origem_config",
                column: "site_deploy_config_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "site_origem_config");

            migrationBuilder.DropTable(
                name: "site_deploy_config");
        }
    }
}
