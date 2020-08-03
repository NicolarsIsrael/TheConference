using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoConference.Web.Data.Migrations
{
    public partial class dptINUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeptId",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DeptName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeptId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DeptName",
                table: "AspNetUsers");
        }
    }
}
