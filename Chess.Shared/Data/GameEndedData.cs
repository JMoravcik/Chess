using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Data
{
    public class GameEndedData
    {
        public UserDto Player1 { get; set; }
        public PlayerStates Player1State { get; set; }
        public UserDto Player2 { get; set; }
        public PlayerStates Player2State { get; set; }
    }
}
