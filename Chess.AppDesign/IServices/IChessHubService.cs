using Chess.Shared.Data;
using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.IServices
{
    public delegate void GameStarted(GameData gameData);
    public delegate void Challenged(UserDto userDto);
    public delegate void GameMove(string move);
    public delegate void GameEnded();
    public delegate void ReceiveActiveUserList(List<UserDto> userDtos);
    public delegate void UserConnected(UserDto userDtos);
    public delegate void UserDisconnected(UserDto userDtos);
    public interface IChessHubService
    {
        event GameStarted GameStarted;
        event Challenged Challenged;
        event GameMove GameMove;
        event GameEnded GameEnded;
        event ReceiveActiveUserList ReceiveActiveUserList;
        event UserDisconnected UserDisconnected;
        event UserConnected UserConnected;

        public Task Start(UserDto userDto);
        public Task Stop();
        public Task GetActiveUserList();

    }
}
