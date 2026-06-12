using EmployeeManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.ApplicationContext
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options) { }


        public DbSet<Employee> Employees { get; set; }
        public DbSet<Admin> Admins  { get; set; }
        public DbSet<Attendence> Attendences   { get; set; }
        public DbSet<Payment> Payments   { get; set; }
    }
}
