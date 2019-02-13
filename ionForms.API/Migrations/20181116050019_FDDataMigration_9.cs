using Microsoft.EntityFrameworkCore.Migrations;

namespace ionForms.API.Migrations
{
    public partial class FDDataMigration_9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Columns_ColumnName",
                table: "Columns");

            migrationBuilder.DropIndex(
                name: "IX_Columns_FormId",
                table: "Columns");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_FormId_ColumnName",
                table: "Columns",
                columns: new[] { "FormId", "ColumnName" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Columns_FormId_ColumnName",
                table: "Columns");

            migrationBuilder.CreateIndex(
                name: "IX_Columns_ColumnName",
                table: "Columns",
                column: "ColumnName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Columns_FormId",
                table: "Columns",
                column: "FormId");
        }
    }
}
