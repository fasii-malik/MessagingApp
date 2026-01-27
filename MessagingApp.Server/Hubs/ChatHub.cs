using MessagingApp.Server.Application.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MessagingApp.Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IUserConnectionService _connections;

        public ChatHub(IMessageService messageService, IUserConnectionService connections)
        {
            _messageService = messageService;
            _connections = connections;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier!;
            _connections.Add(userId, Context.ConnectionId);

            // Send all online users to this newly connected client
            var allOnlineUsers = _connections.GetAllOnlineUserIds()
                .Where(id => id != userId)
                .ToList();

            await Clients.Caller.SendAsync("SetOnlineUsers", allOnlineUsers);

            // Notify all OTHER clients that this user came online
            await Clients.Others.SendAsync("UserOnline", userId);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            var userId = Context.UserIdentifier!;
            _connections.Remove(userId, Context.ConnectionId);

            if (!_connections.IsOnline(userId))
            {
                // Notify all OTHER clients that this user went offline
                await Clients.Others.SendAsync("UserOffline", userId);
            }

            await base.OnDisconnectedAsync(ex);
        }


        public async Task SendMessage(string receiverId, string content)
        {
            var senderId = Context.UserIdentifier!;

            // Save and cache
            var message = await _messageService.SendMessageAsync(senderId, receiverId, content);

            // Send to receiver if online
            //if (_connections.IsOnline(receiverId))
            //{
                var connectionIds = _connections.GetConnectionIds(receiverId);
                foreach (var connId in connectionIds)
                {
                    await Clients.Client(connId).SendAsync("ReceiveMessage", message);
                }
           // }


            // Send back to sender
            await Clients.Caller.SendAsync("ReceiveMessage", message);
        }
    }
}
