using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Literacy_LMS.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddReturnAndOverDuePayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OverdueAmount",
                table: "IssueRequests",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentStatus",
                table: "IssueRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReturnDate",
                table: "IssueRequests",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OverdueAmount",
                table: "IssueRequests");

            migrationBuilder.DropColumn(
                name: "PaymentStatus",
                table: "IssueRequests");

            migrationBuilder.DropColumn(
                name: "ReturnDate",
                table: "IssueRequests");
        }
    }
}
