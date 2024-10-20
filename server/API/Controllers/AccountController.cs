using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext _context, ITokenService tokenService) : BaseApiController
{

    [HttpPost("register")] // by this we can say GO to Controller and then the name we specified /api/Account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto data)
    {

        // Checking if the user is already exist
        if (await UserExist(data.Username)) return BadRequest("UserName is already taken.");

        // creating a method to hash passwords
        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = data.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };

    }


    [HttpPost("login")] // ==> /api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto data)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == data.Username.ToLower());
        if (user is null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        }; ;

    }

    private async Task<bool> UserExist(string username)
    {
        return await _context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
    }

}
