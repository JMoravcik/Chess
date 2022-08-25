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

        public async Task Start(UserDto userDto)
        {

            await hubConnection.StartAsync();
            await hubConnection.SendAsync("Connected", userDto);
        }

        public Task Stop()
        {
            return hubConnection.StopAsync();
        }
    }
}
