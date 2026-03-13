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
    public class StatisticsService
    {
        private readonly AppDbContext _db;
        public StatisticsService(AppDbContext db) => _db = db;

        private DateTime GetStartDate(string period)
        {
            return period switch
            {
                "день" => DateTime.Now.Date,
                "тиждень" => DateTime.Now.AddDays(-7),
                "місяць" => DateTime.Now.AddMonths(-1),
                "рік" => DateTime.Now.AddYears(-1),
                _ => DateTime.Now.Date
            };
        }

        public void ShowPopularBooks(string period)
        {
            DateTime from = GetStartDate(period);

            Console.WriteLine($"\nНайпопулярніші книги за {period}:");
            Console.WriteLine(new string('-', 60));

            var result = _db.Sales
                .Where(s => s.SoldAt >= from)
                .GroupBy(s => s.BookId)
                .Select(g => new { BookId = g.Key, TotalSold = g.Sum(s => s.Quantity) })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToList();

            if (!result.Any()) { Console.WriteLine("Продажів за цей період немає."); return; }

            foreach (var item in result)
            {
                var book = _db.Books
                    .Include(b => b.Author)
                    .FirstOrDefault(b => b.Id == item.BookId);
                Console.WriteLine($"{book?.Title} | Автор: {book?.Author?.FullName} | Продано: {item.TotalSold} шт.");
            }
        }

        public void ShowPopularAuthors(string period)
        {
            DateTime from = GetStartDate(period);

            Console.WriteLine($"\nНайпопулярніші автори за {period}:");
            Console.WriteLine(new string('-', 60));

            var result = _db.Sales
                .Where(s => s.SoldAt >= from)
                .Include(s => s.Book)
                .ThenInclude(b => b.Author)
                .GroupBy(s => s.Book.AuthorId)
                .Select(g => new { AuthorId = g.Key, TotalSold = g.Sum(s => s.Quantity) })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToList();

            if (!result.Any()) { Console.WriteLine("Продажів за цей період немає."); return; }

            foreach (var item in result)
            {
                var author = _db.Authors.FirstOrDefault(a => a.Id == item.AuthorId);
                Console.WriteLine($"{author?.FullName} | Продано: {item.TotalSold} шт.");
            }
        }

        public void ShowPopularGenres(string period)
        {
            DateTime from = GetStartDate(period);

            Console.WriteLine($"\nНайпопулярніші жанри за {period}:");
            Console.WriteLine(new string('-', 60));

            var result = _db.Sales
                .Where(s => s.SoldAt >= from)
                .Include(s => s.Book)
                .ThenInclude(b => b.Genre)
                .GroupBy(s => s.Book.GenreId)
                .Select(g => new { GenreId = g.Key, TotalSold = g.Sum(s => s.Quantity) })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToList();

            if (!result.Any()) { Console.WriteLine("Продажів за цей період немає."); return; }

            foreach (var item in result)
            {
                var genre = _db.Genres.FirstOrDefault(g => g.Id == item.GenreId);
                Console.WriteLine($"{genre?.Name} | Продано: {item.TotalSold} шт.");
            }
        }

        public void ShowNewest()
        {
            Console.WriteLine("\nНовинки (поточний рік):");
            Console.WriteLine(new string('-', 60));

            var books = _db.Books
                .Where(b => b.Year >= DateTime.Now.Year)
                .Include(b => b.Author)
                .Include(b => b.Genre)
                .OrderByDescending(b => b.Id)
                .ToList();

            if (!books.Any()) { Console.WriteLine("Немає новинок."); return; }

            foreach (var b in books)
                Console.WriteLine($"{b.Id}: {b.Title} ({b.Year}) | {b.Author?.FullName} | {b.Genre?.Name} | Залишок: {b.Quantity}");
        }
    }
}