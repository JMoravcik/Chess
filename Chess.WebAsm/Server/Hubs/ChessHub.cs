using Chess.Core.Services;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Chess.WebAsm.Server.Hubs
{
    public class ChessHub : Hub
    {
        public ChessHub(GameService gameService)
        {
            GameService = gameService;
        }

        public GameService GameService { get; }
        private IDatabase Database { get; }

        public async Task Connected(UserDto userDto)
        {
            Context.Items.Add(Context.ConnectionId, userDto);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Context.Items.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}
