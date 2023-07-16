using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DotnetStarter.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class CreateHelloWorldDummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Hello",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Message = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Revision = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hello", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hello_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Hello",
                columns: new[] { "Id", "Created", "Message", "Name", "Revision", "Updated", "UserId" },
                values: new object[,]
                {
                    { new Guid("68bbcab1-4aec-4117-8ed9-1101f4768825"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hello Someone!", "Someone", new Guid("35f9f62b-1d5e-4a37-87b9-b3372771425e"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { new Guid("7ffdf241-7645-42e3-a55c-45f924401534"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hello Kristo!", "Kristo", new Guid("c5800c39-9046-4b5a-a091-67c28f8b8ade"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("7bc36eff-a304-4a0d-970e-1b32606e1bb3") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Hello_UserId",
                table: "Hello",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Hello");
        }
    }
}
