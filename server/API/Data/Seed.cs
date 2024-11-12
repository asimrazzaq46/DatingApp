using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{

    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        // if there is already users in database then just return. 
        if (await userManager.Users.AnyAsync()) return;

        // if there is not any user than read the file from given path
        var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        //Deserilize in C# object of AppUser formate...
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);
        // if there is no data in json than just return
        if (users is null) return;

        var roles = new List<AppRole>{
            new() {Name="Member"},
            new() {Name="Admin"},
            new() {Name="Moderator"},
        };

        foreach (var role in roles)
        {
            await roleManager.CreateAsync(role);
        }

        //iterate every user in users and add to the database
        foreach (var user in users)
        {

            user.UserName = user.UserName!.ToLower();
            user.Photos.First().IsApproved = true;
            await userManager.CreateAsync(user, "Pa$$w0rd");
            await userManager.AddToRoleAsync(user, "Member");
        }

        var admin = new AppUser
        {
            UserName = "admin",
            KnownAs = "admin",
            Gender = "",
            City = "",
            Country = "",
        };

        await userManager.CreateAsync(admin, "Pa$$w0rd");
        await userManager.AddToRolesAsync(admin,["Admin","Moderator"]);

    }

}
