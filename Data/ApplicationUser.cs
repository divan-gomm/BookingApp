using BookingApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BookingApp.Data
{
    public class ApplicationUser : IdentityUser
    {
        public int? BusinessID { get; set; }

        public string? FullName { get; set; }

        public Business? Business { get; set; }

    }
}
