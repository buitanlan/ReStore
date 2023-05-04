#nullable disable

namespace API.DTOs;

public class RegisterDto : LoginDto
{
    public string Email { get; set; }
}