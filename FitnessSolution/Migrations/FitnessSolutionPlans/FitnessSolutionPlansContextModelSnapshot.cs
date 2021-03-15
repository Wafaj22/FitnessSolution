﻿// <auto-generated />
using FitnessSolution.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FitnessSolution.Migrations.FitnessSolutionPlans
{
    [DbContext(typeof(FitnessSolutionPlansContext))]
    partial class FitnessSolutionPlansContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("FitnessSolution.Models.Diet", b =>
                {
                    b.Property<int>("DietId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("DietDescription")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("DietImageName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("DietTitle")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(24)");

                    b.HasKey("DietId");

                    b.ToTable("Diet");
                });

            modelBuilder.Entity("FitnessSolution.Models.Exercice", b =>
                {
                    b.Property<int>("ExerciceId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ExerciceDescription")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("ExerciceImageName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ExerciceTitle")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Level")
                        .HasColumnType("nvarchar(24)");

                    b.Property<int>("WorkoutId")
                        .HasColumnType("int");

                    b.HasKey("ExerciceId");

                    b.HasIndex("WorkoutId");

                    b.ToTable("Exercice");
                });

            modelBuilder.Entity("FitnessSolution.Models.Recipe", b =>
                {
                    b.Property<int>("RecipeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DietId")
                        .HasColumnType("int");

                    b.Property<string>("RecipeDescription")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("RecipeImageName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("RecipeTitle")
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(24)");

                    b.HasKey("RecipeId");

                    b.HasIndex("DietId");

                    b.ToTable("Recipe");
                });

            modelBuilder.Entity("FitnessSolution.Models.Workout", b =>
                {
                    b.Property<int>("WorkoutId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(24)");

                    b.Property<string>("WorkoutDescription")
                        .HasColumnType("nvarchar(4000)");

                    b.Property<string>("WorkoutImageName")
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("WorkoutTitle")
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("WorkoutId");

                    b.ToTable("Workout");
                });

            modelBuilder.Entity("FitnessSolution.Models.Exercice", b =>
                {
                    b.HasOne("FitnessSolution.Models.Workout", "Workout")
                        .WithMany("Exercices")
                        .HasForeignKey("WorkoutId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FitnessSolution.Models.Recipe", b =>
                {
                    b.HasOne("FitnessSolution.Models.Diet", "Diet")
                        .WithMany("Recipes")
                        .HasForeignKey("DietId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
