using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMS.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCodeToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentCode",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$MmGKFYHinEBH448AL3Tisu20ww4OyeMw5rI5G091jXiszo9kFWJYi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentCode",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$Re7IyxNN5uNSIc8Rs1Dr5.WadffWPrfifLE5jJsvaSNzE655T7qBG");
        }
    }
}
