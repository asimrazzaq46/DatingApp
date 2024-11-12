using System;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IPhotoRepositery
{
    Task<IEnumerable<PhotosForApprovalDto>> GetUnApprovedPhotos();
    Task<Photo?> GetPhotoById(int id);
    void RemovePhoto(Photo photo);
}
