﻿// <auto-generated />
using System;
using Kutuphane1.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kutuphane1.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220724114956_first")]
    partial class first
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Kutuphane1.Models.KitapAdi", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Aktif")
                        .HasColumnType("bit");

                    b.Property<decimal?>("FIYAT")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("GuncellemeTarihi")
                        .HasColumnType("datetime2");

                    b.Property<string>("GuncelleyenKullanici")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("KİTABİNADİ")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OlusturanKullanici")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OlusturmaTarihi")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Silindi")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("KitapAdi");
                });

            modelBuilder.Entity("Kutuphane1.Models.OrtakRaf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ADET")
                        .HasColumnType("int");

                    b.Property<bool>("Aktif")
                        .HasColumnType("bit");

                    b.Property<DateTime>("GuncellemeTarihi")
                        .HasColumnType("datetime2");

                    b.Property<string>("GuncelleyenKullanici")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("KAYİTTARİHİ")
                        .HasColumnType("datetime2");

                    b.Property<int>("KitapAdiId")
                        .HasColumnType("int");

                    b.Property<string>("KİTAPADİ")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OlusturanKullanici")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OlusturmaTarihi")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Silindi")
                        .HasColumnType("bit");

                    b.Property<int>("excelKodu")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("KitapAdiId");

                    b.ToTable("OrtakRaf");
                });

            modelBuilder.Entity("Kutuphane1.Models.SayisalKitap", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("ADET")
                        .HasColumnType("int");

                    b.Property<bool>("Aktif")
                        .HasColumnType("bit");

                    b.Property<decimal?>("FİYAT")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime>("GuncellemeTarihi")
                        .HasColumnType("datetime2");

                    b.Property<string>("GuncelleyenKullanici")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("KAYİTTARİHİ")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("KDV")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("KitapAdiId")
                        .HasColumnType("int");

                    b.Property<string>("KİTAPADİ")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OlusturanKullanici")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OlusturmaTarihi")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Silindi")
                        .HasColumnType("bit");

                    b.Property<decimal?>("TOPLAMTUTAR")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("TUTAR")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("KitapAdiId");

                    b.ToTable("SayisalKitap");
                });

            modelBuilder.Entity("Kutuphane1.Models.OrtakRaf", b =>
                {
                    b.HasOne("Kutuphane1.Models.KitapAdi", "KitapAdi")
                        .WithMany("OrtakRafKitaplar")
                        .HasForeignKey("KitapAdiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KitapAdi");
                });

            modelBuilder.Entity("Kutuphane1.Models.SayisalKitap", b =>
                {
                    b.HasOne("Kutuphane1.Models.KitapAdi", "KitapAdi")
                        .WithMany("SayisalKitaplar")
                        .HasForeignKey("KitapAdiId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("KitapAdi");
                });

            modelBuilder.Entity("Kutuphane1.Models.KitapAdi", b =>
                {
                    b.Navigation("OrtakRafKitaplar");

                    b.Navigation("SayisalKitaplar");
                });
#pragma warning restore 612, 618
        }
    }
}
