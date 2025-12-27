using System.ComponentModel.DataAnnotations;

namespace FILMHUB.DTO;

public class UpdateUserDto
{
    public string ? Name { get; set; }
    
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Campo obrigatorio")]
    public string? CurrentPassword  { get; set; }
    
    [Required(ErrorMessage = "Campo obrigatorio")]
    [MinLength(6, ErrorMessage = "A senha deve ter no minímo 6 caracteres")]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "Campo obrigatorio")]
    [Compare("Password",ErrorMessage ="As senhas não coincidem")]
    public  string? ConfirmPassword { get; set; }
}