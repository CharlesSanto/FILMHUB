using System.ComponentModel.DataAnnotations;

namespace FILMHUB.DTO;

public class UpdateUserDto
{
    public string ? Name { get; set; }
    
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
    
    [MinLength(6, ErrorMessage = "A senha deve ter no minímo 6 caracteres")]
    public string? Passowrd { get; set; }
    
    [Compare("Password",ErrorMessage ="As senhas não coincidem")]
    public  string? ConfirmPassword { get; set; }
}