using BookstoreApp.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BookstoreApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<CustomerReservation> Reservations { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Customer> Customers { get; set; }
       
        public DbSet<PromotionBook> PromotionBooks { get; set; }
        public AppDbContext()
        {
        }

       
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            
            modelBuilder.Entity<Author>().HasData(
                new Author { Id = 1, FullName = "Тарас Шевченко" },
                new Author { Id = 2, FullName = "Іван Франко" },
                new Author { Id = 3, FullName = "Леся Українка" }
            );

            modelBuilder.Entity<Publisher>().HasData(
                new Publisher { Id = 1, Name = "А-БА-БА-ГА-ЛА-МА-ГА" },
                new Publisher { Id = 2, Name = "Фоліо" }
            );

            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Поезія" },
                new Genre { Id = 2, Name = "Проза" },
                new Genre { Id = 3, Name = "Фантастика" }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Login = "admin", PasswordHash = "admin123", Role = "Admin" },
                new User { Id = 2, Login = "user", PasswordHash = "user123", Role = "User" }
            );
        }
    }
}
