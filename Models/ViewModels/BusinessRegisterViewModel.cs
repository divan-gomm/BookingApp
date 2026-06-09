using System.ComponentModel.DataAnnotations;

namespace BookingApp.Models.ViewModels
{
    public class BusinessRegisterViewModel
    {
        //User
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        //Business
        [Required]
        public string BusinessName  { get; set; }
        
        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string BusinessEmail { get; set; }

        [Required]
        public string Address { get; set; }
        





    }
}
