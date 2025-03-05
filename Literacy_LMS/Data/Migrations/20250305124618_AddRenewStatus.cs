using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Literacy_LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRenewStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RenewStatus",
                table: "IssueRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RenewStatus",
                table: "IssueRequests");
        }
    }
}
