using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Managers
{
    public delegate Task UserConnected(object id, UserDto userDto);
    public delegate Task UserReconnected(object id, UserDto userDto);
    public delegate Task UserDisconnected(object id, UserDto userDto);
    public delegate Task UserLeft(object id, UserDto userDto);
    public class GameManager
    {
        public class GameRecord
        {
            public Game Game { get; set; }
            public Dictionary<UserDto, Player> Players { get; set; }
        }

        public event UserConnected UserConnected;
        public event UserReconnected UserReconnected;
        public event UserDisconnected UserDisconnected;
        public event UserLeft UserLeft;

        Dictionary<Guid, Timer> Timeouts = new Dictionary<Guid, Timer>();
        Dictionary<object, UserDto> ActiveUsers = new Dictionary<object, UserDto>();
        Dictionary<Guid, GameRecord> Games { get; set; } = new Dictionary<Guid, GameRecord>();

        object disconnectingKey = new object();

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
                UserReconnected?.Invoke(id, userDto);
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
                    Timeouts.Add(user.Id, new Timer(DisconnectUserFromGame, id, TimeSpan.FromSeconds(15), TimeSpan.MaxValue));
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
                UserLeft?.Invoke(state, user);

            }
        }

        public object PlayRandomGame(object id)
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
                return firstPlayerId;
            }
            user.Game = Guid.NewGuid();
            var gameRecord = new GameRecord();
            gameRecord.Game = new Game();
            player = gameRecord.Game.JoinGame(user.NickName);
            gameRecord.Players.Add(user, player);
            Games.Add(user.Game.Value, gameRecord);
            return null;
        }

        public List<UserDto> GetActiveUserList(object id)
        {
            return (from user in ActiveUsers
                   where user.Key != id
                   select user.Value).ToList();
        }
    }
}
