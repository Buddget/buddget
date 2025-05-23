﻿// <auto-generated />
using System;
using Buddget.DAL.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Buddget.DAL.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250327221354_MakeDescriptionNullable")]
    partial class MakeDescriptionNullable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Buddget.DAL.Entities.CategoryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsDefault")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialGoalCategoryEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<int?>("FinancialGoalEntityId")
                        .HasColumnType("integer");

                    b.Property<int>("FinancialGoalId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("FinancialGoalEntityId");

                    b.HasIndex("FinancialGoalId");

                    b.ToTable("FinancialGoalCategories");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialGoalEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("TargetAmount")
                        .HasColumnType("decimal(15,3)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("FinancialGoals");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialGoalSpaceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("FinancialGoalEntityId")
                        .HasColumnType("integer");

                    b.Property<int>("FinancialGoalId")
                        .HasColumnType("integer");

                    b.Property<int>("FinancialSpaceId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FinancialGoalEntityId");

                    b.HasIndex("FinancialGoalId");

                    b.HasIndex("FinancialSpaceId");

                    b.ToTable("FinancialGoalSpaces");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialSpaceEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<byte[]>("ImageData")
                        .HasColumnType("bytea");

                    b.Property<string>("ImageName")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("OwnerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("FinancialSpaces");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialSpaceMemberEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("FinancialSpaceId")
                        .HasColumnType("integer");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FinancialSpaceId");

                    b.HasIndex("UserId");

                    b.ToTable("FinancialSpaceMembers");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.TransactionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(15,3)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Currency")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("FinancialSpaceId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("FinancialSpaceId");

                    b.HasIndex("UserId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.UserEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RegisteredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.CategoryEntity", b =>
                {
                    b.HasOne("Buddget.DAL.Entities.UserEntity", "User")
                        .WithMany("Categories")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialGoalCategoryEntity", b =>
                {
                    b.HasOne("Buddget.DAL.Entities.CategoryEntity", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Buddget.DAL.Entities.FinancialGoalEntity", null)
                        .WithMany("Categories")
                        .HasForeignKey("FinancialGoalEntityId");

                    b.HasOne("Buddget.DAL.Entities.FinancialGoalEntity", "FinancialGoal")
                        .WithMany()
                        .HasForeignKey("FinancialGoalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("FinancialGoal");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialGoalEntity", b =>
                {
                    b.HasOne("Buddget.DAL.Entities.UserEntity", "User")
                        .WithMany("FinancialGoals")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialGoalSpaceEntity", b =>
                {
                    b.HasOne("Buddget.DAL.Entities.FinancialGoalEntity", null)
                        .WithMany("Spaces")
                        .HasForeignKey("FinancialGoalEntityId");

                    b.HasOne("Buddget.DAL.Entities.FinancialGoalEntity", "FinancialGoal")
                        .WithMany()
                        .HasForeignKey("FinancialGoalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Buddget.DAL.Entities.FinancialSpaceEntity", "FinancialSpace")
                        .WithMany("FinancialGoalSpaces")
                        .HasForeignKey("FinancialSpaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FinancialGoal");

                    b.Navigation("FinancialSpace");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialSpaceEntity", b =>
                {
                    b.HasOne("Buddget.DAL.Entities.UserEntity", "Owner")
                        .WithMany("OwnedSpaces")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialSpaceMemberEntity", b =>
                {
                    b.HasOne("Buddget.DAL.Entities.FinancialSpaceEntity", "FinancialSpace")
                        .WithMany("Members")
                        .HasForeignKey("FinancialSpaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Buddget.DAL.Entities.UserEntity", "User")
                        .WithMany("SpaceMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FinancialSpace");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.TransactionEntity", b =>
                {
                    b.HasOne("Buddget.DAL.Entities.CategoryEntity", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Buddget.DAL.Entities.FinancialSpaceEntity", "FinancialSpace")
                        .WithMany("Transactions")
                        .HasForeignKey("FinancialSpaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Buddget.DAL.Entities.UserEntity", "User")
                        .WithMany("Transactions")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("FinancialSpace");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialGoalEntity", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Spaces");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.FinancialSpaceEntity", b =>
                {
                    b.Navigation("FinancialGoalSpaces");

                    b.Navigation("Members");

                    b.Navigation("Transactions");
                });

            modelBuilder.Entity("Buddget.DAL.Entities.UserEntity", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("FinancialGoals");

                    b.Navigation("OwnedSpaces");

                    b.Navigation("SpaceMemberships");

                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}
