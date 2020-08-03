using Microsoft.EntityFrameworkCore.Migrations;

namespace CapacityManagementAPI.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Allocatio__Emplo__15502E78",
                table: "Allocation");

            migrationBuilder.DropForeignKey(
                name: "FK__Allocatio__Proje__145C0A3F",
                table: "Allocation");

            migrationBuilder.AddForeignKey(
                name: "FK__Allocatio__Emplo__15502E78",
                table: "Allocation",
                column: "EmployeeID",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__Allocatio__Proje__145C0A3F",
                table: "Allocation",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Allocatio__Emplo__15502E78",
                table: "Allocation");

            migrationBuilder.DropForeignKey(
                name: "FK__Allocatio__Proje__145C0A3F",
                table: "Allocation");

            migrationBuilder.AddForeignKey(
                name: "FK__Allocatio__Emplo__15502E78",
                table: "Allocation",
                column: "EmployeeID",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK__Allocatio__Proje__145C0A3F",
                table: "Allocation",
                column: "ProjectID",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
