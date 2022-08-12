using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Kutuphane1.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KitapAdi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KİTABİNADİ = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FIYAT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OlusturanKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuncelleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    Silindi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KitapAdi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrtakRaf",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KAYİTTARİHİ = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KİTAPADİ = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADET = table.Column<int>(type: "int", nullable: false),
                    excelKodu = table.Column<int>(type: "int", nullable: false),
                    KitapAdiId = table.Column<int>(type: "int", nullable: false),
                    OlusturanKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuncelleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    Silindi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrtakRaf", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrtakRaf_KitapAdi_KitapAdiId",
                        column: x => x.KitapAdiId,
                        principalTable: "KitapAdi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SayisalKitap",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KAYİTTARİHİ = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KİTAPADİ = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ADET = table.Column<int>(type: "int", nullable: false),
                    FİYAT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TUTAR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    KDV = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TOPLAMTUTAR = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    KitapAdiId = table.Column<int>(type: "int", nullable: false),
                    OlusturanKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GuncelleyenKullanici = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuncellemeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Aktif = table.Column<bool>(type: "bit", nullable: false),
                    Silindi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SayisalKitap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SayisalKitap_KitapAdi_KitapAdiId",
                        column: x => x.KitapAdiId,
                        principalTable: "KitapAdi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrtakRaf_KitapAdiId",
                table: "OrtakRaf",
                column: "KitapAdiId");

            migrationBuilder.CreateIndex(
                name: "IX_SayisalKitap_KitapAdiId",
                table: "SayisalKitap",
                column: "KitapAdiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrtakRaf");

            migrationBuilder.DropTable(
                name: "SayisalKitap");

            migrationBuilder.DropTable(
                name: "KitapAdi");
        }
    }
}
