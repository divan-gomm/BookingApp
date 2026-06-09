using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingApp.Models;
using BookingApp.Data;
using BookingApp.Services;
using Microsoft.AspNetCore.Identity;

public class BusinessesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public BusinessesController(
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    // GET: BUSINESSES
    public async Task<IActionResult> Index()
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        // If Admin → see all businesses
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            var allBusinesses = await _context.Businesses.ToListAsync();
            return View(allBusinesses);
        }

        // BusinessOwner → see only their business
        if (user.BusinessID != null)
        {
            var business = await _context.Businesses
                .Where(b => b.ID == user.BusinessID.Value)
                .ToListAsync();

            return View(business);
        }

        return Forbid();
    }

    // GET: BUSINESSES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        var business = await _context.Businesses
            .FirstOrDefaultAsync(b => b.ID == id);

        if (business == null)
            return NotFound();

        // Admin can view all
        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(business);

        // BusinessOwner can only view own
        if (user.BusinessID == business.ID)
            return View(business);

        return Forbid();
    }

    // GET: BUSINESSES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: BUSINESSES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Business business)
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        if (ModelState.IsValid)
        {
            _context.Add(business);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(business);
    }

    // GET: BUSINESSES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var business = await _context.Businesses.FindAsync(id);

        if (business == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(business);

        if (user.BusinessID == business.ID)
            return View(business);

        return Forbid();
    }

    // POST: BUSINESSES/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Business business)
    {
        if (id != business.ID)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        var existing = await _context.Businesses.FindAsync(id);

        if (existing == null)
            return NotFound();

        if (!await _userManager.IsInRoleAsync(user, "Admin") &&
            user.BusinessID != existing.ID)
        {
            return Forbid();
        }

        if (ModelState.IsValid)
        {
            existing.Name = business.Name;
            existing.PhoneNumber = business.PhoneNumber;
            existing.Email = business.Email;
            existing.Address = business.Address;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(business);
    }

    // GET: BUSINESSES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var business = await _context.Businesses.FirstOrDefaultAsync(b => b.ID == id);

        if (business == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(business);

        return Forbid();
    }

    // POST: BUSINESSES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var business = await _context.Businesses.FindAsync(id);

        if (business != null)
        {
            _context.Businesses.Remove(business);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool BusinessExists(int id)
    {
        return _context.Businesses.Any(e => e.ID == id);
    }
}