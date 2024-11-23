﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using llassist.ApiService.Repositories;

#nullable disable

namespace llassist.ApiService.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241117045717_AddLibraryModels")]
    partial class AddLibraryModels
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("CollectionEntries", b =>
                {
                    b.Property<string>("CollectionId")
                        .HasColumnType("text");

                    b.Property<string>("EntriesId")
                        .HasColumnType("text");

                    b.HasKey("CollectionId", "EntriesId");

                    b.HasIndex("EntriesId");

                    b.ToTable("CollectionEntries");
                });

            modelBuilder.Entity("llassist.Common.Models.AppSetting", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Key");

                    b.ToTable("AppSettings", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Article", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Abstract")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Authors")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DOI")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("MustRead")
                        .HasColumnType("boolean");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("Articles", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.ArticleKeySemantic", b =>
                {
                    b.Property<string>("ArticleId")
                        .HasColumnType("text");

                    b.Property<int>("KeySemanticIndex")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("ArticleId", "KeySemanticIndex");

                    b.ToTable("ArticleKeySemantics", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.ArticleRelevance", b =>
                {
                    b.Property<string>("ArticleId")
                        .HasColumnType("text");

                    b.Property<string>("EstimateRelevanceJobId")
                        .HasColumnType("text");

                    b.Property<int>("RelevanceIndex")
                        .HasColumnType("integer");

                    b.Property<string>("ContributionReason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("ContributionScore")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsContributing")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRelevant")
                        .HasColumnType("boolean");

                    b.Property<string>("Question")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RelevanceReason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("RelevanceScore")
                        .HasColumnType("double precision");

                    b.HasKey("ArticleId", "EstimateRelevanceJobId", "RelevanceIndex");

                    b.ToTable("ArticleRelevances", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.EstimateRelevanceJob", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<int>("CompletedArticles")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ModelName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("TotalArticles")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("EstimateRelevanceJobs", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.ArticleReference", b =>
                {
                    b.Property<string>("ArticleId")
                        .HasColumnType("text");

                    b.Property<string>("EntryId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ArticleId", "EntryId");

                    b.HasIndex("EntryId");

                    b.ToTable("ArticleReferences", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Catalog", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Owner")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Catalogs", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Category", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CatalogId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Depth")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("ParentId")
                        .HasColumnType("text");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("SchemaType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.HasIndex("ParentId");

                    b.ToTable("Categories", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.CategoryEntry", b =>
                {
                    b.Property<string>("CategoryId")
                        .HasColumnType("text");

                    b.Property<string>("EntryId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("CategoryId", "EntryId");

                    b.HasIndex("EntryId");

                    b.ToTable("CategoryEntries", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Collection", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CatalogId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.ToTable("Collections", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Entry", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CatalogId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Citation")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(5000)
                        .HasColumnType("character varying(5000)");

                    b.Property<string>("EntryType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Identifier")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Metadata")
                        .IsRequired()
                        .HasColumnType("jsonb");

                    b.Property<DateTime?>("PublishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.ToTable("Entries", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.EntryLabel", b =>
                {
                    b.Property<string>("EntryId")
                        .HasColumnType("text");

                    b.Property<string>("LabelId")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("EntryId", "LabelId");

                    b.HasIndex("LabelId");

                    b.ToTable("EntryLabels", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Label", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("CatalogId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character varying(7)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("CatalogId");

                    b.ToTable("Labels", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Resource", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EntryId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Hash")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(2000)
                        .HasColumnType("character varying(2000)");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.HasIndex("EntryId");

                    b.ToTable("Resources", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Project", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Projects", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.ProjectDefinition", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Definition")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectDefinitions", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.QuestionDefinition", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("Definition")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ResearchQuestionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ResearchQuestionId");

                    b.ToTable("QuestionDefinitions", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.ResearchQuestion", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("ProjectId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("QuestionText")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ResearchQuestions", (string)null);
                });

            modelBuilder.Entity("llassist.Common.Models.Snapshot", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("EntityId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EntityType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EstimateRelevanceJobId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("SerializedEntity")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EstimateRelevanceJobId");

                    b.ToTable("Snapshots", (string)null);
                });

            modelBuilder.Entity("CollectionEntries", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Collection", null)
                        .WithMany()
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("llassist.Common.Models.Library.Entry", null)
                        .WithMany()
                        .HasForeignKey("EntriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("llassist.Common.Models.Article", b =>
                {
                    b.HasOne("llassist.Common.Models.Project", "Project")
                        .WithMany("Articles")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("llassist.Common.Models.ArticleKeySemantic", b =>
                {
                    b.HasOne("llassist.Common.Models.Article", "Article")
                        .WithMany("ArticleKeySemantics")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");
                });

            modelBuilder.Entity("llassist.Common.Models.ArticleRelevance", b =>
                {
                    b.HasOne("llassist.Common.Models.Article", "Article")
                        .WithMany("ArticleRelevances")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.ArticleReference", b =>
                {
                    b.HasOne("llassist.Common.Models.Article", "Article")
                        .WithMany()
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("llassist.Common.Models.Library.Entry", "Entry")
                        .WithMany("ArticleReferences")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Article");

                    b.Navigation("Entry");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Category", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Catalog", "Catalog")
                        .WithMany("Categories")
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("llassist.Common.Models.Library.Category", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Catalog");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.CategoryEntry", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Category", "Category")
                        .WithMany("Entries")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("llassist.Common.Models.Library.Entry", "Entry")
                        .WithMany("Categories")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("Entry");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Collection", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Catalog", "Catalog")
                        .WithMany()
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Catalog");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Entry", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Catalog", "Catalog")
                        .WithMany("Entries")
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Catalog");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.EntryLabel", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Entry", "Entry")
                        .WithMany("Labels")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("llassist.Common.Models.Library.Label", "Label")
                        .WithMany()
                        .HasForeignKey("LabelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entry");

                    b.Navigation("Label");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Label", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Catalog", "Catalog")
                        .WithMany("Labels")
                        .HasForeignKey("CatalogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Catalog");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Resource", b =>
                {
                    b.HasOne("llassist.Common.Models.Library.Entry", "Entry")
                        .WithMany("Resources")
                        .HasForeignKey("EntryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Entry");
                });

            modelBuilder.Entity("llassist.Common.Models.ProjectDefinition", b =>
                {
                    b.HasOne("llassist.Common.Models.Project", "Project")
                        .WithMany("ProjectDefinitions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("llassist.Common.Models.QuestionDefinition", b =>
                {
                    b.HasOne("llassist.Common.Models.ResearchQuestion", "ResearchQuestion")
                        .WithMany("QuestionDefinitions")
                        .HasForeignKey("ResearchQuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ResearchQuestion");
                });

            modelBuilder.Entity("llassist.Common.Models.ResearchQuestion", b =>
                {
                    b.HasOne("llassist.Common.Models.Project", "Project")
                        .WithMany("ResearchQuestions")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Project");
                });

            modelBuilder.Entity("llassist.Common.Models.Snapshot", b =>
                {
                    b.HasOne("llassist.Common.Models.EstimateRelevanceJob", "EstimateRelevanceJob")
                        .WithMany("Snapshots")
                        .HasForeignKey("EstimateRelevanceJobId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EstimateRelevanceJob");
                });

            modelBuilder.Entity("llassist.Common.Models.Article", b =>
                {
                    b.Navigation("ArticleKeySemantics");

                    b.Navigation("ArticleRelevances");
                });

            modelBuilder.Entity("llassist.Common.Models.EstimateRelevanceJob", b =>
                {
                    b.Navigation("Snapshots");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Catalog", b =>
                {
                    b.Navigation("Categories");

                    b.Navigation("Entries");

                    b.Navigation("Labels");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Category", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Entries");
                });

            modelBuilder.Entity("llassist.Common.Models.Library.Entry", b =>
                {
                    b.Navigation("ArticleReferences");

                    b.Navigation("Categories");

                    b.Navigation("Labels");

                    b.Navigation("Resources");
                });

            modelBuilder.Entity("llassist.Common.Models.Project", b =>
                {
                    b.Navigation("Articles");

                    b.Navigation("ProjectDefinitions");

                    b.Navigation("ResearchQuestions");
                });

            modelBuilder.Entity("llassist.Common.Models.ResearchQuestion", b =>
                {
                    b.Navigation("QuestionDefinitions");
                });
#pragma warning restore 612, 618
        }
    }
}