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
    public class PromotionService
    {
        private readonly AppDbContext _db;
        public PromotionService(AppDbContext db) => _db = db;

        public void RestoreExpiredPromotions()
        {
            var expired = _db.Promotions
                .Include(p => p.PromotionBooks)
                .Where(p => p.EndDate < DateTime.Now)
                .ToList();

            foreach (var promotion in expired)
            {
                foreach (var pb in promotion.PromotionBooks)
                {
                    var book = _db.Books.Find(pb.BookId);
                    if (book != null)
                        book.SellPrice = pb.OriginalPrice;
                }
                _db.PromotionBooks.RemoveRange(promotion.PromotionBooks);
            }

            if (expired.Any())
                _db.SaveChanges();
        }

        public void ShowAll()
        {
            var promotions = _db.Promotions
                .Include(p => p.PromotionBooks)
                .ThenInclude(pb => pb.Book)
                .ToList();

            Console.WriteLine("\nАкції:");
            Console.WriteLine(new string('-', 60));

            if (!promotions.Any()) { Console.WriteLine("Акцій немає."); return; }

            foreach (var p in promotions)
            {
                string status = DateTime.Now >= p.StartDate && DateTime.Now <= p.EndDate
                    ? "[АКТИВНА]" : "[НЕАКТИВНА]";
                Console.WriteLine($"{p.Id}: {p.Name} {status}");
                Console.WriteLine($"   Знижка: {p.DiscountPercent}% | З {p.StartDate:dd.MM.yyyy HH:mm} по {p.EndDate:dd.MM.yyyy HH:mm}");
                if (p.PromotionBooks.Any())
                {
                    foreach (var pb in p.PromotionBooks)
                        Console.WriteLine($"   - {pb.Book?.Title} | Оригінальна: {pb.OriginalPrice} | Акційна: {Math.Round(pb.OriginalPrice * (1 - p.DiscountPercent / 100), 2)}");
                }
                else
                {
                    Console.WriteLine("   Книг в акції немає.");
                }
                Console.WriteLine(new string('-', 60));
            }
        }

        public void CreatePromotion(string name, decimal discountPercent, DateTime startDate, DateTime endDate)
        {
            var promotion = new Promotion
            {
                Name = name,
                DiscountPercent = discountPercent,
                StartDate = startDate,
                EndDate = endDate
            };
            _db.Promotions.Add(promotion);
            _db.SaveChanges();
            Console.WriteLine($"Акцію '{name}' створено з ID: {promotion.Id}");
        }

        public void AddBookToPromotion(int promotionId, int bookId)
        {
            var promotion = _db.Promotions.Find(promotionId);
            var book = _db.Books.Find(bookId);

            if (promotion == null) { Console.WriteLine("Акцію не знайдено"); return; }
            if (book == null) { Console.WriteLine("Книгу не знайдено"); return; }
            if (promotion.EndDate < DateTime.Now) { Console.WriteLine("Акція вже завершена"); return; }

            bool already = _db.PromotionBooks.Any(pb => pb.PromotionId == promotionId && pb.BookId == bookId);
            if (already) { Console.WriteLine("Книга вже в цій акції"); return; }

            decimal originalPrice = book.SellPrice;

            _db.PromotionBooks.Add(new PromotionBook
            {
                PromotionId = promotionId,
                BookId = bookId,
                OriginalPrice = originalPrice
            });

            book.SellPrice = Math.Round(originalPrice * (1 - promotion.DiscountPercent / 100), 2);
            _db.SaveChanges();

            Console.WriteLine($"Книгу '{book.Title}' додано до акції '{promotion.Name}'");
            Console.WriteLine($"Ціна: {originalPrice} -> {book.SellPrice} (-{promotion.DiscountPercent}%)");
        }

        public void RemoveBookFromPromotion(int promotionId, int bookId)
        {
            var promotionBook = _db.PromotionBooks
                .FirstOrDefault(pb => pb.PromotionId == promotionId && pb.BookId == bookId);

            if (promotionBook == null) { Console.WriteLine("Книгу не знайдено в цій акції"); return; }

            var book = _db.Books.Find(bookId);
            if (book != null)
                book.SellPrice = promotionBook.OriginalPrice;

            _db.PromotionBooks.Remove(promotionBook);
            _db.SaveChanges();
            Console.WriteLine($"Книгу видалено з акції. Ціна відновлена: {book?.SellPrice}");
        }
    }
}