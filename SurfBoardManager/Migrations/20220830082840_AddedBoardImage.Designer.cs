﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SurfBoardManager.Data;

#nullable disable

namespace SurfBoardManager.Migrations
{
    [DbContext(typeof(SurfBoardManagerContext))]
    [Migration("20220830082840_AddedBoardImage")]
    partial class AddedBoardImage
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("SurfBoardManager.Models.BoardPost", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BoardImage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("BoardType")
                        .HasColumnType("INT");

                    b.Property<string>("Equipment")
                        .IsRequired()
                        .HasColumnType("NVarChar(255)");

                    b.Property<decimal>("Length")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("NVarChar(255)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Thickness")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Volume")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("Width")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("BoardPost");
                });
#pragma warning restore 612, 618
        }
    }
}
