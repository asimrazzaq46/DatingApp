using System;
using API.Controllers;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositery;

public class UserRepositery(DataContext _context, IMapper _mapper) : IUserRepositery
{
    public async Task<AppUser?> GetUserByIdAsync(int id)
    {
        return await _context.Users.Include(x => x.Photos)
        .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<AppUser?> GetUserByUserNameAsync(string username)
    {
        return await _context.Users.Include(X => X.Photos).SingleOrDefaultAsync(u => u.UserName == username);
    }

    public async Task<IEnumerable<AppUser>> GetAllUsersAsync()
    {
        return await _context.Users
        .Include(X => X.Photos)
        .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void update(AppUser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }

    public async Task<IEnumerable<MemberDto>> GetAllMembersAsync()
    {
        return await _context.Users
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
    }
 
    public async Task<MemberDto?> GetMemberByUsernamAsync(string username)
    {
        return await _context.Users
        .Where(x => x.UserName == username)
        .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
        .SingleOrDefaultAsync();

    }
}
