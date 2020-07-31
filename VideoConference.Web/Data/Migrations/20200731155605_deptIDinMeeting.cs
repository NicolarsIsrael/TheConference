using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoConference.Web.Data.Migrations
{
    public partial class deptIDinMeeting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedId",
                table: "Meeting");

            migrationBuilder.AddColumn<int>(
                name: "DeptID",
                table: "Meeting",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeptID",
                table: "Meeting");

            migrationBuilder.AddColumn<string>(
                name: "GeneratedId",
                table: "Meeting",
                nullable: true);
        }
    }
}
