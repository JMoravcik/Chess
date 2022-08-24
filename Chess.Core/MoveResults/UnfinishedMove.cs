using Chess.Core.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.MoveResults
{
    /// <summary>
    /// Pawn has available upgrade
    /// </summary>
    public class UnfinishedMove : MoveResult 
    {
        public Pawn Pawn { get; }

        internal UnfinishedMove(Pawn pawn)
        {
            Pawn = pawn;
        }
    }
}
