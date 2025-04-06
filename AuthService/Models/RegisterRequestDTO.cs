using Microsoft.Extensions.Options;

namespace AuthService.Models;

public class RegisterRequestDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Patronymic { get; set; }
    public string Role { get; set; }

    private bool Validate()
    {
        if (string.IsNullOrEmpty(Email) ||
            string.IsNullOrEmpty(Password) ||
            string.IsNullOrEmpty(Name) ||
            string.IsNullOrEmpty(Surname) ||
            string.IsNullOrEmpty(Role))
        {
            return false;
        }

        return true;
    }
}