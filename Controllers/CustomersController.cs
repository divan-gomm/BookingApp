using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingApp.Models;
using BookingApp.Data;
using BookingApp.Services;
using Microsoft.AspNetCore.Identity;

public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomersController(
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    // GET: CUSTOMERS
    public async Task<IActionResult> Index()
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        // Admin sees all customers
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return View(await _context.Customers.ToListAsync());
        }

        // BusinessOwner sees only their customers
        if (user.BusinessID != null)
        {
            var customers = await _context.Customers
                .Where(c => c.BusinessID == user.BusinessID.Value)
                .ToListAsync();

            return View(customers);
        }

        return Forbid();
    }

    // GET: CUSTOMERS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.ID == id);

        if (customer == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(customer);

        if (user.BusinessID == customer.BusinessID)
            return View(customer);

        return Forbid();
    }

    // GET: CUSTOMERS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: CUSTOMERS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            customer.BusinessID = user.BusinessID.Value;

            _context.Add(customer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    // GET: CUSTOMERS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var customer = await _context.Customers.FindAsync(id);

        if (customer == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(customer);

        if (user.BusinessID == customer.BusinessID)
            return View(customer);

        return Forbid();
    }

    // POST: CUSTOMERS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer customer)
    {
        if (id != customer.ID)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        var existing = await _context.Customers.FindAsync(id);

        if (existing == null)
            return NotFound();

        if (!await _userManager.IsInRoleAsync(user, "Admin") &&
            user.BusinessID != existing.BusinessID)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            existing.Name = customer.Name;
            existing.Surname = customer.Surname;
            existing.PhoneNumber = customer.PhoneNumber;
            existing.Email = customer.Email;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(customer);
    }

    // GET: CUSTOMERS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.ID == id);

        if (customer == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(customer);

        if (user.BusinessID == customer.BusinessID)
            return View(customer);

        return Forbid();
    }

    // POST: CUSTOMERS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var customer = await _context.Customers.FindAsync(id);

        if (customer != null)
        {
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CustomerExists(int id)
    {
        return _context.Customers.Any(e => e.ID == id);
    }
}