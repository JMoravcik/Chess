using Chess.AppDesign.IServices;
using Chess.Shared;
using Chess.Shared.Dtos;
using Microsoft.AspNetCore.SignalR.Client;

namespace Chess.WebAsm.Client.Services
{

    public class ChessHubService : IChessHubService
    {
        private HubConnection hubConnection;

        public ChessHubService()
        {
            hubConnection = new HubConnectionBuilder()
                .WithUrl($"{Routes.ServerUrl}{Routes.ChessHub}")
                .Build();
        }

        public event GameStarted GameStarted;
        public event Challenged Challenged;
        public event GameMove GameMove;
        public event GameEnded GameEnded;
        public event ReceiveActiveUserList ReceiveActiveUserList;
        public event UserDisconnected UserDisconnected;
        public event UserConnected UserConnected;

        public Task GetActiveUserList()
        {
            return hubConnection.SendAsync(Routes.ChessHubServer_GetActiveUserList);
        }

        public async Task Start(UserDto userDto)
        {
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
