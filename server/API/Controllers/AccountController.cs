using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> _userManager, ITokenService tokenService, IMapper _mapper) : BaseApiController
{

    [HttpPost("register")] // by this we can say GO to Controller and then the name we specified /api/Account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDto data)
    {

        // Checking if the user is already exist
        if (await UserExist(data.Username)) return BadRequest("UserName is already taken.");


        var user = _mapper.Map<AppUser>(data);

        user.UserName = data.Username.ToLower();

        // creating a method to hash passwords
        // using var hmac = new HMACSHA512();
        // without identity that's how we save the password
        // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.Password));
        // user.PasswordSalt = hmac.Key;

        var result = await _userManager.CreateAsync(user, data.Password);
        if (!result.Succeeded) return BadRequest(result.Errors);
        return new UserDto
        {
            Username = user.UserName,
            Token = await tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            Gender = user.Gender
        };

    }


    [HttpPost("login")] // ==> /api/account/login
    public async Task<ActionResult<UserDto>> Login(LoginDto data)
    {
        if(data.Username is null || data.Password is null) return BadRequest("username or password is not provided");
        var user = await _userManager.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.NormalizedUserName == data.Username.ToUpper());

        if (user is null || user.UserName is null) return Unauthorized("Invalid Username");

        var result = await _userManager.CheckPasswordAsync(user, data.Password);
        if (!result) return Unauthorized();


        //old code to save passwords
        // using var hmac = new HMACSHA512(user.PasswordSalt);

        // var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data.Password));

        // for (int i = 0; i < computedHash.Length; i++)
        // {
        //     if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
        // }



        return new UserDto
        {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = await tokenService.CreateToken(user),
            Gender = user.Gender,
            PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain)?.Url
        }; ;

    }

    private async Task<bool> UserExist(string username)
    {
        return await _userManager.Users.AnyAsync(u => u.NormalizedUserName == username.ToUpper());
    }

}
