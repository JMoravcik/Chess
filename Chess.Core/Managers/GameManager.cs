using Chess.Core.MoveResults;
using Chess.Shared.Data;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Managers
{
    public delegate Task UserConnected(object id, UserDto userDto);
    public delegate Task UserReconnected(object id, object id2, GameData gameData);
    public delegate Task UserDisconnected(object id, UserDto userDto);
    public delegate Task GameEnded(object[] ids, GameEndedData gameEndedData);
    public class GameManager
    {

        public GameManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public class GameRecord
        {
            public Game Game { get; set; } = new Game();
            public Dictionary<UserDto, Player> Players { get; set; } = new Dictionary<UserDto, Player>();
        }

        public event UserConnected UserConnected;
        public event UserReconnected UserReconnected;
        public event UserDisconnected UserDisconnected;
        public event GameEnded GameEnded;

        Dictionary<Guid, Timer> Timeouts = new Dictionary<Guid, Timer>();
        Dictionary<object, UserDto> ActiveUsers = new Dictionary<object, UserDto>();
        Dictionary<Guid, GameRecord> Games { get; set; } = new Dictionary<Guid, GameRecord>();

        object disconnectingKey = new object();
        private readonly IServiceProvider serviceProvider;

        private IDatabase GetDatabase()
        {
            using var scope = serviceProvider.CreateScope();
            return scope.ServiceProvider.GetRequiredService<IDatabase>();
        }

        public bool Connected(object id, UserDto userDto)
        {
            lock (disconnectingKey)
            {
                if (ActiveUsers.ContainsKey(id))
                {
                    var activeUser = ActiveUsers[id];
                    if (activeUser.Id == userDto.Id)
                    {
                        return UserHasReconnected(id, userDto);
                    }
                    else return false;
                }

                var user = ActiveUsers.FirstOrDefault(u => u.Value.Id == userDto.Id);
                if (user.Value == null)
                {
                    ActiveUsers.Add(id, userDto);
                    UserConnected?.Invoke(id ,userDto);
                    return true;
                }

                userDto = user.Value;
                if (UserHasReconnected(id, userDto))
                {
                    ActiveUsers.Remove(user.Key);
                    ActiveUsers.Add(id, userDto);
                    return true;
                }
                return false;
            }
        }

        private bool UserHasReconnected(object id, UserDto userDto)
        {
            if (Timeouts.ContainsKey(userDto.Id))
            {
                Timeouts[userDto.Id].Dispose();
                Timeouts.Remove(userDto.Id);
                if (userDto.Game != null)
                {
                    var game = Games[userDto.Game.Value];
                    var players = ActiveUsers.Where(a => game.Players.Any(p => p.Key.Id == a.Value.Id)).ToList();
                    if (players.Count != 2) return false;
                    
                    var gameData = new GameData();
                    gameData.IdsMap = game.Game.Chessboard.GetChessboardSymbolMap();
                    gameData.Black = game.Players.First(p => p.Value.PlayerColor == PlayerColors.Black).Key;
                    gameData.White = game.Players.First(p => p.Value.PlayerColor == PlayerColors.White).Key;

                    UserReconnected?.Invoke(players[0].Key, players[1].Key, gameData);
                }
                
                return true;
            }
            return false;
        }

        public void Disonnected(object id)
        {
            if (ActiveUsers.ContainsKey(id))
            {
                var user = ActiveUsers[id];
                if (user.Game != null)
                {
                    Timeouts.Add(user.Id, new Timer(DisconnectUserFromGame, id, TimeSpan.FromSeconds(15), Timeout.InfiniteTimeSpan));
                }
                else
                {
                    ActiveUsers.Remove(id);
                }
                UserDisconnected?.Invoke(id, user);
            }
        }

        private void DisconnectUserFromGame(object state)
        {
            lock(disconnectingKey)
            {
                var user = ActiveUsers[state];
                if (user == null) return;
                if (!Timeouts.ContainsKey(user.Id)) return;
                ActiveUsers.Remove(state);
                Timeouts.Remove(user.Id);
                if (user.Game.HasValue && Games.ContainsKey(user.Game.Value))
                {
                    var player = Games[user.Game.Value].Players[user];
                    Games[user.Game.Value].Game.LeftGame(player);
                }
            }
        }

        public bool IsPlaying(object id)
        {
            var user = ActiveUsers[id];

            return user.Game != null && Games.ContainsKey(user.Game.Value);
        }

        public (object firstPlayerId, GameData gameData) PlayRandomGame(object id)
        {
            var user = ActiveUsers[id];
            var game = Games.FirstOrDefault(g => g.Value.Players.Count == 1);
            Player player;
            if (game.Value != null)
            {
                var firstPlayer = game.Value.Players.First();
                object firstPlayerId = ActiveUsers.First(u => u.Value.Id == firstPlayer.Key.Id).Key;
                player = game.Value.Game.JoinGame(user.NickName);
                game.Value.Players.Add(user, player);
                user.Game = game.Key;
                var gameData = new GameData()
                {
                    IdsMap = game.Value.Game.Chessboard.GetChessboardSymbolMap(),
                    White = user,
                    Black = firstPlayer.Key
                };
                return (firstPlayerId, gameData);
            }
            user.Game = Guid.NewGuid();
            var gameRecord = new GameRecord();
            gameRecord.Game.GameStateChanged += Game_GameStateChanged;
            player = gameRecord.Game.JoinGame(user.NickName);
            gameRecord.Players.Add(user, player);
            Games.Add(user.Game.Value, gameRecord);
            return (null, null);
        }

        private void Game_GameStateChanged(Game game)
        {
            using var database = GetDatabase();
            var gameEndedData = new GameEndedData();
            if (game.GameState == GameStates.Ended)
            {
                var record = Games.First(g => g.Value.Game == game);
                if (record.Value.Players.Any(p => p.Value.PlayerState == PlayerStates.Winner))
                {
                    var winner = record.Value.Players.First(p => p.Value.PlayerState == PlayerStates.Winner);
                    var looser = record.Value.Players.First(p => p.Value.PlayerState != PlayerStates.Winner);
                    gameEndedData.Player1 = winner.Key;
                    gameEndedData.Player1State = Shared.PlayerStates.Winner;
                    gameEndedData.Player2 = winner.Key;
                    gameEndedData.Player2State = Shared.PlayerStates.Looser;
                    int TotalElo = winner.Key.Elo + looser.Key.Elo;
                    double percent = (double)looser.Key.Elo / TotalElo;
                    winner.Key.Elo += (int)(50 * percent);
                    looser.Key.Elo -= (int)(50 * percent);
                    database.UpdateUser(winner.Key);
                    database.UpdateUser(looser.Key);
                }
                else
                {
                    int sum = record.Value.Players.Sum(p => p.Key.Elo);
                    var strongerPlayer = record.Value.Players.MaxBy(p => p.Key.Elo);
                    var weakerPlayer = record.Value.Players.First(p => p.Key != strongerPlayer.Key);
                    int difference = strongerPlayer.Key.Elo - weakerPlayer.Key.Elo;
                    if (difference != 0)
                    {
                        double percent = (double)difference / sum;
                        weakerPlayer.Key.Elo += (int)(percent * 50);
                        strongerPlayer.Key.Elo -= (int)(percent * 50);
                    }
                    database.UpdateUser(strongerPlayer.Key);
                    database.UpdateUser(weakerPlayer.Key);
                    gameEndedData.Player1 = strongerPlayer.Key;
                    gameEndedData.Player1State = Shared.PlayerStates.Drawer;
                    gameEndedData.Player2 = weakerPlayer.Key;
                    gameEndedData.Player2State = Shared.PlayerStates.Drawer;
                }
                var ids = ActiveUsers.Where(u => record.Value.Players.Any(p => p.Key.Id == u.Value.Id)).Select(u => u.Key).ToArray();
                foreach (var id in ids)
                {
                    ActiveUsers[id].Game = null;
                }
                Games.Remove(record.Key);
                GameEnded?.Invoke(ids, gameEndedData);
            }
        }

        public List<UserDto> GetActiveUserList(object id)
        {
            return (from user in ActiveUsers
                   where user.Key != id
                   select user.Value).ToList();
        }

        public (object opponentsId, MoveResult result) PlayerMove(object id, string move)
        {
            var opponentsId = GetOpponentsId(id);
            var user = ActiveUsers[id];
            if (user.Game == null) return (opponentsId, new InvalidMove("You are not logged into any game!"));
            if (!Games.ContainsKey(user.Game.Value)) return (opponentsId, new InvalidMove("You are not logged into any game!"));

            var game = Games[user.Game.Value];
            return (opponentsId, game.Game.PlayerMove(game.Players[user], move));
        }

        public (object opponentsId, MoveResult result) Promotion(object id, string pawnCoordination, int pieceId)
        {
            var opponentsId = GetOpponentsId(id);
            var user = ActiveUsers[id];
            if (user.Game == null) return (opponentsId, new InvalidMove("You are not logged into any game!"));
            if (!Games.ContainsKey(user.Game.Value)) return (opponentsId, new InvalidMove("You are not logged into any game!"));
            var game = Games[user.Game.Value];
            return (opponentsId, game.Game.UpgradePawnAndCompletePlayerMove(game.Players[user], pawnCoordination, pieceId));
        }

        public object GetOpponentsId(object id)
        {
            var user = ActiveUsers[id];
            var gameRecord = Games[user.Game.Value];
            var opponent = gameRecord.Players.First(p => p.Key.Id != user.Id);
            return ActiveUsers.First(u => u.Value.Id == opponent.Key.Id).Key;
        }

        public PlayerColors GetPlayerColor(object id)
        {
            var user = ActiveUsers[id];
            var gameRecord = Games[user.Game.Value];
            return gameRecord.Players[user].PlayerColor;
        }

        public GameData GetGameData(object id)
        {
            var user = ActiveUsers[id];
            if (user.Game == null || !Games.ContainsKey(user.Game.Value)) return null;
            var game = Games[user.Game.Value];
            return new GameData()
            {
                IdsMap = game.Game.Chessboard.GetChessboardSymbolMap(),
                Black = game.Players.First(p => p.Value.PlayerColor == PlayerColors.Black).Key,
                White = game.Players.First(p => p.Value.PlayerColor == PlayerColors.White).Key,
            };
        }
    }
}
