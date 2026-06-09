using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingApp.Models;
using BookingApp.Data;
using BookingApp.Services;

public class BookingsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CurrentUserService _currentUserService;

    public BookingsController(
        ApplicationDbContext context,
        CurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    // GET: BOOKINGS
    public async Task<IActionResult> Index()
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        var businessId = await _currentUserService.GetBusinessIdAsync();

        // =========================
        // BUSINESS OWNER / STAFF
        // =========================
        if (businessId != null)
        {
            var bookings = await _context.Bookings
                .Where(b => b.BusinessID == businessId)
                .Include(b => b.Customer)
                .Include(b => b.Service)
                .Include(b => b.Staff)
                .ToListAsync();

            return View(bookings);
        }

        // =========================
        // CUSTOMER
        // =========================
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (customer == null)
            return View(new List<Booking>());

        var customerBookings = await _context.Bookings
            .Where(b => b.CustomerID == customer.ID)
            .Include(b => b.Service)
            .Include(b => b.Staff)
            .ToListAsync();

        return View(customerBookings);
    }

    // GET: DETAILS
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();
        var businessId = await _currentUserService.GetBusinessIdAsync();

        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Service)
            .Include(b => b.Staff)
            .FirstOrDefaultAsync(b => b.ID == id);

        if (booking == null)
            return NotFound();

        // BUSINESS ACCESS
        if (businessId != null && booking.BusinessID == businessId)
            return View(booking);

        // CUSTOMER ACCESS
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (customer != null && booking.CustomerID == customer.ID)
            return View(booking);

        return Forbid();
    }

    // GET: CREATE
    public IActionResult Create()
    {
        return View();
    }

    // POST: CREATE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Booking booking)
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        var businessId = await _currentUserService.GetBusinessIdAsync();

        if (!ModelState.IsValid)
            return View(booking);

        // BUSINESS CREATED BOOKING
        if (businessId != null)
        {
            booking.BusinessID = businessId.Value;
        }

        // CUSTOMER CREATED BOOKING
        else
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (customer == null)
                return Forbid();

            booking.CustomerID = customer.ID;
        }

        _context.Add(booking);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: EDIT
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var booking = await _context.Bookings.FindAsync(id);

        if (booking == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();
        var businessId = await _currentUserService.GetBusinessIdAsync();

        if (businessId != null && booking.BusinessID == businessId)
            return View(booking);

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (customer != null && booking.CustomerID == customer.ID)
            return View(booking);

        return Forbid();
    }

    // POST: EDIT
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Booking booking)
    {
        if (id != booking.ID)
            return NotFound();

        var existing = await _context.Bookings.FindAsync(id);

        if (existing == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();
        var businessId = await _currentUserService.GetBusinessIdAsync();

        if (businessId != null && existing.BusinessID != businessId)
            return Forbid();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.UserId == user.Id);

        if (customer != null && existing.CustomerID != customer.ID)
            return Forbid();

        if (!ModelState.IsValid)
            return View(booking);

        existing.StartTime = booking.StartTime;
        existing.EndTime = booking.EndTime;
        existing.Status = booking.Status;
        existing.CustomerID = booking.CustomerID;
        existing.ServiceID = booking.ServiceID;
        existing.StaffID = booking.StaffID;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // DELETE
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var booking = await _context.Bookings
            .Include(b => b.Customer)
            .Include(b => b.Service)
            .Include(b => b.Staff)
            .FirstOrDefaultAsync(b => b.ID == id);

        if (booking == null)
            return NotFound();

        return View(booking);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var booking = await _context.Bookings.FindAsync(id);

        if (booking != null)
        {
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}