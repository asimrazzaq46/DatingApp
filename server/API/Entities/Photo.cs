using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; } = false;
    public string? PublicId { get; set; }

    //NAvigation Property 
    public int AppuserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
}