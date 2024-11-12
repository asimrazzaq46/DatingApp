using System;
using API.Data.Repositery;
using API.Interfaces;

namespace API.Data;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepositery UserRepositery { get; private set; }
    public IMessageRepositery MessageRepositery { get; private set; }

    public ILikesRepositery LikesRepositery { get; private set; }

    public IPhotoRepositery PhotoRepositery { get; private set; }

    public DataContext _context;

    public UnitOfWork(IUserRepositery userRepositery, IMessageRepositery messageRepositery,
                     ILikesRepositery likesRepositery, IPhotoRepositery photoRepositery, DataContext context)
    {
        _context = context;
        UserRepositery = userRepositery;
        MessageRepositery = messageRepositery;
        LikesRepositery = likesRepositery;
        PhotoRepositery = photoRepositery;
    }



    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}
