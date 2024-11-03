namespace API.Entities;

public class Message
{
    public int Id { get; set; }
    public required string SenderUsername { get; set; }
    public required string RecipentUsername { get; set; }
    public required string Content { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; } = DateTime.UtcNow;
    public bool SenderDeleted { get; set; }
    public bool RecipentDeleted { get; set; }

    // CONNECTION BTW USERS ForeignKeys
    public int SenderId { get; set; }
    public AppUser Sender { get; set; } = null!;
    public int RecipentId { get; set; }
    public AppUser Recipent { get; set; } = null!;
 
}
