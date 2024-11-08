using System;
using API.Interfaces;

namespace API.Data;

public class UnitOfWork : IUnitOfWork
{
    public IUserRepositery UserRepositery { get; private set;}
    public IMessageRepositery MessageRepositery { get; private set;}

    public ILikesRepositery LikesRepositery { get; private set;}
    public DataContext _context;

    public UnitOfWork(IUserRepositery userRepositery, IMessageRepositery messageRepositery,
                     ILikesRepositery likesRepositery, DataContext context)
    {
        _context = context;
        UserRepositery = userRepositery;
        MessageRepositery = messageRepositery;
        LikesRepositery = likesRepositery;
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
