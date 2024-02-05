using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentaMatch.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<DentalCase> DentalCases { get; set; }
        public DbSet<CaseChronicDiseases> CaseChronicDiseases { get; set; }
        public DbSet<ChronicDisease> ChronicDiseases { get; set; }
        public DbSet<CaseDentalDiseases> CaseDentalDiseases { get; set; }
        public DbSet<DentalDisease> DentalDiseases { get; set; }
        public DbSet<MouthImages> MouthImages { get; set; }
        public DbSet<PrescriptionImages> PrescriptionImages { get; set; }
        public DbSet<XrayIamges> XrayIamges { get; set; }

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

            modelBuilder.Entity<CaseDentalDiseases>()
                .HasKey(cd => new { cd.CaseId, cd.DiseaseId });

            modelBuilder.Entity<CaseChronicDiseases>()
                .HasKey(cd => new { cd.CaseId, cd.DiseaseId });

            modelBuilder.Entity<DentalCase>()
                .HasOne(d => d.Doctor)
                .WithMany(p => p.DrAssignedCases)
                .HasForeignKey(d => d.DoctorId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<DentalCase>()
                .HasOne(d => d.Patient)
                .WithMany(p => p.PatientCases)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull);


        }
    }
}
