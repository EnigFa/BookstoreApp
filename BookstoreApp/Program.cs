using BookstoreApp.Data;
using BookstoreApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace BookstoreApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.Unicode; ;
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("MSSQLConnection"));

            using var db = new AppDbContext(optionsBuilder.Options);

            var bookService = new BookService(db);
            var authorService = new AuthorService(db);
            var publisherService = new PublisherService(db);
            var genreService = new GenreService(db);
            var customerService = new CustomerService(db);
            var statisticsService = new StatisticsService(db);
            var promotionService = new PromotionService(db);
            promotionService.RestoreExpiredPromotions();

            Console.WriteLine("=== Авторизація ===");
            Console.Write("Логін: ");
            string login = Console.ReadLine() ?? "";
            Console.Write("Пароль: ");
            string password = Console.ReadLine() ?? "";

            if (!IsValidUser(db, login, password))
            {
                Console.WriteLine("Невірний логін/пароль!");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Успішний вхід! Натисніть...");
            Console.ReadKey();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Книгарня ===");
                Console.WriteLine("--- Книги ---");
                Console.WriteLine("1.  Додати книгу");
                Console.WriteLine("2.  Всі книги");
                Console.WriteLine("11. Видалити книгу");
                Console.WriteLine("12. Редагувати книгу");
                Console.WriteLine("13. Продати книгу");
                Console.WriteLine("14. Списати книгу");
                Console.WriteLine("15. Акція на книгу");
                Console.WriteLine("16. Резерв книги");
                Console.WriteLine("--- Пошук ---");
                Console.WriteLine("6.  Пошук за назвою");
                Console.WriteLine("7.  Пошук за автором");
                Console.WriteLine("8.  Пошук за жанром");
                Console.WriteLine("--- Списки ---");
                Console.WriteLine("9.  Новинки");
                Console.WriteLine("10. Популярні");
                Console.WriteLine("--- Довідники ---");
                Console.WriteLine("3.  Автори");
                Console.WriteLine("4.  Видавництва");
                Console.WriteLine("5.  Жанри");
                Console.WriteLine("17. Покупці");
                Console.WriteLine("--- Статистика ---");
                Console.WriteLine("18. Популярні книги за період");
                Console.WriteLine("19. Популярні автори за період");
                Console.WriteLine("20. Популярні жанри за період");
                Console.WriteLine("0.  Вихід");
                Console.Write("Вибір: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        return;

                    case "1":
                        AddBookMenu(bookService, authorService, publisherService, genreService);
                        break;

                    case "2":
                        bookService.ShowAllBooks();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "3":
                        authorService.Show();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "4":
                        publisherService.Show();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "5":
                        genreService.Show();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "6":
                        Console.Write("Назва (частково): ");
                        Console.Out.Flush();
                        while (Console.KeyAvailable) Console.ReadKey(true);
                        string titleQuery = Console.ReadLine() ?? "";
                        if (string.IsNullOrWhiteSpace(titleQuery))
                        {
                            Console.WriteLine("Введіть назву для пошуку!");
                            Console.ReadKey();
                            break;
                        }
                        bookService.ShowSearchResults(bookService.SearchByTitle(titleQuery), titleQuery);
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "7":
                        authorService.Show();
                        Console.Write("ID автора: ");
                        if (int.TryParse(Console.ReadLine(), out int searchAuthorId))
                            bookService.ShowSearchResults(bookService.SearchByAuthor(searchAuthorId), $"автор ID={searchAuthorId}");
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "8":
                        genreService.Show();
                        Console.Write("ID жанру: ");
                        if (int.TryParse(Console.ReadLine(), out int searchGenreId))
                            bookService.ShowSearchResults(bookService.SearchByGenre(searchGenreId), $"жанр ID={searchGenreId}");
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "9":
                        bookService.ShowNewest();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "10":
                        bookService.ShowPopular();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "11":
                        Console.Write("ID книги для видалення: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                            bookService.DeleteBook(deleteId);
                        else
                            Console.WriteLine("Невірний ID");
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "12":
                        EditBookMenu(bookService, authorService, publisherService, genreService);
                        break;

                    case "13":
                        SellBookMenu(bookService, customerService);
                        break;

                    case "14":
                        Console.Write("ID книги для списання: ");
                        int writeOffId = int.TryParse(Console.ReadLine(), out int woi) ? woi : 0;
                        Console.Write("Кількість для списання: ");
                        int writeOffQty = int.TryParse(Console.ReadLine(), out int woq) ? woq : 1;
                        bookService.WriteOffBook(writeOffId, writeOffQty);
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "15":
                        PromotionMenu(promotionService, bookService);
                        break;

                    case "16":
                        ReserveBookMenu(bookService, customerService);
                        break;

                    case "17":
                        customerService.Show();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;
                    case "18":
                        statisticsService.ShowPopularBooks(SelectPeriod());
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "19":
                        statisticsService.ShowPopularAuthors(SelectPeriod());
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "20":
                        statisticsService.ShowPopularGenres(SelectPeriod());
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;



                    default:
                        Console.WriteLine("Невірний вибір!");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static DateTime EnterDateTime(bool allowNow = false)
        {
            if (allowNow)
            {
                Console.WriteLine("1. Зараз");
                Console.WriteLine("2. Вказати час");
                Console.Write("Вибір: ");
                if (Console.ReadLine() == "1")
                    return DateTime.Now;
            }

            Console.Write("Рік (наприклад 2026): ");
            int year = int.TryParse(Console.ReadLine(), out int y) ? y : DateTime.Now.Year;

            Console.Write("Місяць (1-12): ");
            int month = int.TryParse(Console.ReadLine(), out int mo) ? mo : DateTime.Now.Month;

            Console.Write("День (1-31): ");
            int day = int.TryParse(Console.ReadLine(), out int dd) ? dd : DateTime.Now.Day;

            Console.Write("Година (0-23): ");
            int hour = int.TryParse(Console.ReadLine(), out int h) ? h : 0;

            Console.Write("Хвилина (0-59): ");
            int minute = int.TryParse(Console.ReadLine(), out int min) ? min : 0;

            return new DateTime(year, month, day, hour, minute, 0);
        }

        static void PromotionMenu(PromotionService promotionService, BookService bookService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Управління акціями ===");
                Console.WriteLine("1. Всі акції");
                Console.WriteLine("2. Створити акцію");
                Console.WriteLine("3. Додати книгу до акції");
                Console.WriteLine("4. Видалити книгу з акції");
                Console.WriteLine("0. Назад");
                Console.Write("Вибір: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "0":
                        return;

                    case "1":
                        promotionService.ShowAll();
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "2":
                        Console.Clear();
                        Console.WriteLine("=== Створення акції ===");

                        Console.Write("Назва акції: ");
                        string name = Console.ReadLine() ?? "";

                        Console.Write("Знижка %: ");
                        decimal discount = decimal.TryParse(Console.ReadLine(), out decimal d) ? d : 10m;

                        Console.WriteLine("\nДата початку акції:");
                        DateTime startDate = EnterDateTime(allowNow: true);

                        Console.WriteLine("\nДата кінця акції:");
                        DateTime endDate = EnterDateTime(allowNow: false);

                        if (endDate <= startDate)
                        {
                            Console.WriteLine("Помилка: дата кінця має бути пізніше дати початку!");
                            Console.ReadKey();
                            break;
                        }

                        promotionService.CreatePromotion(name, discount, startDate, endDate);
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "3":
                        Console.Clear();
                        promotionService.ShowAll();
                        Console.Write("ID акції: ");
                        int promoId = int.TryParse(Console.ReadLine(), out int pi) ? pi : 0;
                        bookService.ShowAllBooks();
                        Console.Write("ID книги: ");
                        int bookId = int.TryParse(Console.ReadLine(), out int bi) ? bi : 0;
                        promotionService.AddBookToPromotion(promoId, bookId);
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    case "4":
                        Console.Clear();
                        promotionService.ShowAll();
                        Console.Write("ID акції: ");
                        int removePromoId = int.TryParse(Console.ReadLine(), out int rpi) ? rpi : 0;
                        Console.Write("ID книги: ");
                        int removeBookId = int.TryParse(Console.ReadLine(), out int rbi) ? rbi : 0;
                        promotionService.RemoveBookFromPromotion(removePromoId, removeBookId);
                        Console.WriteLine("Натисніть...");
                        Console.ReadKey();
                        break;

                    default:
                        Console.WriteLine("Невірний вибір!");
                        Console.ReadKey();
                        break;
                }
            }
        }
        static string SelectPeriod()
        {
            Console.WriteLine("\nОберіть період:");
            Console.WriteLine("1. День");
            Console.WriteLine("2. Тиждень");
            Console.WriteLine("3. Місяць");
            Console.WriteLine("4. Рік");
            Console.Write("Вибір: ");

            return Console.ReadLine() switch
            {
                "1" => "день",
                "2" => "тиждень",
                "3" => "місяць",
                "4" => "рік",
                _ => "день"
            };
        }
        static bool IsValidUser(AppDbContext db, string login, string password)
        {
            return db.Users.Any(u => u.Login == login && u.PasswordHash == password);
        }

        static int GetOrCreateAuthor(AuthorService authorService)
        {
            authorService.Show();
            Console.Write("ID автора (або 0 щоб створити нового): ");
            int id = int.TryParse(Console.ReadLine(), out int a) ? a : 0;
            if (id == 0)
            {
                Console.Write("ПІБ нового автора: ");
                string name = Console.ReadLine() ?? "";
                id = authorService.AddAndReturnId(name);
            }
            return id;
        }

        static int GetOrCreatePublisher(PublisherService publisherService)
        {
            publisherService.Show();
            Console.Write("ID видавництва (або 0 щоб створити нове): ");
            int id = int.TryParse(Console.ReadLine(), out int p) ? p : 0;
            if (id == 0)
            {
                Console.Write("Назва нового видавництва: ");
                string name = Console.ReadLine() ?? "";
                id = publisherService.AddAndReturnId(name);
            }
            return id;
        }

        static int GetOrCreateGenre(GenreService genreService)
        {
            genreService.Show();
            Console.Write("ID жанру (або 0 щоб створити новий): ");
            int id = int.TryParse(Console.ReadLine(), out int g) ? g : 0;
            if (id == 0)
            {
                Console.Write("Назва нового жанру: ");
                string name = Console.ReadLine() ?? "";
                id = genreService.AddAndReturnId(name);
            }
            return id;
        }

        static int GetOrCreateCustomer(CustomerService customerService)
        {
            customerService.Show();
            Console.Write("ID покупця (або 0 щоб створити нового): ");
            int id = int.TryParse(Console.ReadLine(), out int c) ? c : 0;
            if (id == 0)
            {
                Console.Write("ПІБ покупця: ");
                string name = Console.ReadLine() ?? "";
                Console.Write("Телефон (Enter - пропустити): ");
                string? phone = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(phone)) phone = null;
                Console.Write("Email (Enter - пропустити): ");
                string? email = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(email)) email = null;
                id = customerService.AddAndReturnId(name, phone, email);
            }
            return id;
        }

        static void AddBookMenu(BookService bookService, AuthorService authorService,
                               PublisherService publisherService, GenreService genreService)
        {
            Console.Clear();
            Console.WriteLine("=== Додати книгу ===");

            int authorId = GetOrCreateAuthor(authorService);
            int publisherId = GetOrCreatePublisher(publisherService);
            int genreId = GetOrCreateGenre(genreService);

            Console.Write("Назва: ");
            string title = Console.ReadLine() ?? "";

            Console.Write("Сторінок: ");
            int pages = int.TryParse(Console.ReadLine(), out int pg) ? pg : 0;

            Console.Write("Рік: ");
            int year = int.TryParse(Console.ReadLine(), out int yr) ? yr : 0;

            Console.Write("Собівартість: ");
            decimal cost = decimal.TryParse(Console.ReadLine(), out decimal c) ? c : 0m;

            Console.Write("Ціна продажу: ");
            decimal sell = decimal.TryParse(Console.ReadLine(), out decimal s) ? s : 0m;

            Console.Write("Кількість екземплярів: ");
            int quantity = int.TryParse(Console.ReadLine(), out int qt) ? qt : 0;

            Console.Write("Продовження (так/ні): ");
            bool isSequel = (Console.ReadLine() ?? "").ToLower() == "так";
            int? prevId = null;
            if (isSequel)
            {
                Console.Write("ID попередньої книги: ");
                prevId = int.TryParse(Console.ReadLine(), out int pid) ? pid : null;
            }

            bookService.AddBook(title, authorId, publisherId, genreId, pages, year, cost, sell, quantity, isSequel, prevId);
            Console.WriteLine("Натисніть...");
            Console.ReadKey();
        }

        static void EditBookMenu(BookService bookService, AuthorService authorService,
                                PublisherService publisherService, GenreService genreService)
        {
            Console.Clear();
            Console.WriteLine("=== Редагування книги ===");
            bookService.ShowAllBooks();

            Console.Write("ID книги для редагування: ");
            if (!int.TryParse(Console.ReadLine(), out int editId))
            {
                Console.WriteLine("Невірний ID");
                Console.ReadKey();
                return;
            }

            Console.Write("Нова назва (Enter - пропустити): ");
            string? newTitle = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newTitle)) newTitle = null;

            Console.Write("Змінити автора? (так/ні): ");
            int? newAuthorId = null;
            if ((Console.ReadLine() ?? "").ToLower() == "так")
                newAuthorId = GetOrCreateAuthor(authorService);

            Console.Write("Змінити видавництво? (так/ні): ");
            int? newPublisherId = null;
            if ((Console.ReadLine() ?? "").ToLower() == "так")
                newPublisherId = GetOrCreatePublisher(publisherService);

            Console.Write("Змінити жанр? (так/ні): ");
            int? newGenreId = null;
            if ((Console.ReadLine() ?? "").ToLower() == "так")
                newGenreId = GetOrCreateGenre(genreService);

            Console.Write("Нова ціна продажу (Enter - пропустити): ");
            string? priceInput = Console.ReadLine();
            decimal? newPrice = decimal.TryParse(priceInput, out decimal nsp) ? nsp : null;

            Console.Write("Нова собівартість (Enter - пропустити): ");
            string? costInput = Console.ReadLine();
            decimal? newCost = decimal.TryParse(costInput, out decimal ncp) ? ncp : null;

            Console.Write("Нова кількість сторінок (Enter - пропустити): ");
            string? pagesInput = Console.ReadLine();
            int? newPages = int.TryParse(pagesInput, out int npg) ? npg : null;

            Console.Write("Новий рік (Enter - пропустити): ");
            string? yearInput = Console.ReadLine();
            int? newYear = int.TryParse(yearInput, out int ny) ? ny : null;

            Console.Write("Нова кількість екземплярів (Enter - пропустити): ");
            string? quantityInput = Console.ReadLine();
            int? newQuantity = int.TryParse(quantityInput, out int nq) ? nq : null;

            bookService.UpdateBook(editId, newTitle, newPrice, newPages, newAuthorId, newPublisherId, newGenreId, newYear, newCost, newQuantity, null, null);
            Console.WriteLine("Натисніть...");
            Console.ReadKey();
        }

        static void SellBookMenu(BookService bookService, CustomerService customerService)
        {
            Console.Clear();
            Console.WriteLine("=== Продаж книги ===");
            bookService.ShowAllBooks();

            Console.Write("ID книги для продажу: ");
            int bookId = int.TryParse(Console.ReadLine(), out int bi) ? bi : 0;

            Console.Write("Кількість: ");
            int qty = int.TryParse(Console.ReadLine(), out int q) ? q : 1;

            int customerId = GetOrCreateCustomer(customerService);

            bookService.SellBook(bookId, qty, customerId);
            Console.WriteLine("Натисніть...");
            Console.ReadKey();
        }

        static void ReserveBookMenu(BookService bookService, CustomerService customerService)
        {
            Console.Clear();
            Console.WriteLine("=== Резерв книги ===");
            bookService.ShowAllBooks();

            Console.Write("ID книги для резерву: ");
            int bookId = int.TryParse(Console.ReadLine(), out int bi) ? bi : 0;

            int customerId = GetOrCreateCustomer(customerService);

            bookService.ReserveBook(bookId, customerId);
            Console.WriteLine("Натисніть...");
            Console.ReadKey();
        }
    }
}
