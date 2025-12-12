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
    
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]  
    public IActionResult CreateUser(RegisterDto registerDto)
    {
        if (!ValidateEmailHelper.IsValidEmail(registerDto.Email))
            return BadRequest(new {message = "Invalid email."});

        if (_authService.EmailExists(registerDto.Email))
            return BadRequest(new { message = "Email already exists." });
        
        if (registerDto.Password != registerDto.ConfirmPassword)
            return BadRequest(new { message = "Passwords do not match." });
        
        _authService.CreateUser(registerDto);
        return Ok(new { message = "User created successfully." });
    }
}