using Microsoft.EntityFrameworkCore.Migrations;

namespace SerialNumber.EntityFrameworkCore.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SerialNumber",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Number = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SerialNumber", x => x.Name);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SerialNumber");
        }
    }
}
