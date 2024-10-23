using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public Boolean IsMain { get; set; }
    public string? PublicId { get; set; }

    //NAvigation Property 
    public int AppuserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
}