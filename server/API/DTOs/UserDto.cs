using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class UserDto
{
    [Required]
    public required string Username { get; set; }
    [Required]
    public required string Token { get; set; }

    public required string Gender { get; set; }

    [Required]
    public string? KnownAs { get; set; }

    public string? PhotoUrl {get; set;}


}
