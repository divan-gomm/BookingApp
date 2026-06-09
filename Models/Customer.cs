namespace BookingApp.Models
{
    public class Customer
    {
        public int ID { get; set; }

        public string UserId { get; set; } 

        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }

        public int? BusinessID { get; set; }

        // Navigation property for the related Business
        public Business? Business { get; set; }
    }
}