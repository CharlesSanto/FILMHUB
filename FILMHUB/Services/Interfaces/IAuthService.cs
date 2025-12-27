using FILMHUB.DTO;
using FILMHUB.Models;

namespace FILMHUB.Services.Interfaces;

public interface IAuthService
{
    bool EmailExists(string email);
    void CreateUser(RegisterDto registerDto);
    User ValidateUser(string email,  string password);
    void UpdateUser(int userId, string? name, string? email);
}