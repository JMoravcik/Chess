using Chess.Core.Managers;
using Chess.Shared;
using Chess.WebAsm.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Chess.WebAsm.Server.Services
{
    public class ChessHubService : IHostedService
    {
        private readonly GameManager gameManager;
        private readonly IHubContext<ChessHub> hubContext;

        public ChessHubService(GameManager gameManager, IHubContext<ChessHub> hubContext)
        {
            this.gameManager = gameManager;
            this.hubContext = hubContext;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            gameManager.UserConnected += GameManager_UserConnected;
            gameManager.UserDisconnected += GameManager_UserDisconnected;
            gameManager.UserReconnected += GameManager_UserReconnected;
            gameManager.GameEnded += GameManager_GameEnded;
            return Task.CompletedTask;
        }

        private Task GameManager_GameEnded(object[] ids, Shared.Data.GameEndedData gameEndedData)
        {
            return hubContext.Clients.Clients(ids.Select(i => i.ToString())).SendAsync(Routes.ChessHubClient_GameEnded, gameEndedData);
        }

        private Task GameManager_UserReconnected(object id, object id2, Shared.Data.GameData gameData)
        {
            return hubContext.Clients.Clients(id.ToString(), id2.ToString()).SendAsync(Routes.ChessHubClient_UserReconnected, gameData);
        }

        private Task GameManager_UserConnected(object id, Shared.Dtos.UserDto userDto)
        {
            return hubContext.Clients.AllExcept(id.ToString()).SendAsync(Routes.ChessHubClient_UserConnected, userDto);
        }



        private Task GameManager_UserDisconnected(object id, Shared.Dtos.UserDto userDto)
        {
            return hubContext.Clients.AllExcept(id.ToString()).SendAsync(Routes.ChessHubClient_UserDisconnected, userDto);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            gameManager.UserConnected -= GameManager_UserConnected;
            gameManager.UserDisconnected -= GameManager_UserDisconnected;
            gameManager.UserReconnected -= GameManager_UserReconnected;
            gameManager.GameEnded -= GameManager_GameEnded;
            return Task.CompletedTask;
        }
    }
}
