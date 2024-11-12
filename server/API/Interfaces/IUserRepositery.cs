using System;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces;

public interface IUserRepositery
{
    void update(AppUser user);

    Task<IEnumerable<AppUser>> GetAllUsersAsync();
    Task<AppUser?> GetUserByIdAsync(int id);
    Task<AppUser?> GetUserByUserNameAsync(string username);
    Task<PagedList<MemberDto>> GetAllMembersAsync(UserParams userParams);
    Task<MemberDto?> GetMemberByUsernamAsync(string username, bool isCurrentUser);
    Task<AppUser?> GetUserByPhotoId(int photoId);


}
