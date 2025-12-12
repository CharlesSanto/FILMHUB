using System.Text.RegularExpressions;

namespace FILMHUB.Helpers;

public class ValidateEmailHelper
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
}