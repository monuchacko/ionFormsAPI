using Microsoft.EntityFrameworkCore.Migrations;

namespace ionForms.API.Migrations
{
    public partial class FDDataMigration_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColumnValue",
                table: "Columns",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColumnValue",
                table: "Columns");
        }
    }
}
