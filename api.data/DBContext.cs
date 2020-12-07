using System.Data.Entity;
using api.core.Security;
using api.data.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
    public class DBContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DBContext()
        { }

        public Microsoft.EntityFrameworkCore.DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=API-Database.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
               new User
               {
                   Email = "admin@company",
                   FirstName = "Admin - Shailen",
                   LastName = "Sukul",
                   Password = "admin",
                   Roles = new string[] { RoleDefinitions.Administrator },
                   Salt = "".GetSalt(32)
               }
            );

            modelBuilder.Entity<User>().HasData(
              new User
              {
                  Email = "nancy.drew@company",
                  FirstName = "Nancy",
                  LastName = "Drew",
                  Password = "office",
                  Roles = new string[] { RoleDefinitions.Office },
                  Salt = "".GetSalt(32)
              }
            );

            modelBuilder.Entity<User>().HasData(
             new User
             {
                 Email = "rakesh.prasad@company",
                 FirstName = "Rakesh",
                 LastName = "Prasad",
                 Password = "customer",
                 Roles = new string[] { RoleDefinitions.Customer },
                 Salt = "".GetSalt(32)
             }
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
