using Microsoft.EntityFrameworkCore.Migrations;

namespace ionForms.API.Migrations
{
    public partial class FDDataMigration_7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DefaultValue",
                table: "Columns",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Columns",
                maxLength: 5000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Columns",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "Columns",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultValue",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Columns");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "Columns");
        }
    }
}
