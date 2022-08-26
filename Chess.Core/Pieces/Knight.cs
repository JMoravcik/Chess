using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Pieces
{
    public class Knight : Piece
    {
        public const int PieceId = 3;

        public override int GetId()
        {
            return Owner.PlayerColor == PlayerColors.White ? PieceId : PieceId + 6;
        }
        internal Knight(Player owner, IChessboard chessboard) : base(owner, chessboard)
        {
        }

        public override IEnumerable<Field> GetAvailableMoves()
        {
            Field field = null;
            if (GetAvailableField(Row - 1, Col + 2, out field)) yield return field;
            if (GetAvailableField(Row + 1, Col + 2, out field)) yield return field;
            if (GetAvailableField(Row - 1, Col - 2, out field)) yield return field;
            if (GetAvailableField(Row + 1, Col - 2, out field)) yield return field;
            if (GetAvailableField(Row + 2, Col - 1, out field)) yield return field;
            if (GetAvailableField(Row + 2, Col + 1, out field)) yield return field;
            if (GetAvailableField(Row - 2, Col + 1, out field)) yield return field;
            if (GetAvailableField(Row - 2, Col - 1, out field)) yield return field;
        }


    }
}
