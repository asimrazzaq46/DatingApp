using System;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker tracker) : Hub
{
    public override async Task OnConnectedAsync()
    {
        if (Context.User == null) throw new HubException("cannot get current user claim");

        var isOnline = await tracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
        if (isOnline) await Clients.Others.SendAsync("UserIsOnline", Context.User?.GetUserName());

        var currenUsers = await tracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currenUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {

        if (Context.User == null) throw new HubException("cannot get current user claim");

        var isOffline = await tracker.UserDisConnected(Context.User.GetUserName(), Context.ConnectionId);

        if (isOffline) await Clients.Others.SendAsync("UserIsOffline", Context.User?.GetUserName());


        await base.OnDisconnectedAsync(exception);
    }
}
