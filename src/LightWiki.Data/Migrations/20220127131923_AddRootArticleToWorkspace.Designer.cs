// <auto-generated />
using System;
using LightWiki.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LightWiki.Data.Migrations
{
    [DbContext(typeof(WikiContext))]
    [Migration("20220127131923_AddRootArticleToWorkspace")]
    partial class AddRootArticleToWorkspace
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<int>("GroupsId")
                        .HasColumnType("integer")
                        .HasColumnName("groups_id");

                    b.Property<int>("UsersId")
                        .HasColumnType("integer")
                        .HasColumnName("users_id");

                    b.HasKey("GroupsId", "UsersId")
                        .HasName("pk_group_user");

                    b.HasIndex("UsersId")
                        .HasDatabaseName("ix_group_user_users_id");

                    b.ToTable("group_user", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Article", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<int>("GlobalAccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("global_access_rule");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("ParentArticleId")
                        .HasColumnType("integer")
                        .HasColumnName("parent_article_id");

                    b.Property<string>("Slug")
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("integer")
                        .HasColumnName("workspace_id");

                    b.HasKey("Id")
                        .HasName("pk_articles");

                    b.HasIndex("ParentArticleId")
                        .HasDatabaseName("ix_articles_parent_article_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_articles_user_id");

                    b.HasIndex("WorkspaceId")
                        .HasDatabaseName("ix_articles_workspace_id");

                    b.HasIndex("Name", "WorkspaceId")
                        .IsUnique()
                        .HasDatabaseName("ix_articles_name_workspace_id");

                    b.HasIndex("Slug", "WorkspaceId")
                        .IsUnique()
                        .HasDatabaseName("ix_articles_slug_workspace_id");

                    b.ToTable("articles", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.ArticleGroupAccessRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleAccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("article_access_rule");

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer")
                        .HasColumnName("article_id");

                    b.Property<int>("GroupId")
                        .HasColumnType("integer")
                        .HasColumnName("group_id");

                    b.HasKey("Id")
                        .HasName("pk_article_group_access_rules");

                    b.HasIndex("GroupId")
                        .HasDatabaseName("ix_article_group_access_rules_group_id");

                    b.HasIndex("ArticleId", "GroupId")
                        .IsUnique()
                        .HasDatabaseName("ix_article_group_access_rules_article_id_group_id");

                    b.ToTable("article_group_access_rules", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.ArticlePersonalAccessRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleAccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("article_access_rule");

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer")
                        .HasColumnName("article_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_article_personal_access_rules");

                    b.HasIndex("ArticleId")
                        .HasDatabaseName("ix_article_personal_access_rules_article_id");

                    b.HasIndex("UserId", "ArticleId")
                        .IsUnique()
                        .HasDatabaseName("ix_article_personal_access_rules_user_id_article_id");

                    b.ToTable("article_personal_access_rules", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.ArticleVersion", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ArticleId")
                        .HasColumnType("integer")
                        .HasColumnName("article_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Patch")
                        .HasColumnType("text")
                        .HasColumnName("patch");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_article_versions");

                    b.HasIndex("ArticleId")
                        .HasDatabaseName("ix_article_versions_article_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_article_versions_user_id");

                    b.ToTable("article_versions", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Group", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("GroupAccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("group_access_rule");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<string>("Slug")
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.HasKey("Id")
                        .HasName("pk_groups");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_groups_name");

                    b.HasIndex("Slug")
                        .IsUnique()
                        .HasDatabaseName("ix_groups_slug");

                    b.ToTable("groups", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.GroupPersonalAccessRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("access_rule");

                    b.Property<int>("GroupId")
                        .HasColumnType("integer")
                        .HasColumnName("group_id");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_group_personal_access_rules");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_group_personal_access_rules_user_id");

                    b.HasIndex("GroupId", "UserId")
                        .IsUnique()
                        .HasDatabaseName("ix_group_personal_access_rules_group_id_user_id");

                    b.ToTable("group_personal_access_rules", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.HasIndex("Email")
                        .IsUnique()
                        .HasDatabaseName("ix_users_email");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_users_name");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Workspace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created_at");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<int?>("RootArticleId")
                        .HasColumnType("integer")
                        .HasColumnName("root_article_id");

                    b.Property<string>("Slug")
                        .HasColumnType("text")
                        .HasColumnName("slug");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated_at");

                    b.Property<int>("WorkspaceAccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("workspace_access_rule");

                    b.HasKey("Id")
                        .HasName("pk_workspaces");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasDatabaseName("ix_workspaces_name");

                    b.HasIndex("RootArticleId")
                        .IsUnique()
                        .HasDatabaseName("ix_workspaces_root_article_id");

                    b.HasIndex("Slug")
                        .IsUnique()
                        .HasDatabaseName("ix_workspaces_slug");

                    b.ToTable("workspaces", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.WorkspaceGroupAccessRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("GroupId")
                        .HasColumnType("integer")
                        .HasColumnName("group_id");

                    b.Property<int>("WorkspaceAccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("workspace_access_rule");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("integer")
                        .HasColumnName("workspace_id");

                    b.HasKey("Id")
                        .HasName("pk_workspace_group_access_rules");

                    b.HasIndex("WorkspaceId")
                        .HasDatabaseName("ix_workspace_group_access_rules_workspace_id");

                    b.HasIndex("GroupId", "WorkspaceId")
                        .IsUnique()
                        .HasDatabaseName("ix_workspace_group_access_rules_group_id_workspace_id");

                    b.ToTable("workspace_group_access_rules", (string)null);
                });

            modelBuilder.Entity("LightWiki.Domain.Models.WorkspacePersonalAccessRule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.Property<int>("WorkspaceAccessRule")
                        .HasColumnType("integer")
                        .HasColumnName("workspace_access_rule");

                    b.Property<int>("WorkspaceId")
                        .HasColumnType("integer")
                        .HasColumnName("workspace_id");

                    b.HasKey("Id")
                        .HasName("pk_workspace_personal_access_rules");

                    b.HasIndex("WorkspaceId")
                        .HasDatabaseName("ix_workspace_personal_access_rules_workspace_id");

                    b.HasIndex("UserId", "WorkspaceId")
                        .IsUnique()
                        .HasDatabaseName("ix_workspace_personal_access_rules_user_id_workspace_id");

                    b.ToTable("workspace_personal_access_rules", (string)null);
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_group_user_groups_groups_id");

                    b.HasOne("LightWiki.Domain.Models.User", null)
                        .WithMany()
                        .HasForeignKey("UsersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_group_user_users_users_id");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Article", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Article", "ParentArticle")
                        .WithMany()
                        .HasForeignKey("ParentArticleId")
                        .HasConstraintName("fk_articles_articles_parent_article_id");

                    b.HasOne("LightWiki.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_articles_users_user_id");

                    b.HasOne("LightWiki.Domain.Models.Workspace", "Workspace")
                        .WithMany("Articles")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_articles_workspaces_workspace_id");

                    b.Navigation("ParentArticle");

                    b.Navigation("User");

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.ArticleGroupAccessRule", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Article", "Article")
                        .WithMany("GroupAccessRules")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_article_group_access_rules_articles_article_id");

                    b.HasOne("LightWiki.Domain.Models.Group", "Group")
                        .WithMany("ArticleGroupAccessRules")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_article_group_access_rules_groups_group_id");

                    b.Navigation("Article");

                    b.Navigation("Group");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.ArticlePersonalAccessRule", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Article", "Article")
                        .WithMany("PersonalAccessRules")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_article_personal_access_rules_articles_article_id");

                    b.HasOne("LightWiki.Domain.Models.User", "User")
                        .WithMany("ArticlePersonalAccessRules")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_article_personal_access_rules_users_user_id");

                    b.Navigation("Article");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.ArticleVersion", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Article", "Article")
                        .WithMany("Versions")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_article_versions_articles_article_id");

                    b.HasOne("LightWiki.Domain.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_article_versions_users_user_id");

                    b.Navigation("Article");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.GroupPersonalAccessRule", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Group", "Group")
                        .WithMany("GroupPersonalAccessRules")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_group_personal_access_rules_groups_group_id");

                    b.HasOne("LightWiki.Domain.Models.User", "User")
                        .WithMany("GroupPersonalAccessRules")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_group_personal_access_rules_users_user_id");

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Workspace", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Article", "RootArticle")
                        .WithOne("RootedWorkspace")
                        .HasForeignKey("LightWiki.Domain.Models.Workspace", "RootArticleId")
                        .HasConstraintName("fk_workspaces_articles_root_article_id1");

                    b.Navigation("RootArticle");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.WorkspaceGroupAccessRule", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.Group", "Group")
                        .WithMany("WorkspaceGroupAccessRules")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_workspace_group_access_rules_groups_group_id");

                    b.HasOne("LightWiki.Domain.Models.Workspace", "Workspace")
                        .WithMany("GroupAccessRules")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_workspace_group_access_rules_workspaces_workspace_id");

                    b.Navigation("Group");

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.WorkspacePersonalAccessRule", b =>
                {
                    b.HasOne("LightWiki.Domain.Models.User", "User")
                        .WithMany("WorkspacePersonalAccessRules")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_workspace_personal_access_rules_users_user_id");

                    b.HasOne("LightWiki.Domain.Models.Workspace", "Workspace")
                        .WithMany("PersonalAccessRules")
                        .HasForeignKey("WorkspaceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("fk_workspace_personal_access_rules_workspaces_workspace_id");

                    b.Navigation("User");

                    b.Navigation("Workspace");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Article", b =>
                {
                    b.Navigation("GroupAccessRules");

                    b.Navigation("PersonalAccessRules");

                    b.Navigation("RootedWorkspace");

                    b.Navigation("Versions");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Group", b =>
                {
                    b.Navigation("ArticleGroupAccessRules");

                    b.Navigation("GroupPersonalAccessRules");

                    b.Navigation("WorkspaceGroupAccessRules");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.User", b =>
                {
                    b.Navigation("ArticlePersonalAccessRules");

                    b.Navigation("GroupPersonalAccessRules");

                    b.Navigation("WorkspacePersonalAccessRules");
                });

            modelBuilder.Entity("LightWiki.Domain.Models.Workspace", b =>
                {
                    b.Navigation("Articles");

                    b.Navigation("GroupAccessRules");

                    b.Navigation("PersonalAccessRules");
                });
#pragma warning restore 612, 618
        }
    }
}
