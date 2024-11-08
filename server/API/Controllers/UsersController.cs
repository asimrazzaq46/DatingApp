using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

// Http://localhost:5001/api/[controller]   => controller will be change by the name
//                                              of file in this case it's "users"

[Authorize]
public class UsersController(IUnitOfWork _unitOfWork, IMapper _mapper, IPhotoService _photoService) : BaseApiController
{
    //IMapper is used to automatically map all the properties into DTOs specified....
    //only if the properties names are same in DTo and Entity files
    // this is how it maps _mapper.Map<NameOFDto>(Entity);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
    {


        userParams.CurrentUserName = User.GetUserName();

        var users = await _unitOfWork.UserRepositery.GetAllMembersAsync(userParams);

        Response.AddPaginationHeader(users);

        return Ok(users);
    }

    [HttpGet("{username}")] //   /api/users/username
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await _unitOfWork.UserRepositery.GetMemberByUsernamAsync(username);

        if (user is null) return NotFound();


        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {

        //Created an extension Method to get username
        var username = User.GetUserName();

        var user = await _unitOfWork.UserRepositery.GetUserByUserNameAsync(username);

        if (user is null) return BadRequest("No user found in database");

        _mapper.Map(memberUpdateDto, user);

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed To update the user");

    }

    [HttpPost("add-photo")]

    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        //Created an extension Method to get username
        var username = User.GetUserName();

        var user = await _unitOfWork.UserRepositery.GetUserByUserNameAsync(username);

        if (user is null) return BadRequest("User is not found");

        var result = await _photoService.AddPhotoAsync(file);

        if (result.Error is not null) return BadRequest(result.Error.Message);

        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId

        };

        if (user.Photos.Count == 0) photo.IsMain = true;


        user.Photos.Add(photo);

        // status of 201 (CREATED) and sending the location where it gets updated 
        // CreatedAtAction take thre3 arguments...controller,parameter and result we want to send
        // => CreatedAtAction("/api/users", "username" , data)

        if (await _unitOfWork.Complete()) return CreatedAtAction(nameof(GetUser),
            new { username = user.UserName },
            _mapper.Map<PhotoDto>(photo));

        return BadRequest("Problem adding photo");
    }


    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await _unitOfWork.UserRepositery.GetUserByUserNameAsync(User.GetUserName());
        if (user is null) return BadRequest("User is not found");

        var photo = user.Photos.FirstOrDefault(u => u.Id == photoId);
        if (photo is null || photo.IsMain) return BadRequest("Cannot use this as Main Photo");

        var currentMainPhoto = user.Photos.FirstOrDefault(u => u.IsMain);

        if (currentMainPhoto is not null) currentMainPhoto.IsMain = false;
        photo.IsMain = true;

        if (await _unitOfWork.Complete()) return NoContent();

        return BadRequest("Problem setting Main Image");
    }

    [HttpDelete("delete-photo/{photoId:int}")]


    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        var user = await _unitOfWork.UserRepositery.GetUserByUserNameAsync(User.GetUserName());
        if (user is null) return BadRequest("User is not found");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

        if (photo is null || photo.IsMain) return BadRequest("this photo cannot be deleted");
        if (photo.PublicId is not null)
        {

            var result = await _photoService.DeletePhotoAsync(photo.PublicId);
            if (result.Error is not null) return BadRequest(result.Error.Message);
        }
        user.Photos.Remove(photo);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem Deleting Photo");

    }


}
