using Chess.Core.Managers;
using Chess.Core.MoveResults;
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
            if (gameManager.IsPlaying(Context.ConnectionId))
            {
                await Clients.Caller.SendAsync(Routes.ChessHubClient_OperationFailed, "user is already in game!");
                return;
            }
            var game = gameManager.PlayRandomGame(Context.ConnectionId);
            if (game.firstPlayerId == null) return;
            await Clients.Clients(Context.ConnectionId, game.firstPlayerId.ToString()).SendAsync(Routes.ChessHubClient_StartGame, game.gameData);
        }

        public async Task GetData()
        {
            var data = gameManager.GetGameData(Context.ConnectionId);
            if (data != null)
            {
                await Clients.Caller.SendAsync(Routes.ChessHubClient_UserReconnected, data);
            }
        }

        public async Task MovePlayer(string move)
        {
            (object opponentsId, MoveResult result) = gameManager.PlayerMove(Context.ConnectionId, move);
            if (result is InvalidMove invalidMove)
            {
                await Clients.Caller.SendAsync(Routes.ChessHubClient_InvalidMove, invalidMove.ErrorMessage);
            }
            else if (result is UnfinishedMove unfinishedMove)
            {
                await Clients.Caller.SendAsync(Routes.ChessHubClient_Promotion, move);
            }
            else if (result is SuccessfullMove)
            {
                await Clients.Clients(Context.ConnectionId, opponentsId.ToString()).SendAsync(Routes.ChessHubClient_GameMove, move);
            }
        }

        public async Task Promoting(string move, int id)
        {
            var splittedMove = move.Split(' ');
            (var opponentsId, var result) = gameManager.Promotion(Context.ConnectionId, splittedMove[1], id);
            if (result is InvalidMove invalidMove)
            {
                await Clients.Caller.SendAsync(Routes.ChessHubClient_InvalidMove, invalidMove.ErrorMessage);
            }
            else if (result is SuccessfullMove)
            {
                id = gameManager.GetPlayerColor(Context.ConnectionId) == Core.PlayerColors.White ? id : id + 6;
                await Clients.Clients(Context.ConnectionId, opponentsId.ToString()).SendAsync(Routes.ChessHubClient_GameMove, $"{move} {id}");
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            gameManager.Disonnected(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
