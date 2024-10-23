using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// Http://localhost:5001/api/[controller]   => controller will be change by the name
//                                              of file in this case it's "users"

[Authorize]
public class UsersController(IUserRepositery _userRepo, IMapper _mapper) : BaseApiController
{
    //IMapper is used to automatically map all the properties into DTOs specified....
    //only if the properties names are same in DTo and Entity files
    // this is how it maps _mapper.Map<NameOFDto>(Entity);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await _userRepo.GetAllMembersAsync();


        return Ok(users);
    }

    [HttpGet("{username}")] //   /api/users/username
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await _userRepo.GetMemberByUsernamAsync(username);

        if (user is null) return NotFound();


        return user;
    }



}
