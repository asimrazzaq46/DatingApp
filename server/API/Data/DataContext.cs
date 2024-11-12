using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace API.Data;

public class DataContext(DbContextOptions options) : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>,
    IdentityUserToken<int>>(options)
{

    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
        .HasMany(ur => ur.UserRoles)
        .WithOne(u => u.User)
        .HasForeignKey(ur => ur.UserId)
        .IsRequired();


        builder.Entity<AppRole>()
        .HasMany(ur => ur.UserRoles)
        .WithOne(u => u.Role)
        .HasForeignKey(ur => ur.RoleId)
        .IsRequired();

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


        //MESSAGES CONFIGURATION

        builder.Entity<Message>()
        .HasOne(u => u.Recipent)
        .WithMany(u => u.MessagesRecived)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
        .HasOne(u => u.Sender)
        .WithMany(U => U.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);

        //  Returning only approved Photos
        builder.Entity<Photo>()
        .HasQueryFilter(p=>p.IsApproved);

    }

}
