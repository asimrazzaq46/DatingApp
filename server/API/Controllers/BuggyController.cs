using System;
using System.Reflection.Metadata.Ecma335;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext _context) : BaseApiController
{
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth()
    {
        return "from get auth error";
    }


    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {

        var user = _context.Users.Find(-1);
        if (user is null) return NotFound();
        return user;
    }

    [HttpGet("server-error")]
    public ActionResult<AppUser> GetServerError()
    {

        var user = _context.Users.Find(-1) ?? throw new Exception("A bad thing is happened");

        return user;
    
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {

        return BadRequest("not a good request");
    }

}
