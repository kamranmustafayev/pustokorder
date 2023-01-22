using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PustokProj.Migrations
{
    public partial class BookImageAndBookTableUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Queue",
                table: "BookImages");

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsNew",
                table: "Books",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPoster",
                table: "BookImages",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsNew",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "IsPoster",
                table: "BookImages");

            migrationBuilder.AddColumn<int>(
                name: "Queue",
                table: "BookImages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
