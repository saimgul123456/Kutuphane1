using Microsoft.EntityFrameworkCore.Migrations;

namespace Kutuphane1.Migrations
{
    public partial class guncelleme : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KitapAdiId",
                table: "SayisalKitap",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SayisalKitap_KitapAdiId",
                table: "SayisalKitap",
                column: "KitapAdiId");

            migrationBuilder.AddForeignKey(
                name: "FK_SayisalKitap_KitapAdi_KitapAdiId",
                table: "SayisalKitap",
                column: "KitapAdiId",
                principalTable: "KitapAdi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SayisalKitap_KitapAdi_KitapAdiId",
                table: "SayisalKitap");

            migrationBuilder.DropIndex(
                name: "IX_SayisalKitap_KitapAdiId",
                table: "SayisalKitap");

            migrationBuilder.DropColumn(
                name: "KitapAdiId",
                table: "SayisalKitap");
        }
    }
}
