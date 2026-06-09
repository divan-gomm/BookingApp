using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookingApp.Models;
using BookingApp.Data;
using BookingApp.Services;

public class ServicesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CurrentUserService _currentUserService;

    public ServicesController(
        ApplicationDbContext context,
        CurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    // GET: SERVICES
    public async Task<IActionResult> Index()
    {
        var businessId = await _currentUserService.GetBusinessIdAsync();

        var services = await _context.Services
            .Where(s => s.BusinessID == businessId)
            .ToListAsync();

        return View(services);
    }

    // GET: SERVICES/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var businessId = await _currentUserService.GetBusinessIdAsync();

        var service = await _context.Services
            .FirstOrDefaultAsync(m => m.ID == id && m.BusinessID == businessId);

        if (service == null)
            return NotFound();

        return View(service);
    }

    // GET: SERVICES/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SERVICES/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Service service)
    {
        var businessId = await _currentUserService.GetBusinessIdAsync();

        if (businessId == null)
        {
            return Forbid(); // or Unauthorized()
        }

        var data = _context.Services
            .Where(x => x.BusinessID == businessId.Value)
            .ToList();

        return View(service);
    }

    // GET: SERVICES/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var businessId = await _currentUserService.GetBusinessIdAsync();

        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.ID == id && s.BusinessID == businessId);

        if (service == null)
            return NotFound();

        return View(service);
    }

    // POST: SERVICES/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Service service)
    {
        var businessId = await _currentUserService.GetBusinessIdAsync();

        if (id != service.ID)
            return NotFound();

        if (!ModelState.IsValid)
            return View(service);

        var existing = await _context.Services
            .FirstOrDefaultAsync(s => s.ID == id && s.BusinessID == businessId);

        if (existing == null)
            return NotFound();

        existing.Name = service.Name;
        existing.Price = service.Price;
        existing.DurationMinutes = service.DurationMinutes;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: SERVICES/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var businessId = await _currentUserService.GetBusinessIdAsync();

        var service = await _context.Services
            .FirstOrDefaultAsync(m => m.ID == id && m.BusinessID == businessId);

        if (service == null)
            return NotFound();

        return View(service);
    }

    // POST: SERVICES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var businessId = await _currentUserService.GetBusinessIdAsync();

        var service = await _context.Services
            .FirstOrDefaultAsync(s => s.ID == id && s.BusinessID == businessId);

        if (service != null)
        {
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool ServiceExists(int id)
    {
        return _context.Services.Any(e => e.ID == id);
    }
}