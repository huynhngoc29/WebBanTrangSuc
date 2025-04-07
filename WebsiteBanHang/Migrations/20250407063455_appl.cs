using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebBanTrangSuc.Migrations
{
    /// <inheritdoc />
    public partial class appl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategory_Categories_CategoryId",
                table: "CategorySubCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategory_SubCategories_SubCategoryId",
                table: "CategorySubCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategory",
                table: "CategorySubCategory");

            migrationBuilder.RenameTable(
                name: "CategorySubCategory",
                newName: "CategorySubCategories");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubCategory_SubCategoryId",
                table: "CategorySubCategories",
                newName: "IX_CategorySubCategories_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubCategory_CategoryId",
                table: "CategorySubCategories",
                newName: "IX_CategorySubCategories_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategories",
                table: "CategorySubCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategories_Categories_CategoryId",
                table: "CategorySubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategories_SubCategories_SubCategoryId",
                table: "CategorySubCategories",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategories_Categories_CategoryId",
                table: "CategorySubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CategorySubCategories_SubCategories_SubCategoryId",
                table: "CategorySubCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CategorySubCategories",
                table: "CategorySubCategories");

            migrationBuilder.RenameTable(
                name: "CategorySubCategories",
                newName: "CategorySubCategory");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubCategories_SubCategoryId",
                table: "CategorySubCategory",
                newName: "IX_CategorySubCategory_SubCategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_CategorySubCategories_CategoryId",
                table: "CategorySubCategory",
                newName: "IX_CategorySubCategory_CategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CategorySubCategory",
                table: "CategorySubCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategory_Categories_CategoryId",
                table: "CategorySubCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CategorySubCategory_SubCategories_SubCategoryId",
                table: "CategorySubCategory",
                column: "SubCategoryId",
                principalTable: "SubCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
