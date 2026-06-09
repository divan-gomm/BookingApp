using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookingApp.Models
{
    public class Booking
    {
        public int ID { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }

        // Foreign keys
        public int? CustomerID { get; set; }
        public int? ServiceID { get; set; }
        public int? StaffID { get; set; }
        public int? BusinessID { get; set; }

        // Navigation properties (THIS FIXES YOUR ERROR)
        public Customer Customer { get; set; }
        public Service Service { get; set; }
        public Staff Staff { get; set; }
        public Business Business { get; set; }
    }
}