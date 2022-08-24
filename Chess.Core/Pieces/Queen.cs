using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Pieces
{
    public class Queen : Piece
    {
        public const int PieceId = 5;

        internal Queen(Player owner, IChessboard chessboard) : base(owner, chessboard)
        {
            
        }

        public override IEnumerable<Field> GetAvailableMoves()
        {
            foreach (var field in GetAvailableFields(1, 1)) yield return field;
            foreach (var field in GetAvailableFields(1, -1)) yield return field;
            foreach (var field in GetAvailableFields(-1, 1)) yield return field;
            foreach (var field in GetAvailableFields(-1, -1)) yield return field;
            foreach (var field in GetAvailableFields(0, 1)) yield return field;
            foreach (var field in GetAvailableFields(0, -1)) yield return field;
            foreach (var field in GetAvailableFields(1, 0)) yield return field;
            foreach (var field in GetAvailableFields(-1, 0)) yield return field;
        }
    }
}
