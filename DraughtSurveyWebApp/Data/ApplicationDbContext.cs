using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DraughtSurveyWebApp.Models;

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
        public DbSet<UserHydrostaticTableRow> UserHydrostaticTableRows { get; set; }
        public DbSet<DeductiblesInput> DeductiblesInputs { get; set; }
        public DbSet<DeductiblesResults> DeductiblesResults { get; set; }

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

            modelBuilder.Entity<UserHydrostaticTableRow>()
                .HasOne(u => u.ApplicationUser)
                .WithMany()
                .HasForeignKey(u => u.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
