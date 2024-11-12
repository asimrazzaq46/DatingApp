using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositery;

public class PhotoRepositery(DataContext _context) : IPhotoRepositery
{
    public async Task<Photo?> GetPhotoById(int id)
    {
        return await _context.Photos
        .IgnoreQueryFilters()
        .SingleOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<PhotosForApprovalDto>> GetUnApprovedPhotos()
    {
        return await _context.Photos
        .IgnoreQueryFilters()
        .Where(p => p.IsApproved == false)
        .Select(p => new PhotosForApprovalDto
        {
            Id = p.Id,
            Url = p.Url,
            Username = p.AppUser.UserName,
            IsApproved = p.IsApproved
        })
        .ToListAsync();
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}
