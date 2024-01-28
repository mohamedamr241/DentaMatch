using DentaMatch.Models.Doctor;
using DentaMatch.Models.Patient;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentaMatch.Data
{
    public class ApplicationDbContext: IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Doctor> Doctors { get; set; }  // MIGRATIONS OF Doctor
        public DbSet<Patient> Patients { get; set; }  // MIGRATIONS OF Patient
    }
}
