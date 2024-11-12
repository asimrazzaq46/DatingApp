using System;

namespace API.DTOs;

public class PhotosForApprovalDto
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public string? Username { get; set; }
    public bool IsApproved { get; set; }
}
