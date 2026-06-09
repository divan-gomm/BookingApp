using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BookingApp.Models;
using BookingApp.Data;
using BookingApp.Services;

[Authorize]
public class StaffsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CurrentUserService _currentUserService;
    private readonly UserManager<ApplicationUser> _userManager;

    public StaffsController(
        ApplicationDbContext context,
        CurrentUserService currentUserService,
        UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _currentUserService = currentUserService;
        _userManager = userManager;
    }

    // GET: STAFFS
    public async Task<IActionResult> Index()
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        // ADMIN: sees everything
        if (await _userManager.IsInRoleAsync(user, "Admin"))
        {
            return View(await _context.StaffMember.ToListAsync());
        }

        // BUSINESS OWNER / STAFF: sees own business staff
        if (user.BusinessID != null)
        {
            var staff = await _context.StaffMember
                .Where(s => s.BusinessID == user.BusinessID.Value)
                .ToListAsync();

            return View(staff);
        }

        return Forbid();
    }

    // GET: STAFFS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var staff = await _context.StaffMember.FirstOrDefaultAsync(s => s.ID == id);

        if (staff == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(staff);

        if (user?.BusinessID != null && user.BusinessID == staff.BusinessID)
            return View(staff);

        return Forbid();
    }

    // GET: STAFFS/Create
    public async Task<IActionResult> Create()
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null || user.BusinessID == null)
            return Forbid();

        return View();
    }

    // POST: STAFFS/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Staff staff)
    {
        var user = await _currentUserService.GetUserAsync();

        if (user == null || user.BusinessID == null)
            return Forbid();

        if (!ModelState.IsValid)
            return View(staff);

        staff.BusinessID = user.BusinessID.Value;

        _context.Add(staff);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: STAFFS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var staff = await _context.StaffMember.FindAsync(id);

        if (staff == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(staff);

        if (user?.BusinessID != null && user.BusinessID == staff.BusinessID)
            return View(staff);

        return Forbid();
    }

    // POST: STAFFS/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Staff staff)
    {
        if (id != staff.ID)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (user == null)
            return RedirectToAction("Index", "Home");

        var existing = await _context.StaffMember.FindAsync(id);

        if (existing == null)
            return NotFound();

        if (!await _userManager.IsInRoleAsync(user, "Admin") &&
            user.BusinessID != existing.BusinessID)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
            return View(staff);

        existing.Name = staff.Name;
        existing.Surname = staff.Surname;
        existing.PhoneNumber = staff.PhoneNumber;
        existing.Email = staff.Email;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: STAFFS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var staff = await _context.StaffMember.FirstOrDefaultAsync(s => s.ID == id);

        if (staff == null)
            return NotFound();

        var user = await _currentUserService.GetUserAsync();

        if (await _userManager.IsInRoleAsync(user, "Admin"))
            return View(staff);

        if (user?.BusinessID != null && user.BusinessID == staff.BusinessID)
            return View(staff);

        return Forbid();
    }

    // POST: STAFFS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var staff = await _context.StaffMember.FindAsync(id);

        if (staff != null)
        {
            _context.StaffMember.Remove(staff);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool StaffExists(int id)
    {
        return _context.StaffMember.Any(e => e.ID == id);
    }
}