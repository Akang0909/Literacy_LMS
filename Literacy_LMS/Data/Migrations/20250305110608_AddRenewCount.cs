using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Literacy_LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRenewCount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RenewCount",
                table: "IssueRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewCount",
                table: "IssueRequests");
        }
    }
}
