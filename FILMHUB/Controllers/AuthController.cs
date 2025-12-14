using FILMHUB.Data;
using FILMHUB.DTO;
using FILMHUB.Helpers;
using FILMHUB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FILMHUB.Controllers;

public class AuthController : Controller
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Login(LoginDto loginDto)
    {
        var user = _authService.ValidateUser(loginDto.Email,  loginDto.Password);

        if (user == null)
        {
            ModelState.AddModelError("Email", "Email ou senha incorretos.");
            return View(loginDto);
        }
        
        HttpContext.Session.SetInt32("UserId", user.Id);
        HttpContext.Session.SetString("UserName", user.Name);
        
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login", "Auth");
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]  
    public IActionResult Register(RegisterDto registerDto)
    {
        if (_authService.EmailExists(registerDto.Email))
            ModelState.AddModelError("Email", "Este e-mail já está cadastrado. Tente outro ou faça login.");
        
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }
        
        _authService.CreateUser(registerDto);
        // return Ok(new { message = "User created successfully." });
        return RedirectToAction("Login", "Auth");
    }
}