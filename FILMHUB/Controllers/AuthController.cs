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
    public IActionResult Register()
    {
        return View();
    }
    [HttpPost]  
    public IActionResult Register(RegisterDto registerDto)
    {
        if (!ValidateEmailHelper.IsValidEmail(registerDto.Email))
            return BadRequest(new {message = "Invalid email."});

        if (_authService.EmailExists(registerDto.Email))
            ModelState.AddModelError("Email", "Este e-mail já está cadastrado. Tente outro ou faça login.");
        
        if (registerDto.Password != registerDto.ConfirmPassword)
            return BadRequest(new { message = "Passwords do not match." });
        
        if (!ModelState.IsValid)
        {
            return View(registerDto);
        }
        
        _authService.CreateUser(registerDto);
        // return Ok(new { message = "User created successfully." });
        return RedirectToAction("Index", "Home");
    }
}