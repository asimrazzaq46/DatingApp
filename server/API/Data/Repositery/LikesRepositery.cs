using System;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Data.Repositery;

public class LikesRepositery(DataContext _context, IMapper _mapper) : ILikesRepositery
{
    public void AddLike(UserLike like)
    {
        _context.Likes.Add(like);
    }

    public void DeleteLike(UserLike like)
    {
        _context.Likes.Remove(like);
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int CurrentUserId)
    {
        return await _context.Likes
        .Where(u => u.SourcerUserId == CurrentUserId)
        .Select(x => x.TargetUserId)
        .ToListAsync();

    }

    public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
    {
        return await _context.Likes.FindAsync(sourceUserId, targetUserId);
    }

    public async Task<PagedList<MemberDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = _context.Likes.AsQueryable();
        IQueryable<MemberDto> query;
        switch (likesParams.Predicate)
        {
            case "liked":
                query = likes
                .Where(s => s.SourcerUserId == likesParams.UserId)
                .Select(x => x.TargetUser)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                break;

            case "likedBy":
                query = likes
                .Where(u => u.TargetUserId == likesParams.UserId)
                .Select(x => x.SourcerUser)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                break;

            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

                query = likes
                .Where(u => u.TargetUserId == likesParams.UserId && likeIds.Contains(u.SourcerUserId))
                .Select(x => x.SourcerUser)
                .ProjectTo<MemberDto>(_mapper.ConfigurationProvider);
                break;
        }

        return await PagedList<MemberDto>.CreateAsync(query, likesParams.PageNumber, likesParams.PageSize);
    }

    public async Task<bool> SaveChanges()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}
