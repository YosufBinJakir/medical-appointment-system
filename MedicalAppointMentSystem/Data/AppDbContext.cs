using MedicalAppointMentSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace MedicalAppointMentSystem.Data
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Doctor>? Doctors { get; set; }
        public virtual DbSet<Patient>? Patients { get; set; }
        public virtual DbSet<Appointment>? Appointments { get; set; }
        public virtual DbSet<PrescriptionDetail>? PrescriptionDetails { get; set; }
        public virtual DbSet<Medicine>? Medicines { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured) 
        //    {
        //        optionsBuilder.UseSqlServer("Server=DESKTOP-T478IJP\\SQLEXPRESS;Database=MAS_DB;User Id=sa;Password=Abc@123;MultipleActiveResultSets=True;Encrypt=False");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.HasKey(a => a.AppointmentId);

                entity.HasMany(a => a.PrescriptionDetails)
                      .WithOne(pd => pd.Appointment)
                      .HasForeignKey(pd => pd.AppointmentId);
            });

        }
    }
}
