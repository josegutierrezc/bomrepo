using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BomRepo.BRMaster.DL
{
    public partial class BRMasterContext : DbContext
    {
        public BRMasterContext()
        {
        }

        public BRMasterContext(DbContextOptions<BRMasterContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CostumerUser> CostumerUsers { get; set; }
        public virtual DbSet<Costumer> Costumers { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=ITDEV\\SQLDEVELOPMENT;Database=BRMaster;User Id=brsysadmin;Password=1KillsAll");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");

            modelBuilder.Entity<CostumerUser>(entity =>
            {
                entity.HasKey(e => new { e.CostumerId, e.UserId });
            });

            modelBuilder.Entity<Costumer>(entity =>
            {
                entity.Property(e => e.City).HasMaxLength(2);

                entity.Property(e => e.Name).IsRequired();

                entity.Property(e => e.Number).IsRequired();

                entity.Property(e => e.State).HasMaxLength(2);

                entity.Property(e => e.ZipCode).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Firstname).IsRequired();

                entity.Property(e => e.Lastname).IsRequired();

                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Username).IsRequired();
            });
        }
    }
}
