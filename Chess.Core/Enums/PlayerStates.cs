﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public enum PlayerStates
    {
        WaitingForOponent,
        WaitingForTurn,
        OnTurn,
        Promoting,
        Winner,
        Looser,
        Drawer
    }
}
