using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Literacy_LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnIdAndTextbook : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NumberOfCopies",
                table: "IssueRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Textbook",
                table: "IssueRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfCopies",
                table: "IssueRequests");

            migrationBuilder.DropColumn(
                name: "Textbook",
                table: "IssueRequests");
        }
    }
}
