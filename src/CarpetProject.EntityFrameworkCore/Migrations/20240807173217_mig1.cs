using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarpetProject.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EasyAbpFileManagementFiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ParentId = table.Column<Guid>(type: "uuid", nullable: true),
                    FileContainerName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    MimeType = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    FileType = table.Column<int>(type: "integer", nullable: false),
                    SubFilesQuantity = table.Column<int>(type: "integer", nullable: false),
                    HasSubdirectories = table.Column<bool>(type: "boolean", nullable: false),
                    ByteSize = table.Column<long>(type: "bigint", nullable: false),
                    Hash = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    BlobName = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Flag = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    SoftDeletionToken = table.Column<string>(type: "text", nullable: true, defaultValue: ""),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EasyAbpFileManagementFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EasyAbpFileManagementUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    Surname = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PhoneNumber = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EasyAbpFileManagementUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_BlobName",
                table: "EasyAbpFileManagementFiles",
                column: "BlobName");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_FileName_ParentId_OwnerUserId_Fi~",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "FileName", "ParentId", "OwnerUserId", "FileContainerName", "TenantId", "SoftDeletionToken" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_Hash",
                table: "EasyAbpFileManagementFiles",
                column: "Hash");

            migrationBuilder.CreateIndex(
                name: "IX_EasyAbpFileManagementFiles_ParentId_OwnerUserId_FileContain~",
                table: "EasyAbpFileManagementFiles",
                columns: new[] { "ParentId", "OwnerUserId", "FileContainerName", "FileName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EasyAbpFileManagementFiles");

            migrationBuilder.DropTable(
                name: "EasyAbpFileManagementUsers");
        }
    }
}
