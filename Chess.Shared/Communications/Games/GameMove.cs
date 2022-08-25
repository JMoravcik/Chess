using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Communications.Games
{
    public class GameMove
    {
        public string Move { get; set; }
        public PlayerColors NextPlaying { get; set; }

    }
}
