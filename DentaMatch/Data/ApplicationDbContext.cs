using DentaMatch.Models;
using DentaMatch.Models.Dental_Case.CaseProgress;
using DentaMatch.Models.Dental_Case.Chronic_Diseases;
using DentaMatch.Models.Dental_Case.Comments;
using DentaMatch.Models.Dental_Case.Dental_Diseases;
using DentaMatch.Models.Dental_Case.Images;
using DentaMatch.Models.Dental_Case.Reports;
using DentaMatch.Models.Doctor_Models;
using DentaMatch.Models.Notifications;
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
        public DbSet<DentalCaseComments> Comment { get; set; }
        public DbSet<DentalCaseProgress> DentalCaseProgress { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<UserNotifications> UserNotifications { get; set; }

        public DbSet<DoctorSpecializationRequests> DoctorSpecialization { get; set; }

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

            modelBuilder.Entity<Report>()
                  .HasKey(r => new { r.CaseId, r.DoctorId });

            modelBuilder.Entity<Report>()
                 .HasOne(r => r.DentalCase)
                 .WithMany(r => r.Reports)
                 .HasForeignKey(r => r.CaseId)
                 .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Report>()
                 .HasOne(r => r.Doctor)
                 .WithMany()
                 .HasForeignKey(r => r.DoctorId)
                 .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.Entity<DentalCaseProgress>()
                .HasOne(dcp => dcp.DentalCases)
                .WithMany()
                .HasForeignKey(dcp => dcp.CaseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorSpecializationRequests>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Specialization)
                    .IsRequired();

                entity.Property(e => e.IsVerified)
                    .IsRequired();

                entity.HasOne(e => e.Doctor)
                    .WithMany(d => d.doctorSpecializationRequests)  // Assuming Doctor has a collection of DoctorSpecializationRequests
                    .HasForeignKey(e => e.DoctorId);
            });

        }
    }
}
