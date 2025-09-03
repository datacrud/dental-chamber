using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DM.Models
{
    public class DentalDbContext: DbContext
    {
        public DentalDbContext() : base("name=DefaultConnection")
        {
            Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;            
        }

      
        public DbSet<Product> Products { set; get; }
        public DbSet<Status> Statuses { set; get; }
        public DbSet<Inventory> Inventories { set; get; }         

        public DbSet<MedicalInfo> MedicalInfos { set; get; }
        public DbSet<MedicalService> MedicalServices { set; get; }
        public DbSet<Patient> Patients { set; get; }
        public DbSet<Prescription> Prescriptions { set; get; }         
        public DbSet<PatientMedicalService> PatientMedicalServices { set; get; } 
        public DbSet<PatientMedicalInfo> PatientMedicalInfos { set; get; } 

        public DbSet<Payment>  Payments { set; get; }

        public DbSet<Doctor> Doctors { get; set; } 
        public DbSet<Appointment> Appointments { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();


            modelBuilder.Entity<Patient>().HasIndex(p => p.Code).IsUnique();

        }

    }
}
