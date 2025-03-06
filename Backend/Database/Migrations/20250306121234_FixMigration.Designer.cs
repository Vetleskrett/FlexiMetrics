﻿// <auto-generated />
using System;
using System.Text.Json;
using Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Database.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250306121234_FixMigration")]
    partial class FixMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Database.Models.Analysis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AnalyzerId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("CompletedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AnalyzerId");

                    b.ToTable("Analyses");
                });

            modelBuilder.Entity("Database.Models.Analyzer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.ToTable("Analyzers");
                });

            modelBuilder.Entity("Database.Models.Assignment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("CollaborationType")
                        .HasColumnType("integer");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("DueDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("GradingType")
                        .HasColumnType("integer");

                    b.Property<bool>("Mandatory")
                        .HasColumnType("boolean");

                    b.Property<int?>("MaxPoints")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Published")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Assignments");
                });

            modelBuilder.Entity("Database.Models.AssignmentField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<int?>("Max")
                        .HasColumnType("integer");

                    b.Property<int?>("Min")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Regex")
                        .HasColumnType("text");

                    b.Property<int?>("SubType")
                        .HasColumnType("integer");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentId");

                    b.ToTable("AssignmentFields");
                });

            modelBuilder.Entity("Database.Models.Course", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Semester")
                        .HasColumnType("integer");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("Database.Models.CourseStudent", b =>
                {
                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("StudentId")
                        .HasColumnType("uuid");

                    b.HasKey("CourseId", "StudentId");

                    b.HasIndex("StudentId");

                    b.ToTable("CourseStudents");
                });

            modelBuilder.Entity("Database.Models.CourseTeacher", b =>
                {
                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TeacherId")
                        .HasColumnType("uuid");

                    b.HasKey("CourseId", "TeacherId");

                    b.HasIndex("TeacherId");

                    b.ToTable("CourseTeachers");
                });

            modelBuilder.Entity("Database.Models.Delivery", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.HasIndex("TeamId");

                    b.HasIndex("AssignmentId", "StudentId", "TeamId")
                        .IsUnique();

                    b.ToTable("Deliveries");
                });

            modelBuilder.Entity("Database.Models.DeliveryAnalysis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AnalysisId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DeliveryId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("AnalysisId");

                    b.HasIndex("DeliveryId");

                    b.ToTable("DeliveryAnalyses");
                });

            modelBuilder.Entity("Database.Models.DeliveryAnalysisField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("DeliveryAnalysisId")
                        .HasColumnType("uuid");

                    b.Property<JsonDocument>("JsonValue")
                        .HasColumnType("jsonb");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Type")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("DeliveryAnalysisId");

                    b.ToTable("DeliveryAnalysisFields");
                });

            modelBuilder.Entity("Database.Models.DeliveryField", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentFieldId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("DeliveryId")
                        .HasColumnType("uuid");

                    b.Property<JsonDocument>("JsonValue")
                        .HasColumnType("jsonb");

                    b.HasKey("Id");

                    b.HasIndex("AssignmentFieldId");

                    b.HasIndex("DeliveryId");

                    b.ToTable("DeliveryFields");
                });

            modelBuilder.Entity("Database.Models.Feedback", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AssignmentId")
                        .HasColumnType("uuid");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasMaxLength(21)
                        .HasColumnType("character varying(21)");

                    b.Property<Guid?>("StudentId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("StudentId");

                    b.HasIndex("TeamId");

                    b.HasIndex("AssignmentId", "StudentId", "TeamId")
                        .IsUnique();

                    b.ToTable("Feedbacks");

                    b.HasDiscriminator().HasValue("Feedback");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Database.Models.Team", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("CourseId")
                        .HasColumnType("uuid");

                    b.Property<int>("TeamNr")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("Database.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Role")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("TeamUser", b =>
                {
                    b.Property<Guid>("StudentsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TeamId")
                        .HasColumnType("uuid");

                    b.HasKey("StudentsId", "TeamId");

                    b.HasIndex("TeamId");

                    b.ToTable("TeamUser");
                });

            modelBuilder.Entity("Database.Models.ApprovalFeedback", b =>
                {
                    b.HasBaseType("Database.Models.Feedback");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("boolean");

                    b.HasDiscriminator().HasValue("ApprovalFeedback");
                });

            modelBuilder.Entity("Database.Models.LetterFeedback", b =>
                {
                    b.HasBaseType("Database.Models.Feedback");

                    b.Property<int>("LetterGrade")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("LetterFeedback");
                });

            modelBuilder.Entity("Database.Models.PointsFeedback", b =>
                {
                    b.HasBaseType("Database.Models.Feedback");

                    b.Property<int>("Points")
                        .HasColumnType("integer");

                    b.HasDiscriminator().HasValue("PointsFeedback");
                });

            modelBuilder.Entity("Database.Models.Analysis", b =>
                {
                    b.HasOne("Database.Models.Analyzer", "Analyzer")
                        .WithMany()
                        .HasForeignKey("AnalyzerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Analyzer");
                });

            modelBuilder.Entity("Database.Models.Analyzer", b =>
                {
                    b.HasOne("Database.Models.Assignment", "Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignment");
                });

            modelBuilder.Entity("Database.Models.Assignment", b =>
                {
                    b.HasOne("Database.Models.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("Database.Models.AssignmentField", b =>
                {
                    b.HasOne("Database.Models.Assignment", "Assignment")
                        .WithMany("Fields")
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Assignment");
                });

            modelBuilder.Entity("Database.Models.CourseStudent", b =>
                {
                    b.HasOne("Database.Models.Course", "Course")
                        .WithMany("CourseStudents")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Models.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("Database.Models.CourseTeacher", b =>
                {
                    b.HasOne("Database.Models.Course", "Course")
                        .WithMany("CourseTeachers")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Models.User", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("Database.Models.Delivery", b =>
                {
                    b.HasOne("Database.Models.Assignment", "Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Models.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Database.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Assignment");

                    b.Navigation("Student");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Database.Models.DeliveryAnalysis", b =>
                {
                    b.HasOne("Database.Models.Analysis", "Analysis")
                        .WithMany("DeliveryAnalyses")
                        .HasForeignKey("AnalysisId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Models.Delivery", "Delivery")
                        .WithMany()
                        .HasForeignKey("DeliveryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Analysis");

                    b.Navigation("Delivery");
                });

            modelBuilder.Entity("Database.Models.DeliveryAnalysisField", b =>
                {
                    b.HasOne("Database.Models.DeliveryAnalysis", "DeliveryAnalysis")
                        .WithMany("Fields")
                        .HasForeignKey("DeliveryAnalysisId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DeliveryAnalysis");
                });

            modelBuilder.Entity("Database.Models.DeliveryField", b =>
                {
                    b.HasOne("Database.Models.AssignmentField", "AssignmentField")
                        .WithMany()
                        .HasForeignKey("AssignmentFieldId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Models.Delivery", "Delivery")
                        .WithMany("Fields")
                        .HasForeignKey("DeliveryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AssignmentField");

                    b.Navigation("Delivery");
                });

            modelBuilder.Entity("Database.Models.Feedback", b =>
                {
                    b.HasOne("Database.Models.Assignment", "Assignment")
                        .WithMany()
                        .HasForeignKey("AssignmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Models.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Database.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Assignment");

                    b.Navigation("Student");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Database.Models.Team", b =>
                {
                    b.HasOne("Database.Models.Course", "Course")
                        .WithMany("Teams")
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Course");
                });

            modelBuilder.Entity("TeamUser", b =>
                {
                    b.HasOne("Database.Models.User", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Database.Models.Team", null)
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Database.Models.Analysis", b =>
                {
                    b.Navigation("DeliveryAnalyses");
                });

            modelBuilder.Entity("Database.Models.Assignment", b =>
                {
                    b.Navigation("Fields");
                });

            modelBuilder.Entity("Database.Models.Course", b =>
                {
                    b.Navigation("CourseStudents");

                    b.Navigation("CourseTeachers");

                    b.Navigation("Teams");
                });

            modelBuilder.Entity("Database.Models.Delivery", b =>
                {
                    b.Navigation("Fields");
                });

            modelBuilder.Entity("Database.Models.DeliveryAnalysis", b =>
                {
                    b.Navigation("Fields");
                });
#pragma warning restore 612, 618
        }
    }
}
