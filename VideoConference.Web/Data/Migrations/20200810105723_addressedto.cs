using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoConference.Web.Data.Migrations
{
    public partial class addressedto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressedTo",
                table: "DocumentMinute",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AddressedTo",
                table: "Document",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Document",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressedTo",
                table: "DocumentMinute");

            migrationBuilder.DropColumn(
                name: "AddressedTo",
                table: "Document");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Document");
        }
    }
}
