using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace newproj.Migrations
{
    /// <inheritdoc />
    public partial class Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RequestHistory_RequestId",
                table: "RequestHistory",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_RequestHistory_Requests_RequestId",
                table: "RequestHistory",
                column: "RequestId",
                principalTable: "Requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RequestHistory_Requests_RequestId",
                table: "RequestHistory");

            migrationBuilder.DropIndex(
                name: "IX_RequestHistory_RequestId",
                table: "RequestHistory");
        }
    }
}
