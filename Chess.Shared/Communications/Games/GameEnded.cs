using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Communications.Games
{
    public class GameEnded
    {
        PlayerStates Black { get; set; }
        PlayerStates White { get; set; }
    }
}
