using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoConference.Web.Data.Migrations
{
    public partial class deptNameInMeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeptID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "DeptName",
                table: "Meeting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeptName",
                table: "Meeting");

            migrationBuilder.AddColumn<int>(
                name: "DeptID",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: 0);
        }
    }
}
