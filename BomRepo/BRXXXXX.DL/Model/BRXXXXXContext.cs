using System;
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
        public virtual DbSet<UserBranchPart> UserBranchParts { get; set; }
        public virtual DbSet<UserBranchPartPlacement> UserBranchPartPlacements { get; set; }
        public virtual DbSet<UserBranchPartProperty> UserBranchPartProperties { get; set; }
        public virtual DbSet<PartDefinition> PartDefinitions { get; set; }
        public virtual DbSet<ProjectPartDefinition> ProjectPartDefinitions { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<PartPlacement> PartPlacements { get; set; }
        public virtual DbSet<Property> Properties { get; set; }
        public virtual DbSet<PartDefinitionProperty> PartDefinitionProperties { get; set; }
        public virtual DbSet<PartProperty> PartProperties { get; set; }
        
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

            modelBuilder.Entity<UserBranchPart>(entity =>
            {
                entity.HasKey(e => new { e.Id });
                entity.Property(e => e.CreatedOn).IsRequired();
                entity.Property(e => e.UserBranchId).IsRequired();
                entity.Property(e => e.PartDefinitionId).IsRequired();
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<UserBranchPartPlacement>(entity =>
            {
                entity.HasKey(e => new { e.UserBranchId, e.ParentUserBranchPartId, e.ChildUserBranchPartId });
                entity.Property(e => e.Qty).IsRequired();
            });

            modelBuilder.Entity<UserBranchPartProperty>(entity => {
                entity.HasKey(e => new { e.UserBranchPartId, e.PropertyId });
            });

            modelBuilder.Entity<PartDefinition>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });

            modelBuilder.Entity<ProjectPartDefinition>(entity =>
            {
                entity.HasKey(e => new { e.PartDefinitionId, e.ProjectId });
            });

            modelBuilder.Entity<Part>(entity =>
            {
                entity.HasKey(e => new { e.Id });
            });

            modelBuilder.Entity<PartPlacement>(entity => {
                entity.HasKey(e => new { e.ParentPartId, e.ChildPartId });
                entity.Property(e => e.Qty).IsRequired();
            });

            modelBuilder.Entity<Property>(entity => {
                entity.HasKey(e => new { e.Id });
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.IsString).IsRequired();
                entity.Property(e => e.IsInteger).IsRequired();
                entity.Property(e => e.IsDouble).IsRequired();
                entity.Property(e => e.IsBoolean).IsRequired();
                entity.Property(e => e.IsDateTime).IsRequired();
            });

            modelBuilder.Entity<PartDefinitionProperty>(entity => {
                entity.HasKey(e => new { e.PartDefinitionId, e.PropertyId });
            });

            modelBuilder.Entity<PartProperty>(entity => {
                entity.HasKey(e => new { e.PartId, e.PropertyId });
            });
        }
    }
}
