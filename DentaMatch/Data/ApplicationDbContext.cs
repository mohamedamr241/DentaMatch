using DentaMatch.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentaMatch.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }  // MIGRATIONS OF ApplicationUsers

        public DbSet<Patient> PatientDetails { get; set; }
        public DbSet<Doctor> DoctorDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure one-to-one relationships

            modelBuilder.Entity<Patient>()
                .HasOne(pd => pd.User)
                .WithOne()
                .HasForeignKey<Patient>(pd => pd.UserId);

            modelBuilder.Entity<Doctor>()
                .HasOne(dd => dd.User)
                .WithOne()
                .HasForeignKey<Doctor>(dd => dd.UserId);
        }
    }
}
