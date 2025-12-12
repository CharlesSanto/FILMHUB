using FILMHUB.Data;
using FILMHUB.DTO;
using FILMHUB.Helpers;
using FILMHUB.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FILMHUB.Controllers;


[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]  
    public IActionResult CreateUser(RegisterDto registerDto)
    {
        if (!ValidateEmailHelper.IsValidEmail(registerDto.Email))
            return BadRequest(new {message = "Invalid email."});

        if (_authService.EmailExists(registerDto.Email))
            return BadRequest(new { message = "Email already exists." });
        
        _authService.CreateUser(registerDto);
        return Ok(new { message = "User created successfully." });
    }
}