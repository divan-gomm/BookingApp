using BookingApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingApp.Services
{
    public class CurrentUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;

        public CurrentUserService(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<ApplicationUser?> GetUserAsync()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

            return user;
        }

        public async Task<int?> GetBusinessIdAsync()
        {
            var user = await GetUserAsync();

            if (user == null)
                return null;

            var business = await _context.Businesses
                .FirstOrDefaultAsync(b => b.OwnerUserId == user.Id);

            return business?.ID;
        }
    }
}