 using MessagingApp.Client.Models.Chat;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

public class ChatHubClient
{
    private HubConnection? _connection;
    private readonly ILogger<ChatHubClient> _logger;

    public ChatHubClient(ILogger<ChatHubClient> logger)
    {
        _logger = logger;
    }

    public event Action<string>? UserOnline;
    public event Action<string>? UserOffline;
    public event Action<MessageDto>? MessageReceived;
    public event Action<GroupMessageDto>? GroupMessageReceived;
    public event Action<List<string>>? SetOnlineUsers;
    


    public async Task ConnectAsync(string token, NavigationManager nav)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(("https://localhost:7222/chathub"), options =>
            {
                options.AccessTokenProvider = () => Task.FromResult(token);
            })
            .WithAutomaticReconnect()
                .WithAutomaticReconnect()
    // ADD THIS:
    .ConfigureLogging(logging => {
        logging.SetMinimumLevel(LogLevel.Information);
        logging.AddDebug(); // This forces output to the VS Debug Console
    })
            .Build();

        // Lifecycle Hooks
        _connection.Reconnecting += (error) => {
            _logger.LogWarning("SignalR lost connection. Reconnecting: {Error}", error?.Message);
            return Task.CompletedTask;
        };

        _connection.Reconnected += (connectionId) => {
            _logger.LogInformation("SignalR reconnected. New ID: {Id}", connectionId);
            return Task.CompletedTask;
        };

        _connection.Closed += (error) => {
            _logger.LogError("SignalR connection closed: {Error}", error?.Message);
            return Task.CompletedTask;
        };

        _connection.On<string>("UserOnline", id => UserOnline?.Invoke(id));
        _connection.On<string>("UserOffline", id => UserOffline?.Invoke(id));
        _connection.On<MessageDto>("ReceiveMessage", msg => MessageReceived?.Invoke(msg));
        _connection.On<GroupMessageDto>("ReceiveGroupMessage", msg => GroupMessageReceived?.Invoke(msg));
        _connection.On<List<string>>("SetOnlineUsers", ids => SetOnlineUsers?.Invoke(ids));
        


        try
        {
            await _connection.StartAsync();
            _logger.LogInformation("SignalR connected successfully. State: {State}", _connection.State);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while connecting to SignalR hub.");
            throw; // Re-throw if you want the UI to handle the error
        }
    }

    public async Task SendMessageAsync(string receiverId, string message)
    {
        if (_connection != null)
            await _connection.SendAsync("SendMessage", receiverId, message);
    }

    public async Task SendGroupMessageAsync(Guid groupId, Guid senderId, string message)
    {
        if (_connection != null)
            await _connection.SendAsync("SendGroupMessage", groupId, senderId, message);
    }

    public async Task JoinGroupAsync(Guid groupId)
    {
        if (_connection != null)
            await _connection.SendAsync("JoinGroup", groupId);
    }

    public async Task LeaveGroupAsync(Guid groupId)
    {
        if (_connection != null)
            await _connection.SendAsync("LeaveGroup", groupId);
    }


    public async Task DisconnectAsync()
    {
        if (_connection != null)
        {
            await _connection.StopAsync();
            await _connection.DisposeAsync();
            _connection = null;
        }
    }

}
