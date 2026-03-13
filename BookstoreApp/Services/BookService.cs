using BookstoreApp.Data;
using BookstoreApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookstoreApp.Services
{
    public class BookService
    {
        private readonly AppDbContext _db;

        public BookService(AppDbContext db)
        {
            _db = db;
        }

        public void AddBook(string title, int authorId, int publisherId, int genreId, int pages, int year,
                            decimal costPrice, decimal sellPrice, int quantity, bool isSequel, int? previousBookId = null)
        {
            var newBook = new Book
            {
                Title = title,
                AuthorId = authorId,
                PublisherId = publisherId,
                GenreId = genreId,
                Pages = pages,
                Year = year,
                CostPrice = costPrice,
                SellPrice = sellPrice,
                Quantity = quantity,
                IsSequel = isSequel,
                PreviousBookId = previousBookId
            };

            _db.Books.Add(newBook);
            _db.SaveChanges();

            Console.WriteLine($"Книгу '{title}' додано. ID: {newBook.Id}, Кількість: {quantity}");
        }

        public void ShowAllBooks()
        {
            Console.WriteLine("\nСписок усіх книг:");
            Console.WriteLine(new string('-', 80));

            var books = _db.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.Genre)
                .ToList();

            if (!books.Any())
            {
                Console.WriteLine("Бібліотека порожня.");
                return;
            }

            foreach (var book in books)
            {
                Console.WriteLine($"ID: {book.Id}");
                Console.WriteLine($"Назва: {book.Title}");
                Console.WriteLine($"Автор: {book.Author?.FullName ?? "Невідомо"} (ID: {book.AuthorId})");
                Console.WriteLine($"Видавництво: {book.Publisher?.Name ?? "Невідомо"} (ID: {book.PublisherId})");
                Console.WriteLine($"Жанр: {book.Genre?.Name ?? "Невідомо"} (ID: {book.GenreId})");
                Console.WriteLine($"Сторінок: {book.Pages} | Рік: {book.Year}");
                Console.WriteLine($"Собівартість: {book.CostPrice} | Ціна: {book.SellPrice}");
                Console.WriteLine($"Кількість: {book.Quantity}");
                Console.WriteLine($"Продовження: {(book.IsSequel ? "Так" : "Ні")}" +
                                (book.PreviousBookId.HasValue ? $" (попередня ID: {book.PreviousBookId})" : ""));
                Console.WriteLine(new string('-', 80));
            }
        }

        public void DeleteBook(int id)
        {
            var book = _db.Books.Find(id);
            if (book != null)
            {
                _db.Books.Remove(book);
                _db.SaveChanges();
                Console.WriteLine($"Книга ID {id} видалена");
            }
            else
            {
                Console.WriteLine("Книга не знайдена");
            }
        }

        public void UpdateBook(int id, string? title, decimal? sellPrice, int? pages,
                               int? authorId, int? publisherId, int? genreId,
                               int? year, decimal? costPrice, int? quantity, bool? isSequel, int? previousBookId)
        {
            var book = _db.Books.Find(id);
            if (book == null)
            {
                Console.WriteLine("Книга не знайдена");
                return;
            }

            if (title != null) book.Title = title;
            if (sellPrice.HasValue) book.SellPrice = sellPrice.Value;
            if (pages.HasValue) book.Pages = pages.Value;
            if (authorId.HasValue) book.AuthorId = authorId.Value;
            if (publisherId.HasValue) book.PublisherId = publisherId.Value;
            if (genreId.HasValue) book.GenreId = genreId.Value;
            if (year.HasValue) book.Year = year.Value;
            if (costPrice.HasValue) book.CostPrice = costPrice.Value;
            if (quantity.HasValue) book.Quantity = quantity.Value;
            if (isSequel.HasValue) book.IsSequel = isSequel.Value;
            if (previousBookId.HasValue) book.PreviousBookId = previousBookId.Value;

            _db.SaveChanges();
            Console.WriteLine($"Книга ID {id} оновлена");
        }

        public void SellBook(int bookId, int quantity, int customerId)
        {
            var book = _db.Books.Find(bookId);
            var customer = _db.Customers.Find(customerId);

            if (book == null) { Console.WriteLine("Книга не знайдена"); return; }
            if (customer == null) { Console.WriteLine("Покупця не знайдено"); return; }
            if (book.Quantity < quantity)
            {
                Console.WriteLine($"Недостатньо екземплярів. Доступно: {book.Quantity}");
                return;
            }

            book.Quantity -= quantity;

            var sale = new Sale
            {
                BookId = bookId,
                CustomerId = customerId,
                Quantity = quantity,
                PricePerUnit = book.SellPrice,
                SoldAt = DateTime.Now
            };

            _db.Sales.Add(sale);
            _db.SaveChanges();

            Console.WriteLine($"Продано: '{book.Title}' x{quantity} шт. по {book.SellPrice} грн покупцю '{customer.FullName}'. Залишок: {book.Quantity}");
        }

        public void WriteOffBook(int bookId, int quantity)
        {
            var book = _db.Books.Find(bookId);
            if (book == null)
            {
                Console.WriteLine("Книга не знайдена");
                return;
            }
            if (book.Quantity < quantity)
            {
                Console.WriteLine($"Недостатньо екземплярів. Доступно: {book.Quantity}");
                return;
            }

            book.Quantity -= quantity;
            _db.SaveChanges();

            Console.WriteLine($"Списано '{book.Title}' x{quantity} шт. Залишок: {book.Quantity}");
        }

        public void AddToPromotion(int id, decimal discountPercent)
        {
            var book = _db.Books.Find(id);
            if (book == null)
            {
                Console.WriteLine("Книга не знайдена");
                return;
            }
            decimal oldPrice = book.SellPrice;
            book.SellPrice = Math.Round(book.SellPrice * (1 - discountPercent / 100), 2);
            _db.SaveChanges();
            Console.WriteLine($"Акція: '{book.Title}' {oldPrice} -> {book.SellPrice} (-{discountPercent}%)");
        }

        public void ReserveBook(int bookId, int customerId)
        {
            var book = _db.Books.Find(bookId);
            var customer = _db.Customers.Find(customerId);
            if (book == null) { Console.WriteLine("Книга не знайдена"); return; }
            if (customer == null) { Console.WriteLine("Покупця не знайдено"); return; }
            if (book.Quantity < 1) { Console.WriteLine("Немає доступних екземплярів"); return; }

            var reservation = new CustomerReservation
            {
                BookId = bookId,
                CustomerId = customerId,
                ReservedAt = DateTime.Now
            };
            _db.Reservations.Add(reservation);
            _db.SaveChanges();
            Console.WriteLine($"Книга '{book.Title}' зарезервована для '{customer.FullName}'");
        }

        public List<Book> SearchByTitle(string title) =>
            _db.Books.Where(b => b.Title.Contains(title))
                     .Include(b => b.Author)
                     .Include(b => b.Genre)
                     .ToList();

        public List<Book> SearchByAuthor(int authorId) =>
            _db.Books.Where(b => b.AuthorId == authorId)
                     .Include(b => b.Author)
                     .ToList();

        public List<Book> SearchByGenre(int genreId) =>
            _db.Books.Where(b => b.GenreId == genreId)
                     .Include(b => b.Genre)
                     .ToList();

        public void ShowSearchResults(List<Book> books, string query)
        {
            Console.WriteLine($"\nРезультати '{query}':");
            Console.WriteLine(new string('-', 40));
            if (!books.Any()) { Console.WriteLine("Нічого не знайдено."); return; }
            foreach (var b in books)
                Console.WriteLine($"{b.Id}: {b.Title} | Автор: {b.Author?.FullName} | Жанр: {b.Genre?.Name} | Кількість: {b.Quantity}");
        }

        public void ShowNewest()
        {
            Console.WriteLine("\nНовинки (поточний рік):");
            Console.WriteLine(new string('-', 40));
            var books = _db.Books
                .Where(b => b.Year >= DateTime.Now.Year)
                .Include(b => b.Author)
                .OrderByDescending(b => b.Id)
                .ToList();
            if (!books.Any()) { Console.WriteLine("Немає новинок."); return; }
            foreach (var b in books)
                Console.WriteLine($"{b.Id}: {b.Title} ({b.Year}) | {b.Author?.FullName} | Кількість: {b.Quantity}");
        }

        public void ShowPopular()
        {
            Console.WriteLine("\nНайпопулярніші книги (топ-5 за продажами):");
            Console.WriteLine(new string('-', 40));
            var popular = _db.Sales
                .GroupBy(s => s.BookId)
                .Select(g => new { BookId = g.Key, TotalSold = g.Sum(s => s.Quantity) })
                .OrderByDescending(g => g.TotalSold)
                .Take(5)
                .ToList();

            if (!popular.Any()) { Console.WriteLine("Продажів ще немає."); return; }

            foreach (var item in popular)
            {
                var book = _db.Books.Include(b => b.Author).FirstOrDefault(b => b.Id == item.BookId);
                Console.WriteLine($"{book?.Title} | Автор: {book?.Author?.FullName} | Продано: {item.TotalSold} шт.");
            }
        }
    }
}
