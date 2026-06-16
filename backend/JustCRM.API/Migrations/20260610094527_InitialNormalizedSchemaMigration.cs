using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JustCRM.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialNormalizedSchemaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CUSTOMERS",
                columns: table => new
                {
                    CustomerId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMERS", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "INDUSTRIES",
                columns: table => new
                {
                    IndustryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IndustryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_INDUSTRIES", x => x.IndustryId);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER_CONTACTS",
                columns: table => new
                {
                    ContactId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    PrimaryPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecondaryPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER_CONTACTS", x => x.ContactId);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_CONTACTS_CUSTOMERS_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CUSTOMERS",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER_DOCUMENTS",
                columns: table => new
                {
                    DocumentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER_DOCUMENTS", x => x.DocumentId);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_DOCUMENTS_CUSTOMERS_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CUSTOMERS",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CUSTOMER_EMPLOYMENT",
                columns: table => new
                {
                    EmploymentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    IndustryId = table.Column<long>(type: "bigint", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CUSTOMER_EMPLOYMENT", x => x.EmploymentId);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_EMPLOYMENT_CUSTOMERS_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "CUSTOMERS",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CUSTOMER_EMPLOYMENT_INDUSTRIES_IndustryId",
                        column: x => x.IndustryId,
                        principalTable: "INDUSTRIES",
                        principalColumn: "IndustryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "INDUSTRIES",
                columns: new[] { "IndustryId", "IndustryName", "IsActive" },
                values: new object[,]
                {
                    { 1L, "Technology", true },
                    { 2L, "Healthcare", true },
                    { 3L, "Finance", true },
                    { 4L, "Education", true },
                    { 5L, "Manufacturing", true }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_CONTACTS_CustomerId",
                table: "CUSTOMER_CONTACTS",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_DOCUMENTS_CustomerId",
                table: "CUSTOMER_DOCUMENTS",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_EMPLOYMENT_CustomerId",
                table: "CUSTOMER_EMPLOYMENT",
                column: "CustomerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CUSTOMER_EMPLOYMENT_IndustryId",
                table: "CUSTOMER_EMPLOYMENT",
                column: "IndustryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CUSTOMER_CONTACTS");

            migrationBuilder.DropTable(
                name: "CUSTOMER_DOCUMENTS");

            migrationBuilder.DropTable(
                name: "CUSTOMER_EMPLOYMENT");

            migrationBuilder.DropTable(
                name: "CUSTOMERS");

            migrationBuilder.DropTable(
                name: "INDUSTRIES");
        }
    }
}
