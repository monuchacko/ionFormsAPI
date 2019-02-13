using Microsoft.EntityFrameworkCore.Migrations;

namespace ionForms.API.Migrations
{
    public partial class FDDataMigration_8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Columns_ColumnName",
                table: "Columns",
                column: "ColumnName",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Columns_ColumnName",
                table: "Columns");
        }
    }
}
