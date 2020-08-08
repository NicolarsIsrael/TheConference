using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VideoConference.Web.Data.Migrations
{
    public partial class documentminute1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Document",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DocumentNumber = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    CurrentDepartmentId = table.Column<int>(nullable: true),
                    SubmittedBy = table.Column<string>(nullable: true),
                    RecievedBy = table.Column<string>(nullable: true),
                    DateReceived = table.Column<DateTime>(nullable: false),
                    Remarks = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Document", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Document_Department_CurrentDepartmentId",
                        column: x => x.CurrentDepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentMinute",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateMinuted = table.Column<DateTime>(nullable: false),
                    FromDepartmentId = table.Column<int>(nullable: true),
                    ToDepartmentId = table.Column<int>(nullable: true),
                    SubmittedBy = table.Column<string>(nullable: true),
                    RecievedBy = table.Column<string>(nullable: true),
                    DocumentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentMinute", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentMinute_Document_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Document",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentMinute_Department_FromDepartmentId",
                        column: x => x.FromDepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DocumentMinute_Department_ToDepartmentId",
                        column: x => x.ToDepartmentId,
                        principalTable: "Department",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Document_CurrentDepartmentId",
                table: "Document",
                column: "CurrentDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMinute_DocumentId",
                table: "DocumentMinute",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMinute_FromDepartmentId",
                table: "DocumentMinute",
                column: "FromDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentMinute_ToDepartmentId",
                table: "DocumentMinute",
                column: "ToDepartmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentMinute");

            migrationBuilder.DropTable(
                name: "Document");
        }
    }
}
