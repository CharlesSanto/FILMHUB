using FILMHUB.Data;
using FILMHUB.DTO;
using FILMHUB.Helpers;
using FILMHUB.Models;
using FILMHUB.Services.Interfaces;

namespace FILMHUB.Services;

public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    
    public AuthService(ApplicationDbContext context)
    {
        _context = context;
    }

    public bool EmailExists(string email)
    {
            return _context.Users.Any(e => e.Email == email);
    }

    public void CreateUser(RegisterDto registerDto)
    {
        User user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = PassowordHelper.HashPassword(registerDto.Password),
            CreatedAt =  DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        _context.Users.Add(user);
        _context.SaveChanges();
    }
    
    public User ValidateUser(string email, string password)
    {
        var user = _context.Users.FirstOrDefault(e => e.Email == email);

        if (user == null) return null;
        if (!PassowordHelper.VerifyPassword(password, user.PasswordHash)) return null;
        
        return user;
    }

    public void UpdateUser(int userId, string? name, string? email)
    {
        var user =  _context.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null) return;
        
        if (!string.IsNullOrWhiteSpace(name))
            user.Name = name;
        if (!string.IsNullOrWhiteSpace(email))
            user.Email = email;
        
        _context.SaveChanges();
    }
}