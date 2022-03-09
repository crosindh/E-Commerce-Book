using Microsoft.EntityFrameworkCore.Migrations;

namespace E_Book.DataAccess.Migrations
{
    public partial class ColumnChangedinShoppingCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_ApplicationId",
                table: "shoppingCarts");

            migrationBuilder.RenameColumn(
                name: "ApplicationId",
                table: "shoppingCarts",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_shoppingCarts_ApplicationId",
                table: "shoppingCarts",
                newName: "IX_shoppingCarts_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_ApplicationUserId",
                table: "shoppingCarts",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_ApplicationUserId",
                table: "shoppingCarts");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "shoppingCarts",
                newName: "ApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_shoppingCarts_ApplicationUserId",
                table: "shoppingCarts",
                newName: "IX_shoppingCarts_ApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_shoppingCarts_AspNetUsers_ApplicationId",
                table: "shoppingCarts",
                column: "ApplicationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
