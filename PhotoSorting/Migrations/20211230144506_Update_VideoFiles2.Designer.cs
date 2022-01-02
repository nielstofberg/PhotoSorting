﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PhotoSorting.Models;

namespace PhotoSorting.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20211230144506_Update_VideoFiles2")]
    partial class Update_VideoFiles2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PhotoSorting.Models.DoubleSet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("Preferred")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("DoubleSets");
                });

            modelBuilder.Entity("PhotoSorting.Models.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CameraMake")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CameraModel")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateAccessed")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateTaken")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int?>("DoubleSetId")
                        .HasColumnType("int");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<bool>("ToDelete")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("DoubleSetId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("PhotoSorting.Models.VideoFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<bool>("Deleted")
                        .HasColumnType("bit");

                    b.Property<int?>("DoubleSetId")
                        .HasColumnType("int");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Length")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Path")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<bool>("ToDelete")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("DoubleSetId");

                    b.ToTable("VideoFiles");
                });

            modelBuilder.Entity("PhotoSorting.Models.Photo", b =>
                {
                    b.HasOne("PhotoSorting.Models.DoubleSet", null)
                        .WithMany("Photos")
                        .HasForeignKey("DoubleSetId");
                });

            modelBuilder.Entity("PhotoSorting.Models.VideoFile", b =>
                {
                    b.HasOne("PhotoSorting.Models.DoubleSet", null)
                        .WithMany("VideoFiles")
                        .HasForeignKey("DoubleSetId");
                });

            modelBuilder.Entity("PhotoSorting.Models.DoubleSet", b =>
                {
                    b.Navigation("Photos");

                    b.Navigation("VideoFiles");
                });
#pragma warning restore 612, 618
        }
    }
}
