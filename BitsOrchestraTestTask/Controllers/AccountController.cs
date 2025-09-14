using BitsOrchestraTestTask.Data;
using BitsOrchestraTestTask.Models.Entities;
using BitsOrchestraTestTask.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BitsOrchestraTestTask.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly JwtService _jwtService;

    public AccountController(ApplicationDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var hash = _jwtService.HashPassword(password);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.PasswordHash == hash);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid login attempt");
            return View();
        }

        var token = _jwtService.GenerateToken(user.Id, user.Email, user.DisplayName);

        Response.Cookies.Append("jwt", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict
        });

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(string email, string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            ModelState.AddModelError("", "Name is required");
            return View();
        }

        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError("", "User already exists");
            return View();
        }

        var user = new User
        {
            Email = email,
            DisplayName = username,
            PasswordHash = _jwtService.HashPassword(password)
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return RedirectToAction("Login");
    }


    [HttpGet]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Index", "Home");
    }
}