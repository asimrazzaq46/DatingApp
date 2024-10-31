using System;

namespace API.Entities;

public class UserLike
{
    public AppUser SourcerUser { get; set; } = null!;
    public int SourcerUserId { get; set; }
    public AppUser TargetUser { get; set; } = null!;
    public int TargetUserId { get; set; }
}
