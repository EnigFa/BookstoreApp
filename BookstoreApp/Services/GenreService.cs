using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreApp.Data;
using BookstoreApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApp.Services
{
    public class GenreService
    {
        private readonly AppDbContext _db;
        public GenreService(AppDbContext db) => _db = db;

        public List<Genre> GetAll() => _db.Genres.ToList();

        public void Show()
        {
            Console.WriteLine("\nЖанри:");
            foreach (var g in GetAll())
                Console.WriteLine($"{g.Id}: {g.Name}");
        }

        public int AddAndReturnId(string name)
        {
            var genre = new Genre { Name = name };
            _db.Genres.Add(genre);
            _db.SaveChanges();
            Console.WriteLine($"Жанр '{name}' створено з ID: {genre.Id}");
            return genre.Id;
        }
    }
}
