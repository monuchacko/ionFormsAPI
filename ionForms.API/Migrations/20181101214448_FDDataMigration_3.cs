using Microsoft.EntityFrameworkCore.Migrations;

namespace ionForms.API.Migrations
{
    public partial class FDDataMigration_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColumnType",
                table: "Columns",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnType",
                table: "Columns");
        }
    }
}
