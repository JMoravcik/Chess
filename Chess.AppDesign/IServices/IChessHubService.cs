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
    public interface IChessHubService
    {
        event GameStarted GameStarted;
        event Challenged Challenged;
        public Task Start(UserDto userDto);
        public Task Stop();

    }
}
