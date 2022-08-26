using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Pieces
{
    public class King : Piece
    {
        public const int PieceId = 6;

        public override int GetId()
        {
            return Owner.PlayerColor == PlayerColors.White ? PieceId : PieceId + 6;
        }
        internal bool CanDoCastling { get; set; }

        internal King(Player owner, IChessboard chessboard) : base(owner, chessboard)
        {
            CanDoCastling = true;
        }

        public override IEnumerable<Field> GetAvailableMoves()
        {
            Field field = null;
            if (GetAvailableField(Row - 1, Col, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (GetAvailableField(Row + 1, Col, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (GetAvailableField(Row - 1, Col + 1, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (GetAvailableField(Row + 1, Col + 1, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (GetAvailableField(Row - 1, Col - 1, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (GetAvailableField(Row + 1, Col - 1, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (GetAvailableField(Row, Col + 1, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (GetAvailableField(Row, Col - 1, out field) && Chessboard.IsFieldSafeFor(Owner, field)) yield return field;
            if (CanDoCastling && Chessboard.IsFieldSafeFor(Owner, Position))
            {
                foreach (var piece in Owner.Pieces)
                {
                    if (piece is Rook rook && CastlingIsAvailable(rook, out field))
                    {
                        yield return field;
                    }
                }
            }
        }

        private bool CastlingIsAvailable(Rook rook, out Field field)
        {
            int dCol = rook.Col - Col;
            dCol /= Math.Abs(dCol);
            field = Chessboard[Row][Col + dCol * 2];
            return rook.CanDoCastling && pathBetweenRookAndKingIsClean(rook) && pathIsSafe(rook);
        }

        public override IEnumerable<Piece> GetAimedOpponentsPieces()
        {
            Field field = null;
            if (GetAvailableField(Row - 1, Col, out field) && field.Occupant != null) yield return field.Occupant;
            if (GetAvailableField(Row + 1, Col, out field) && field.Occupant != null) yield return field.Occupant;
            if (GetAvailableField(Row - 1, Col + 1, out field) && field.Occupant != null) yield return field.Occupant;
            if (GetAvailableField(Row + 1, Col + 1, out field) && field.Occupant != null) yield return field.Occupant;
            if (GetAvailableField(Row - 1, Col - 1, out field) && field.Occupant != null) yield return field.Occupant;
            if (GetAvailableField(Row + 1, Col - 1, out field) && field.Occupant != null) yield return field.Occupant;
            if (GetAvailableField(Row, Col + 1, out field) && field.Occupant != null) yield return field.Occupant;
            if (GetAvailableField(Row, Col - 1, out field) && field.Occupant != null) yield return field.Occupant;
        }

        public override bool ThisFieldIsAimed(Field field)
        {
            Field availableField  = null;
            if (GetAvailableField(Row - 1, Col, out availableField) && availableField == field) return true;
            if (GetAvailableField(Row + 1, Col, out availableField) && availableField == field) return true;
            if (GetAvailableField(Row - 1, Col + 1, out availableField) && availableField == field) return true;
            if (GetAvailableField(Row + 1, Col + 1, out availableField) && availableField == field) return true;
            if (GetAvailableField(Row - 1, Col - 1, out availableField) && availableField == field) return true;
            if (GetAvailableField(Row + 1, Col - 1, out availableField) && availableField == field) return true;
            if (GetAvailableField(Row, Col + 1, out availableField) && availableField == field) return true;
            if (GetAvailableField(Row, Col - 1, out availableField) && availableField == field) return true;
            return false;
        }

        private bool pathIsSafe(Rook rook)
        {
            int dCol = rook.Col - Col;
            dCol /= Math.Abs(dCol);
            return Chessboard.IsFieldSafeFor(Owner, Chessboard[Row][Col + dCol]);
        }

        private bool pathBetweenRookAndKingIsClean(Rook rook)
        {
            int dCol = rook.Col - Col;
            dCol /= Math.Abs(dCol);
            foreach (var availableField in GetAvailableFields(0, dCol))
            {
                if (availableField.Occupant != null) return false;
                if (availableField.Row == rook.Row && availableField.Col == rook.Col - dCol) return true;
            }
            return false;
        }

        internal override bool MovePieceTo(Field field)
        {
            int distance = field.Col - Col;
            var result = base.MovePieceTo(field);
            if (result && Math.Abs(distance) == 2)
            {
                Rook movingRook = null;
                if (distance < 0)
                {
                    movingRook = Owner.Pieces.Find(piece => piece is Rook rook && rook.Col == 0) as Rook;
                    movingRook.Castling();
                }
                else
                {
                    movingRook = Owner.Pieces.Find(piece => piece is Rook rook && rook.Col == 7) as Rook;
                    movingRook.Castling();
                }
            }
            return result;
        }
    }
}
