using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BitsOrchestraTestTask.Data;
using BitsOrchestraTestTask.Helpers;
using BitsOrchestraTestTask.Models.Dtos;
using BitsOrchestraTestTask.Models.Entities;
using BitsOrchestraTestTask.Services;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitsOrchestraTestTask.Controllers;

[Authorize]
public class ContactsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IContactValidator _contactValidator;
    private readonly ICsvImportService _csvImportService;

    public ContactsController(ApplicationDbContext context, ICsvImportService csvImportService, IContactValidator contactValidator)
    {
        _context = context;
        _contactValidator = contactValidator;
        _csvImportService = csvImportService;
    }

    private Guid GetCurrentUserId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                   User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    public async Task<IActionResult> Index()
    {
        var userId = GetCurrentUserId();
        var contacts = await _context.Contacts.Where(c => c.UserId == userId).ToListAsync();
        return View(contacts);
    }

    [HttpPost]
    public async Task<IActionResult> UploadCsv(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["Error"] = "Please select a CSV file.";
            return RedirectToAction("Index");
        }
        
        var userId = GetCurrentUserId();
        var result = await _csvImportService.ImportContactsAsync(file, userId);

        TempData["Success"] = $"Uploaded {result.SuccessCount} contacts. Skipped {result.ErrorCount}.";
        if (result.Errors.Any())
            TempData["CsvErrors"] = string.Join(" | ", result.Errors.Take(20));

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Update(Guid id, string field, string value)
    {
        var userId = GetCurrentUserId();
        var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (contact == null) return NotFound();
        
        var validation = _contactValidator.ValidateField(field, value);
        if (!validation.IsValid)
            return BadRequest(new { success = false, error = string.Join(", ", validation.Errors) });
        
        switch (field)
        {
            case "Name": contact.Name = value; break;
            case "Phone": contact.Phone = value; break;
            case "Salary": contact.Salary = decimal.Parse(value); break;
            case "DateOfBirth": contact.DateOfBirth = DateTime.Parse(value); break;
            case "Married": contact.Married = bool.Parse(value); break;
        }

        await _context.SaveChangesAsync();
        return Ok(new { success = true });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = GetCurrentUserId();
        var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (contact == null) return NotFound();

        _context.Contacts.Remove(contact);
        await _context.SaveChangesAsync();
        return Ok(new { success = true });
    }
}