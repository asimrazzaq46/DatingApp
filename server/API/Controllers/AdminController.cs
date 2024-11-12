using System;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminController(UserManager<AppUser> _userManager, IUnitOfWork _unitOfWork, IPhotoService photoService) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users
        .OrderBy(u => u.UserName)
        .Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList(),
        }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select atleast one role");

        var selectedRoles = roles.Split(",").ToList();
        var user = await _userManager.FindByNameAsync(username);

        if (user is null) return BadRequest("User not found");

        var userRoles = await _userManager.GetRolesAsync(user);

        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("failed to add to roles");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

        if (!result.Succeeded) return BadRequest("Failed to remove from roles");

        return Ok(await _userManager.GetRolesAsync(user));

    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GrtPhotosWithModeration()
    {
        var photos = await _unitOfWork.PhotoRepositery.GetUnApprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approve-photo/{id}")]
    public async Task<ActionResult> ApprovePhoto(int id)
    {
        var photo = await _unitOfWork.PhotoRepositery.GetPhotoById(id);
        if (photo is null) return BadRequest("Photo not found");
        photo.IsApproved = true;
        var user = await _unitOfWork.UserRepositery.GetUserByPhotoId(photo.Id);
        if (user is null) return BadRequest("could not get user");

        if (!user.Photos.Any(p => p.IsMain)) photo.IsMain = true;

        await _unitOfWork.Complete();

        return Ok();
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("reject-photo/{id}")]
    public async Task<ActionResult> RejectPhoto(int id)
    {
        var photo = await _unitOfWork.PhotoRepositery.GetPhotoById(id);
        if (photo is null) return BadRequest("Photo not found");

        if (photo.PublicId is not null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Result == "ok")
            {

                _unitOfWork.PhotoRepositery.RemovePhoto(photo);
            }
        }
        else
        {
            _unitOfWork.PhotoRepositery.RemovePhoto(photo);

        }

        await _unitOfWork.Complete();


        return Ok();
    }



}
