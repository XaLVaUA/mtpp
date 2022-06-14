using Microsoft.AspNetCore.SignalR;
using MTPP5.Data.Entities;

namespace MTPP5.Hubs;

public class AppHub : Hub
{
    public const string Route = "/apphub";
    public const string ReceiveMessageMethodName = "ReceiveMessage";
    public const string RoomJoinMethodName = "RoomJoin";
    public const string RoomJoinResponseMethodName = "RoomJoinResponse";
    public const string RoomLeaveMethodName = "RoomLeave";

    public async Task JoinRoom(User user, int roomId)
    {
        var groupName = GetGroupName(roomId);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync(RoomJoinMethodName, Context.ConnectionId, user);
    }

    public async Task JoinRoomResponse(User user, string connectionId)
    {
        await Clients.Client(connectionId).SendAsync(RoomJoinResponseMethodName, user);
    }

    public async Task LeaveRoom(User user, int roomId)
    {
        var groupName = GetGroupName(roomId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync(RoomLeaveMethodName, user);
    }

    public async Task SendMessage(Message message)
    {
        var groupName = GetGroupName(message.RoomId);
        await Clients.Group(groupName).SendAsync(ReceiveMessageMethodName, message);
    }

    private static string GetGroupName(int roomId) => $"room-{roomId}";
}
