using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace App.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPorta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "porta",
                table: "site_deploy_config",
                type: "INTEGER",
                nullable: false,
                defaultValue: 80);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "porta",
                table: "site_deploy_config");
        }
    }
}
