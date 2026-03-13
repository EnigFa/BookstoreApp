using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace BookstoreApp.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string PasswordHash { get; set; } = string.Empty;  

        public string Role { get; set; } = "User";

        public List<CustomerReservation> Reservations { get; set; } = new();
    }
}
