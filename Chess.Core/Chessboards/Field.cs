using Chess.Core.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public delegate void FieldChanged();
    public class Field
    {
        public event FieldChanged FieldChanged;
        public readonly int Row, Col;
        public bool BlackAim { get; internal set; }
        public bool WhiteAim { get; internal set; }


        private Pawn nextEnpass = null;
        private Pawn currentEnpass = null;
        internal Pawn EnPassant
        { 
            get => currentEnpass; 
            set => nextEnpass = value; 
        }

        internal Field(int row, int col)
        {
            Row = row;
            Col = col;
        }

        private Piece _occupant = null;
        public Piece Occupant 
        { 
            get => _occupant; 
            internal set
            {

                if (_occupant != null && _occupant.Position == this)
                {
                    _occupant.Position = null;
                }
                _occupant = value;
                if (_occupant != null)
                {
                    _occupant.Position = this;
                }
            }
        }

        private Piece prevOccupant;

        internal void Update()
        {
            currentEnpass = nextEnpass;
            nextEnpass = null;
            if (prevOccupant != Occupant)
            {
                if (Occupant is King king)
                {
                    king.CanDoCastling = false;
                }
                else if (Occupant is Rook rook)
                {
                    rook.CanDoCastling = false;
                }

                if (prevOccupant != null && Occupant != null)
                {
                    prevOccupant.Kill();
                }
                prevOccupant = Occupant;
                FieldChanged?.Invoke();
            }
        }

        internal void Reset()
        {
            EnPassant = null;
            if (prevOccupant != Occupant)
            {
                Occupant = prevOccupant;
            }
        }

        internal void InitialPiece(Piece piece)
        {
            prevOccupant = piece;
            Occupant = piece;
            if (piece is Rook rook)
            {
                int row = piece.Owner.PlayerColor == PlayerColors.Black ? 0 : 7;
                if (this.Row != row || (Col != 0 && Col != 7))
                {
                    rook.CanDoCastling = false;
                }
            }
            else if (piece is King king)
            {
                int row = piece.Owner.PlayerColor == PlayerColors.Black ? 0 : 7;
                if (this.Row != row || Col != 4) king.CanDoCastling = false;
            }
        }
    }
}
