using Chess.Core.Managers;
using Chess.Core.Services;
using Chess.Shared;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Chess.WebAsm.Server.Hubs
{
    public class ChessHub : Hub
    {
        private readonly GameManager gameManager;

        public ChessHub(GameManager gameManager)
        {
            this.gameManager = gameManager;
        }


        public async Task Connected(UserDto userDto)
        {
            gameManager.Connected(Context.ConnectionId, userDto);
        }

        public async Task GetActiveUserList()
        {
            var result = gameManager.GetActiveUserList(Context.ConnectionId);
            await Clients.Caller.SendAsync(Routes.ChessHubClient_ActiveUserList, result);
        }

        public async Task PlayRandomGame()
        {
            var id = gameManager.PlayRandomGame(Context.ConnectionId);
            if (id == null) return;

            Clients.Clients(Context.ConnectionId, id).SendAsync()
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            gameManager.Disonnected(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
