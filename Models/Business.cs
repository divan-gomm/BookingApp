namespace BookingApp.Models
{
    public class Business
    {
        public int ID { get; set; }

        public string OwnerUserId { get; set; } 

        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
    }
}