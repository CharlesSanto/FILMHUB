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
        return RedirectToAction("Index", "Home");
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
    
    [HttpGet]
    public IActionResult Settings()
    {
        int? userIdSession = HttpContext.Session.GetInt32("UserId");
        if (userIdSession == null)
        {
            return RedirectToAction("Index", "Home");
        }
        
        var user = _authService.GetUserById(userIdSession.Value);
        
        if(!string.IsNullOrEmpty(user.Name))
            HttpContext.Session.SetString("UserName", user.Name);
        
        return View();
    }
    
    [HttpPost]
    public IActionResult UpdateUser(UpdateUserDto updateUserDto)
    {
        int? userIdSession = HttpContext.Session.GetInt32("UserId");
        if (userIdSession == null)
        {
            return RedirectToAction("Login", "Auth");
        }
        int userId = (int)userIdSession;
        
        if (!string.IsNullOrWhiteSpace(updateUserDto.Name) || !string.IsNullOrWhiteSpace(updateUserDto.Email))
            _authService.UpdateUser(userId, updateUserDto.Name, updateUserDto.Email);
        
        return RedirectToAction("Settings", "Auth");
    }

    public IActionResult ChangePassword(UpdateUserDto updateUserDto)
    {
        int? userIdSession = HttpContext.Session.GetInt32("UserId");
        if (userIdSession == null) return RedirectToAction("Index", "Home");
        
        var user = _authService.GetUserById((int)userIdSession);
        
        if (!PassowordHelper.VerifyPassword(updateUserDto.CurrentPassword, user.PasswordHash))
            ModelState.AddModelError("CurrentPassword", "Senha atual incorreta.");
        
        if (!ModelState.IsValid) return View("Settings", updateUserDto);
        
        _authService.ChangePassword(user.Id, updateUserDto.Password);
        
        return RedirectToAction("Settings", "Auth");
    }
    
    [HttpPost]
    public IActionResult DeleteUser()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        
        if (userId == null) return RedirectToAction("Index", "Home");
        
        _authService.DeleteUser((int)userId);
        
        HttpContext.Session.Clear();
        
        return RedirectToAction("Index", "Home");
    }
}