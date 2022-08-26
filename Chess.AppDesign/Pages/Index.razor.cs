using Chess.AppDesign.IServices;
using Chess.Shared.Dtos;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Pages
{
    public partial class Index
    {
        [Inject] IChessHubService chessHubService { get; set; }

        List<UserDto> userDtos { get; set; } = new List<UserDto>();
        private async Task PlayRandomGame()
        {
            chessHubService.PlayRandomGame();
        }

        private async Task ChallengeUser(UserDto userDto)
        {

        }

        protected override async Task OnInitializedAsync()
        {
            chessHubService.ReceiveActiveUserList += ChessHubService_ReceiveActiveUserList;
            chessHubService.UserConnected += ChessHubService_UserConnected;
            chessHubService.UserDisconnected += ChessHubService_UserDisconnected;
            await chessHubService.GetActiveUserList();
        }

        private void ChessHubService_UserDisconnected(UserDto userDto)
        {
            userDtos.RemoveAll(u => u.Id == userDto.Id);
            this.InvokeAsync(() => StateHasChanged());
        }

        private void ChessHubService_UserConnected(UserDto userDto)
        {
            if (!userDtos.Any(u => u.Id == userDto.Id))
            {
                userDtos.Add(userDto);
                this.InvokeAsync(() => StateHasChanged());
            }
        }

        private void ChessHubService_ReceiveActiveUserList(List<UserDto> userDtos)
        {
            this.userDtos = userDtos;
            this.InvokeAsync(() => StateHasChanged());
        }
    }
}
