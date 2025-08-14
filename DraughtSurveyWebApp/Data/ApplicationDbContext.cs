using DraughtSurveyWebApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DraughtSurveyWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Inspection> Inspections { get; set; }
        public DbSet<VesselInput> VesselInputs { get; set; }
        public DbSet<CargoInput> CargoInputs { get; set; }
        public DbSet<CargoResult> CargoResults { get; set; }

        public DbSet<DraughtSurveyBlock> DraughtSurveyBlocks { get; set; }

        public DbSet<DraughtsInput> DraughtsInputs { get; set; }
        public DbSet<DraughtsResults> DraughtsResults { get; set; }

        public DbSet<HydrostaticInput> HydrostaticInputs { get; set; }
        public DbSet<HydrostaticResults> HydrostaticResults { get; set; }        

        public DbSet<UserHydrostaticTableHeader> UserHydrostaticTableHeaders { get; set; }
        public DbSet<UserHydrostaticTableRow> UserHydrostaticTableRows { get; set; }

        public DbSet<DeductiblesInput> DeductiblesInputs { get; set; }
        public DbSet<DeductiblesResults> DeductiblesResults { get; set; }

        public DbSet<ExcelTemplate> ExcelTemplates { get; set; }
        public DbSet<ExcelExportLog> ExcelExportLogs { get; set; }

        public DbSet<UserSession> UserSessions => Set<UserSession>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);         
                        
            modelBuilder.Entity<Inspection>()
                .HasOne(i => i.ApplicationUser)
                .WithMany(u => u.Inspections)
                .HasForeignKey(i => i.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserHydrostaticTableHeader>()
                .HasMany(u => u.UserHydrostaticTableRows)
                .WithOne(r => r.UserHydrostaticTableHeader)                                
                .HasForeignKey(u => u.UserHydrostaticTableHeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExcelTemplate>(entity => 
            {
                entity.HasIndex(e => e.Name);

                entity.HasOne(i => i.Owner)
                    .WithMany()
                    .HasForeignKey(i => i.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<UserSession>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Id).ValueGeneratedNever();

                entity.HasIndex(x => x.LastSeenUtc);
                entity.HasIndex(x => new { x.UserId, x.StartedUtc });
                entity.HasIndex(x => new { x.UserId, x.LastSeenUtc });

                entity.Property(x => x.UserId).IsRequired().HasMaxLength(450);
                entity.Property(x => x.Ip).HasMaxLength(64);
                entity.Property(x => x.UserAgent).HasMaxLength(1024);

                entity.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ExcelExportLog>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Id).ValueGeneratedNever();
                entity.HasIndex(x => x.CreatedUtc);
                entity.HasIndex(x => x.UserId);

                entity.HasOne(x => x.User)
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.ExcelTemplate)
                    .WithMany()
                    .HasForeignKey(x => x.TemplateId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DraughtSurveyBlock>(entity =>
            {
                entity.Property(e => e.SurveyTimeStart)
                    .HasColumnType("timestamp without time zone");

                entity.Property(e => e.SurveyTimeEnd)
                    .HasColumnType("timestamp without time zone");

                entity.Property(e => e.CargoOperationsDateTime)
                    .HasColumnType("timestamp without time zone");
            });




            //modelBuilder.Entity<ExcelTemplate>()
            //    .HasIndex(e => e.Name)
            //    .HasOne(i => i.Owner)
            //    .WithMany()
            //    .HasForeignKey(i => i.OwnerId)
            //    .OnDelete(DeleteBehavior.Restrict);


            //modelBuilder.Entity<UserHydrostaticTableHeader>()
            //    .HasOne(u => u.ApplicationUser)
            //    .WithMany()
            //    .HasForeignKey(u => u.ApplicationUserId)
            //    .OnDelete(DeleteBehavior.Restrict);



            //modelBuilder.Entity<UserHydrostaticTableRow>()
            //    .HasOne(u => u.ApplicationUser)
            //    .WithMany()
            //    .HasForeignKey(u => u.ApplicationUserId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
