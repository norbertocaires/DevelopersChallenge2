using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Nibo.Migrations
{
    public partial class transactions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Import",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Date = table.Column<DateTime>(maxLength: 6, nullable: false),
                    FileImported = table.Column<string>(nullable: false),
                    FileDuplicate = table.Column<string>(nullable: false),
                    TotalTransactions = table.Column<int>(nullable: false),
                    TotalTransactionsDuplicates = table.Column<int>(nullable: false),
                    TotalTransactionsSaves = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Import", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    Hash = table.Column<string>(nullable: false),
                    Type = table.Column<string>(maxLength: 6, nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Memo = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.Hash);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Import");

            migrationBuilder.DropTable(
                name: "Transaction");
        }
    }
}
