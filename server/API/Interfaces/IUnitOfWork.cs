using System;

namespace API.Interfaces;

public interface IUnitOfWork
{
    IUserRepositery UserRepositery { get; }
    IMessageRepositery MessageRepositery { get; }
    ILikesRepositery LikesRepositery { get; }
    IPhotoRepositery PhotoRepositery { get; }
    Task<bool> Complete();
    bool HasChanges();
}
