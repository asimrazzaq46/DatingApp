using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{

    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>().HasKey(k => new { k.SourcerUserId, k.TargetUserId });

        // Connection with SourceUser (User who likes the photo ) WithMany users LikedUsers ( reciveng Likes )
        builder.Entity<UserLike>()
        .HasOne(s => s.SourcerUser)
        .WithMany(l => l.LikedUsers)
        .HasForeignKey(s => s.SourcerUserId)
        .OnDelete(DeleteBehavior.Cascade);

        // Connection with TargetUser (User who recived likes ) WithMany users who liked his profile LikedBYUsers ( sending Likes )
        builder.Entity<UserLike>()
        .HasOne(s => s.TargetUser)
        .WithMany(l => l.LikedByUsers)
        .HasForeignKey(s => s.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);
    }

}
