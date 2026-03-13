using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookstoreApp.Data;
using BookstoreApp.Entities;

namespace BookstoreApp.Services
{
    public class CustomerService
    {
        private readonly AppDbContext _db;
        public CustomerService(AppDbContext db) => _db = db;

        public void Show()
        {
            var customers = _db.Customers.ToList();
            Console.WriteLine("\nПокупці:");
            if (!customers.Any()) { Console.WriteLine("Немає покупців."); return; }
            foreach (var c in customers)
                Console.WriteLine($"{c.Id}: {c.FullName} | Тел: {c.Phone ?? "-"} | Email: {c.Email ?? "-"}");
        }

        public int AddAndReturnId(string fullName, string? phone, string? email)
        {
            var customer = new Customer
            {
                FullName = fullName,
                Phone = phone,
                Email = email
            };
            _db.Customers.Add(customer);
            _db.SaveChanges();
            Console.WriteLine($"Покупця '{fullName}' створено з ID: {customer.Id}");
            return customer.Id;
        }
    }
}
