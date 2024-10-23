using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class Seed
{

    public static async Task SeedUsers(DataContext context)
    {
        // if there is already users in database then just return. 
        if (await context.Users.AnyAsync()) return;

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

        //iterate every user in users and add to the database
        foreach (var user in users)
        {
            // Password salt and password hash is required property in AppUser
            // so we are creating it for every user with same Password "Pa$$word"


            using var hmac = new HMACSHA512();
            user.UserName = user.UserName.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$word"));
            user.PasswordSalt = hmac.Key;

            context.Users.Add(user);
        }

        await context.SaveChangesAsync();
    }

}
