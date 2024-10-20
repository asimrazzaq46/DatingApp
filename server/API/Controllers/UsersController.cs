using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

// Http://localhost:5001/api/[controller]   => controller will be change by the name
//                                              of file in this case it's "users"


public class UsersController(DataContext _context) : BaseApiController
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();


        return Ok(users);
    }

    [Authorize]
    [HttpGet("{id:int}")] //   /api/users/id
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null) return NotFound();

        return user;
    }



}
