using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoConference.Web.Data.Migrations
{
    public partial class meetingtype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExecMeeting",
                table: "Meeting");

            migrationBuilder.AddColumn<int>(
                name: "MeetingType",
                table: "Meeting",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingType",
                table: "Meeting");

            migrationBuilder.AddColumn<bool>(
                name: "IsExecMeeting",
                table: "Meeting",
                nullable: false,
                defaultValue: false);
        }
    }
}
