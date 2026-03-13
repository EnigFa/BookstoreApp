using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using BookstoreApp.Data;
using BookstoreApp.Entities;

namespace BookstoreApp.Services
{
    public class AuthorService
    {
        private readonly AppDbContext _db;
        public AuthorService(AppDbContext db) => _db = db;

        public List<Author> GetAll() => _db.Authors.ToList();

        public void Show()
        {
            Console.WriteLine("\nАвтори:");
            foreach (var a in GetAll())
                Console.WriteLine($"{a.Id}: {a.FullName}");
        }

        public int AddAndReturnId(string fullName)
        {
            var author = new Author { FullName = fullName };
            _db.Authors.Add(author);
            _db.SaveChanges();
            Console.WriteLine($"Автора '{fullName}' створено з ID: {author.Id}");
            return author.Id;
        }
    }
}

