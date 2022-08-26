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
    public delegate void InvalidMove(string errorMessage);
    public delegate void GameEnded(GameEndedData gameEndedData);
    public delegate void ReceiveActiveUserList(List<UserDto> userDtos);
    public delegate void UserConnected(UserDto userDtos);
    public delegate void UserReconnected(GameData gameData);
    public delegate void UserDisconnected(UserDto userDtos);
    public delegate void Promoting(string pawn);
    public delegate void Waiting();
    public interface IChessHubService
    {
        event GameStarted GameStarted;
        event Challenged Challenged;
        event GameMove GameMove;
        event InvalidMove InvalidMove;
        event GameEnded GameEnded;
        event ReceiveActiveUserList ReceiveActiveUserList;
        event UserDisconnected UserDisconnected;
        event UserConnected UserConnected;
        event UserReconnected UserReconnected;
        event Promoting Promoting;
        event Waiting Waiting;

        public Task Start(UserDto userDto);
        public Task Stop();
        public Task GetActiveUserList();
        public Task PlayRandomGame();
        public Task PlayerMove(string move);
        public Task Promotion(string pawn, int id);
        public Task GetData();
    }
}
