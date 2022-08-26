using Chess.AppDesign.IServices;
using Chess.Shared;
using Chess.Shared.Data;
using Chess.Shared.Dtos;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chess.WebAsm.Client.Services
{

    public class HubService : IChessHubService
    {
        private readonly IJsService jsService;
        private HubConnection hubConnection;

        public HubService(IJsService jsService)
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Routes.ServerUrl}{Routes.ChessHub}")
                .Build();
            this.jsService = jsService;
        }

        public event GameStarted GameStarted;
        public event Challenged Challenged;
        public event GameMove GameMove;
        public event GameEnded GameEnded;
        public event ReceiveActiveUserList ReceiveActiveUserList;
        public event UserDisconnected UserDisconnected;
        public event UserConnected UserConnected;
        public event InvalidMove InvalidMove;
        public event UserReconnected UserReconnected;
        public event Promoting Promoting;
        public event Waiting Waiting;

        public Task GetActiveUserList()
        {
            return hubConnection.SendAsync(Routes.ChessHubServer_GetActiveUserList);
        }

        public Task GetData()
        {
            return hubConnection.SendAsync(Routes.ChessHubServer_GetData);
        }

        public Task PlayerMove(string move)
        {
            return hubConnection.SendAsync(Routes.ChessHubServer_MovePlayer, move);
        }

        public Task PlayRandomGame()
        {
            return hubConnection.SendAsync(Routes.ChessHubServer_PlayRandomGame);
        }

        public Task Promotion(string pawn, int id)
        {
            return hubConnection.SendAsync(Routes.ChessHubServer_Promoting, pawn, id);
        }

        public async Task Start(UserDto userDto)
        {
            hubConnection.On<string>(Routes.ChessHubClient_OperationFailed, async (data) => await jsService.Alert(data));
            hubConnection.On(Routes.ChessHubClient_Waiting, () => Waiting?.Invoke());
            hubConnection.On<GameData>(Routes.ChessHubClient_StartGame, data => GameStarted?.Invoke(data));
            hubConnection.On<GameData>(Routes.ChessHubClient_UserReconnected, data => UserReconnected?.Invoke(data));
            hubConnection.On<string>(Routes.ChessHubClient_InvalidMove, data => InvalidMove?.Invoke(data));
            hubConnection.On<string>(Routes.ChessHubClient_Promotion, (data) => Promoting?.Invoke(data));
            hubConnection.On<string>(Routes.ChessHubClient_GameMove, data => GameMove?.Invoke(data));
            hubConnection.On<GameEndedData>(Routes.ChessHubClient_GameEnded, data => GameEnded?.Invoke(data));
            hubConnection.On<List<UserDto>>(Routes.ChessHubClient_ActiveUserList, data => ReceiveActiveUserList?.Invoke(data));
            hubConnection.On<UserDto>(Routes.ChessHubClient_UserConnected, data => 
            {
                UserConnected?.Invoke(data);
            });
            hubConnection.On<UserDto>(Routes.ChessHubClient_UserDisconnected, data => UserDisconnected?.Invoke(data));


            await hubConnection.StartAsync();
            await hubConnection.SendAsync(Routes.ChessHubServer_Connected, userDto);
        }

        public Task Stop()
        {
            return hubConnection.StopAsync();
        }
    }
}
