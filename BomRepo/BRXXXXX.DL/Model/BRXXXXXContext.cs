﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BomRepo.BRXXXXX.DL
{
    public partial class BRXXXXXContext : DbContext
    {
        public BRXXXXXContext()
        {
        }

        public BRXXXXXContext(DbContextOptions<BRXXXXXContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectStatus> ProjectStatuses { get; set; }
        public virtual DbSet<UserBranch> UserBranches { get; set; }
        public virtual DbSet<UserBranchAssembly> UserBranchAssemblies { get; set; }
        public virtual DbSet<UserBranchAssemblyPart> UserBranchAssemblyParts { get; set; }
        public virtual DbSet<Entity> Entities { get; set; }
        public virtual DbSet<ProjectEntity> ProjectEntities { get; set; }
        public virtual DbSet<Assembly> Assemblies { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=ITDEV\\SQLDEVELOPMENT;Database=BR00000;User Id=brsysadmin;Password=1KillsAll");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<Project>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.Property(e => e.ProjectStatusId).IsRequired();
                entity.Property(e => e.Number).IsRequired();
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<ProjectStatus>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<UserBranch>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.Property(e => e.Username).IsRequired();
                entity.Property(e => e.ProjectId).IsRequired();
                entity.Property(e => e.CreatedOn).IsRequired();
            });

            modelBuilder.Entity<UserBranchAssembly>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.Property(e => e.CreatedOn).IsRequired();
                entity.Property(e => e.UserBranchId).IsRequired();
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<UserBranchAssemblyPart>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.Property(e => e.UserBranchAssemblyId).IsRequired();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Qty).IsRequired();
            });

            modelBuilder.Entity<Entity>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });

            modelBuilder.Entity<ProjectEntity>(entity =>
            {
                entity.HasKey(e => new { e.EntityId });
                entity.HasKey(e => new { e.ProjectId });
            });

            modelBuilder.Entity<Assembly>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });
        }
    }
}
