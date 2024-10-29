using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext _context, ITokenService tokenService, IMapper _mapper) : BaseApiController
{

    [HttpPost("register")] // by this we can say GO to Controller and then the name we specified /api/Account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto data)
    {

        // Checking if the user is already exist
        if (await UserExist(data.Username)) return BadRequest("UserName is already taken.");

        // creating a method to hash passwords
        using var hmac = new HMACSHA512();

        var user = _mapper.Map<AppUser>(data);

        user.UserName = data.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.Password));
        user.PasswordSalt = hmac.Key;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

    }


    [HttpPost("login")] // ==> /api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto data)
    {
        var user = await _context.Users.Include("Photos").FirstOrDefaultAsync(u => u.UserName == data.Username.ToLower());
        if (user is null) return Unauthorized("Invalid Username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        }

        var PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain)?.Url;

        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain)?.Url
        }; ;

    }

    private async Task<bool> UserExist(string username)
    {
        return await _context.Users.AnyAsync(u => u.UserName.ToLower() == username.ToLower());
    }

}
