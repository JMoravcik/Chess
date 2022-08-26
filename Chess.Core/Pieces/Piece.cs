using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Pieces
{
    public abstract class Piece
    {
        public readonly Player Owner;
        protected readonly IChessboard Chessboard;

        protected Piece(Player owner, IChessboard chessboard)
        {
            Owner = owner;
            Owner.Pieces.Add(this);
            Chessboard = chessboard;
        }

        public Field Position { get; internal set; }

        public int Row => Position?.Row ?? -2;
        public int Col => Position?.Col ?? -2;

        public string GetChessCoordination() => $"{Row + 1}{(char)('A' + Col)}";

        private bool ValidateMoveTo(Field field)
        {
            foreach (var availableMove in GetAvailableMoves())
            {
                if (field == availableMove)
                {
                    return true;
                }
            }
            return false;
        }

        public abstract int GetId();

        public abstract IEnumerable<Field> GetAvailableMoves();

        public virtual bool ThisFieldIsAimed(Field field)
        {
            if (Position == null) return false;
            foreach (var availableMove in GetAvailableMoves())
            {
                if (availableMove == field)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets field if piece can move onto it
        /// </summary>
        /// <param name="row">index of fields row</param>
        /// <param name="col">index of fields column</param>
        /// <param name="field">output field if its moveable for this piece</param>
        /// <returns>true if piece can move onto this field, false if it is outside of chessboard or you have already your piece on this field</returns>
        protected bool GetAvailableField(int row, int col, out Field field)
        {
            field = Chessboard[row]?[col];
            return field != null && field.Occupant?.Owner != Owner;
        }

        protected IEnumerable<Field> GetAvailableFields(int dRow, int dCol)
        {
            for (int i = 1; GetAvailableField(Row + dRow*i, Col + i*dCol, out var field); i++)
            {
                if (field.Occupant == null)
                {
                    yield return field;
                }
                else
                {
                    yield return field;
                    yield break;
                } 
            }
        }

        internal void Kill()
        {
            if (Position != null)
            {
                Position.Occupant = null;
                Position = null;
            }
            Owner.Pieces.Remove(this);
        }

        public virtual IEnumerable<Piece> GetAimedOpponentsPieces()
        {
            if (Position == null) yield break;
            foreach (var field in GetAvailableMoves())
            {
                if (field.Occupant != null)
                    yield return field.Occupant;
            }
        }

        internal virtual void AimFields()
        {
            foreach (var field in GetAvailableMoves())
            {
                if (Owner.PlayerColor == PlayerColors.White)
                {
                    field.WhiteAim = true;
                }
                else
                {
                    field.BlackAim = true;
                }
            }
        }

        internal virtual bool MovePieceTo(Field field)
        {
            if (ValidateMoveTo(field))
            {
                Position.Occupant = null;
                field.Occupant = this;
                Chessboard.ResetAim();
                return true;
            }

            return false;
        }
    }
}
