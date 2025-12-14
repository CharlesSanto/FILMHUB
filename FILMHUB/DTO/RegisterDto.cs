using System.ComponentModel.DataAnnotations;

namespace FILMHUB.DTO;

public class RegisterDto
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Name { get; set; }
    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; }
    [Required(ErrorMessage = "A senha é obrigatória")]
    [MinLength(6, ErrorMessage = "A senha deve no minímo 6 caracteres")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Confirme a senha")]
    [Compare("Password", ErrorMessage = "As senhas não coincidem")]
    public string ConfirmPassword { get; set; }
}