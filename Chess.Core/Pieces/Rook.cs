using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Pieces
{
    public class Rook : Piece
    {
        public const int PieceId = 2;

        internal bool CanDoCastling { get; set; }
        internal Rook(Player owner, IChessboard chessboard) : base(owner, chessboard)
        {
            CanDoCastling = true;
        }

        public override IEnumerable<Field> GetAvailableMoves()
        {
            foreach (var field in GetAvailableFields(0, 1)) yield return field;
            foreach (var field in GetAvailableFields(0, -1)) yield return field;
            foreach (var field in GetAvailableFields(1, 0)) yield return field;
            foreach (var field in GetAvailableFields(-1, 0)) yield return field;
        }

        internal void Castling()
        {
            int col = Col;
            int row = Row;
            Position.Occupant = null;
            Chessboard[row][col == 0 ? 3 : 5].Occupant = this;
        }

    }
}
