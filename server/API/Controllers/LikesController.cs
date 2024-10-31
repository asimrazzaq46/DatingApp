using System;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LikesController(ILikesRepositery _likesRepositery) : BaseApiController
{
    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> Toggleike(int targetUserId)
    {
        var sourceUserId = User.GetUserId();
        if (sourceUserId == targetUserId) return BadRequest("You cannot like yourself");
        var existingLike = await _likesRepositery.GetUserLike(sourceUserId, targetUserId);

        if (existingLike is null)
        {
            var like = new UserLike
            {
                SourcerUserId = sourceUserId,
                TargetUserId = targetUserId
            };
            _likesRepositery.AddLike(like);
        }
        else
        {
            _likesRepositery.DeleteLike(existingLike);
        }
        if (await _likesRepositery.SaveChanges()) return Ok();

        return BadRequest("Failed to update like");
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikesId()
    {
        return Ok(await _likesRepositery.GetCurrentUserLikeIds(User.GetUserId()));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = User.GetUserId();
        var users = await _likesRepositery.GetUserLikes(likesParams);

        Response.AddPaginationHeader(users);


        return Ok(users);
    }
}
