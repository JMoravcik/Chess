using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Pieces
{
    public class Pawn : Piece
    {
        public const int PieceId = 1;

        public bool UpgradeAvailable = false;

        internal Pawn(Player owner, IChessboard chessboard) : base(owner, chessboard)
        {
        }

        public override IEnumerable<Field> GetAvailableMoves()
        {
            Field field;
            if (Owner.PlayerColor == PlayerColors.Black)
            {
                if (GetAvailableField(Row + 1, Col, out field) && field.Occupant == null ) yield return field;
                if (Row == 1 && GetAvailableField(Row + 2, Col, out field) && field.Occupant == null ) yield return field;
                if (GetAvailableField(Row + 1, Col + 1, out field) && ( field.Occupant != null || field.EnPassant != null )) yield return field;
                if (GetAvailableField(Row + 1, Col - 1, out field) && ( field.Occupant != null || field.EnPassant != null )) yield return field;
            }
            else
            {
                if (GetAvailableField(Row - 1, Col, out field) && field.Occupant == null) yield return field;
                if (Row == 6 && GetAvailableField(Row - 2, Col, out field) && field.Occupant == null) yield return field;
                if (GetAvailableField(Row - 1, Col + 1, out field) && (field.Occupant != null || field.EnPassant != null)) yield return field;
                if (GetAvailableField(Row - 1, Col - 1, out field) && (field.Occupant != null || field.EnPassant != null)) yield return field;

            }
        }

        internal override void AimFields()
        {
            Field field;
            if (Owner.PlayerColor == PlayerColors.Black)
            {
                if (GetAvailableField(Row + 1, Col + 1, out field) && (field.Occupant != null || field.EnPassant != null)) field.BlackAim = true;
                if (GetAvailableField(Row + 1, Col - 1, out field) && (field.Occupant != null || field.EnPassant != null)) field.BlackAim = true;
            }
            else
            {
                if (GetAvailableField(Row - 1, Col + 1, out field) && (field.Occupant != null || field.EnPassant != null)) field.WhiteAim = true;
                if (GetAvailableField(Row - 1, Col - 1, out field) && (field.Occupant != null || field.EnPassant != null)) field.WhiteAim = true;
            }

        }

        internal override bool MovePieceTo(Field field)
        {
            int row = field.Row - Row;
            var result = base.MovePieceTo(field);
            if (result)
            {
                if (field.EnPassant != null)
                {
                    field.EnPassant.Kill();
                }
                if (Math.Abs(row) == 2)
                {
                    Chessboard[row < 0 ? Row + 1 : Row - 1][Col].EnPassant = this;
                }
                if (field.Row == ( Owner.PlayerColor == PlayerColors.Black ? 7 : 0))
                {
                    UpgradeAvailable = true;
                }
            }

            return result;
        }

        internal void PromoteTo<T>() where T : Piece
        {
            var piece = CreateInstanceOf<T>();
            Position.Occupant = piece;
            this.Kill();
            Chessboard.ResetAim();
        }

        private Piece CreateInstanceOf<T>() where T : Piece
        {
            if (typeof(T) == typeof(Queen)) return new Queen(Owner, Chessboard);
            else if (typeof(T) == typeof(Rook)) return new Rook(Owner, Chessboard);
            else if (typeof(T) == typeof(Knight)) return new Knight(Owner, Chessboard);
            else if (typeof(T) == typeof(Bishop)) return new Bishop(Owner, Chessboard);
            else throw new NotImplementedException("Pawn cannot be promoted into this type of piece");
        }
    }
}
