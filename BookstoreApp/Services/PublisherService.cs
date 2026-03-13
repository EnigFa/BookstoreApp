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
    public class PublisherService
    {
        private readonly AppDbContext _db;
        public PublisherService(AppDbContext db) => _db = db;

        public List<Publisher> GetAll() => _db.Publishers.ToList();

        public void Show()
        {
            Console.WriteLine("\nВидавництва:");
            foreach (var p in GetAll())
                Console.WriteLine($"{p.Id}: {p.Name}");
        }

        public int AddAndReturnId(string name)
        {
            var publisher = new Publisher { Name = name };
            _db.Publishers.Add(publisher);
            _db.SaveChanges();
            Console.WriteLine($"Видавництво '{name}' створено з ID: {publisher.Id}");
            return publisher.Id;
        }
    }
}