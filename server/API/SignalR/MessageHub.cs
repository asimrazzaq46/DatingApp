using System;
using System.Text;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class MessageHub(IMessageRepositery _messageRepositery, IUserRepositery _userRepositery,
 IMapper _mapper, IHubContext<PresenceHub> presenceHub) : Hub
{

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["user"];
        if (Context.User is null || string.IsNullOrEmpty(otherUser)) throw new Exception("Cannot join group");
        var groupName = GetgroupName(Context.User.GetUserName(), otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var group  = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup",group);

        var messages = await _messageRepositery.GetMessageThread(Context.User.GetUserName(), otherUser!);

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);


    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        
        var group = await RemoveFromMessageAsync();
        await Clients.Group(group.Name).SendAsync("UpdatedGroup",group);
        await base.OnDisconnectedAsync(exception);

    }

    public async Task SendMessage(CreateMessageDto createMessageDto)
    {
        var username = Context.User?.GetUserName() ?? throw new Exception("Could not get user");
        if (username == createMessageDto.RecipentUserName.ToLower()) throw new HubException("You cannot message yourself");
        var sender = await _userRepositery.GetUserByUserNameAsync(username);
        var recipient = await _userRepositery.GetUserByUserNameAsync(createMessageDto.RecipentUserName);

        if (recipient is null || sender is null || sender.UserName is null || recipient.UserName is null)
            throw new HubException("Cannot send message at this time");

        var message = new Message
        {
            Sender = sender,
            Recipent = recipient,
            SenderUsername = sender.UserName,
            RecipentUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        var groupName = GetgroupName(sender.UserName, recipient.UserName);
        var group = await _messageRepositery.GetMessageGroup(groupName);
        if (group is not null && group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await PresenceTracker.GetConnectionForUser(recipient.UserName);
            if (connections is not null && connections?.Count is not null)
            {
                await presenceHub.Clients.Clients(connections).SendAsync("NewMessageRecieved",
                new
                {
                    username = sender.UserName,
                    knownAs = sender.KnownAs
                });
            }
        }

        _messageRepositery.AddMessage(message);

        if (await _messageRepositery.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
        }

    }

    private async Task<Group> AddToGroup(string groupName)
    {
        var username = Context.User?.GetUserName() ?? throw new Exception("Cannot get username");
        var group = await _messageRepositery.GetMessageGroup(groupName);
        var connection = new Connection
        {
            ConnectionId = Context.ConnectionId,
            Username = username
        };

        if (group is null)
        {
            group = new Group
            {
                Name = groupName
            };
            _messageRepositery.AddGroup(group);
        }
        group.Connections.Add(connection);
        if (await _messageRepositery.SaveAllAsync()) return group;
        throw new HubException("Failed to join group");
    }

    private async Task<Group> RemoveFromMessageAsync()
    {
        var group = await _messageRepositery.GetGroupForConnection(Context.ConnectionId);
        var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection is not null && group is not null)
        {
            _messageRepositery.RemoveConnection(connection);
            if (await _messageRepositery.SaveAllAsync()) return group;
        }

        throw new Exception("Failed to remove from the group");
    }

    private static string GetgroupName(string caller, string? other)
    {

        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}
