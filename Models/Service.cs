namespace BookingApp.Models
{
    public class Service
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int DurationMinutes { get; set; }
        //FK
        public int BusinessID { get; set; }
        //Navigation Property -identifies relationship
        public Business Business { get; set; }

    }
}
