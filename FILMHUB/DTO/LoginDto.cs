using System.ComponentModel.DataAnnotations;

namespace FILMHUB.DTO;

public class LoginDto
{
    [Required(ErrorMessage = "Informe o email")]
    [EmailAddress(ErrorMessage = "Email invalido")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Informe a senha")]
    public string Password { get; set; }
}