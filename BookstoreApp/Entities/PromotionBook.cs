using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookstoreApp.Entities
{
    public class PromotionBook
    {
        public int PromotionId { get; set; }
        public Promotion Promotion { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        public decimal OriginalPrice { get; set; }
    }
}
