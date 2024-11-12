using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositery;

public class MessageRepositery(DataContext _context, IMapper _mapper) : IMessageRepositery
{
    public void AddGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection?> GetConnection(string ConnectionId)
    {
        return await _context.Connections.FindAsync(ConnectionId);
    }

    public async Task<Message?> GetMessage(int messageId)
    {

        return await _context.Messages.FindAsync(messageId);

    }

    public async Task<Group?> GetMessageGroup(string groupName)
    {
        return await _context.Groups
        .Include(x => x.Connections)
        .FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<PagedList<MessageDto>> GetMessagesForUser(MessagesParams messagesParams)
    {
        var query = _context.Messages
                .OrderByDescending(x => x.MessageSent)
                .AsQueryable();

        query = messagesParams.Container switch
        {
            "Inbox" => query.Where(x => x.Recipent.UserName == messagesParams.Username && x.RecipentDeleted == false),
            "Outbox" => query.Where(x => x.Sender.UserName == messagesParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.Recipent.UserName == messagesParams.Username && x.DateRead == null && x.RecipentDeleted == false)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDto>.CreateAsync(messages, messagesParams.PageNumber, messagesParams.PageSize);


    }

    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
    {
        /* In where(Condition) we are getting the messages for both the users 
             x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername:
             Selects messages where the current user is the recipient and the recipient user is the sender.

             x.SenderUsername == currentUsername && x.RecipientUsername == recipientUsername: 
             Selects messages where the current user is the sender and the recipient user is the recipient.
             for better understanding change currentUsername to me and recipientUsername to otherUser
          */

        var query = _context.Messages
        .Where(x =>
        x.RecipentUsername == currentUsername && x.RecipentDeleted == false && x.SenderUsername == recipientUsername ||
        x.SenderUsername == currentUsername && x.SenderDeleted == false && x.RecipentUsername == recipientUsername)
        .OrderBy(x => x.MessageSent)
        .AsQueryable();


        //getting currentusers unread message from the above query
        var unreadMessages = query.Where(x => x.DateRead == null && x.RecipentUsername == currentUsername).ToList();

        if (unreadMessages.Count != 0)
        {
            unreadMessages.ForEach(message => message.DateRead = DateTime.UtcNow);
        }
        return await query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider).ToListAsync();
    }

    public void RemoveConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }
    public async Task<Group?> GetGroupForConnection(string ConnectionId)
    {
        return await _context.Groups
        .Include(x => x.Connections)
        .Where(x => x.Connections.Any(c => c.ConnectionId == ConnectionId))
        .FirstOrDefaultAsync();

    }


}

