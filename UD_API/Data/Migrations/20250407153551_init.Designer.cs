﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Parcellation_API.Data;

#nullable disable

namespace Parcellation_API.Data.Migrations
{
    [DbContext(typeof(ParcellationContext))]
    [Migration("20250407153551_init")]
    partial class init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.3");

            modelBuilder.Entity("Parcellation_API.Models.ParcellationInput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Caller")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("MajorRoadWidth")
                        .HasColumnType("REAL");

                    b.Property<double>("MinorRoadWidth")
                        .HasColumnType("REAL");

                    b.Property<string>("ParcelCurve")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RoadNetworkCurves")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Inputs");
                });

            modelBuilder.Entity("Parcellation_API.Models.ParcellationOutput", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("InputId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("OutputData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("InputId")
                        .IsUnique();

                    b.ToTable("Outputs");
                });

            modelBuilder.Entity("Parcellation_API.Models.ParcellationOutput", b =>
                {
                    b.HasOne("Parcellation_API.Models.ParcellationInput", "Input")
                        .WithOne("Output")
                        .HasForeignKey("Parcellation_API.Models.ParcellationOutput", "InputId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Input");
                });

            modelBuilder.Entity("Parcellation_API.Models.ParcellationInput", b =>
                {
                    b.Navigation("Output");
                });
#pragma warning restore 612, 618
        }
    }
}
