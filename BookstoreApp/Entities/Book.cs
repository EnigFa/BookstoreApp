using System.ComponentModel.DataAnnotations;

namespace BookstoreApp.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(300)]
        public string Title { get; set; } = string.Empty;

        public int AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; } = null!;

        public int GenreId { get; set; }
        public Genre Genre { get; set; } = null!;

        public int Pages { get; set; }
        public int Year { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SellPrice { get; set; }
        public int Quantity { get; set; }
        public bool IsSequel { get; set; }
        public int? PreviousBookId { get; set; }
        public Book? PreviousBook { get; set; }

        public List<PromotionBook> PromotionBooks { get; set; } = new();
        public List<CustomerReservation> Reservations { get; set; } = new();
    }
}
