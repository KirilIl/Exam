using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace ExamPractice
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
    }
    public class Doctor : Employee
    {
        public string Specialization { get; set; }
    }
    public class Engineer : Employee
    {
        public string FavoriteVideogame { get; set; }
    }
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Latitude { get; set; }
        public int Longitude { get; set; }
        public List<Employee> Employees { get; set; }
    }
    public class Context : DbContext
    {
        public Context() : base("ef2")
        { }
        public DbSet<City> Cities { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Engineer> Engineers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Entity<Employee>().HasRequired(x => x.City).WithMany(x => x.Employees).HasForeignKey(x => x.CityId);

            modelBuilder.Entity<Doctor>().Property(d => d.Specialization).IsRequired();
            modelBuilder.Entity<Employee>().Property(e => e.Name).HasMaxLength(128);
        }
    }


    class Program
    {
        public static IEnumerable<string> GetSpecialization(string name)
        {
            using (var ctx = new Context())
            {
                return ctx.Doctors
                    .Join(ctx.Cities.Where(n => n.Name == name),
                    doct => doct.CityId,
                    city => city.Id,
                    (doct, city) => doct)
                    .Select(x => x.Specialization).ToList();
            }
        }


        static void Main(string[] args)
        {

            foreach (var sp in GetSpecialization("Kiev"))
            {
                Console.WriteLine(sp);
            }
            Console.ReadLine();
        }
    }
}
