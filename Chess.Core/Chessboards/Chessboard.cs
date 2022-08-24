using Chess.Core.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public partial class Chessboard 
    {
        FieldRow[] fieldTable = new FieldRow[8];

        public Player black { get; }
        public Player white { get; }

        internal Chessboard(Player black, Player white)
        {
            for (int i = 0; i < Game.Rows; i++)
            {
                fieldTable[i] = new FieldRow(i);
            }
            this.black = black;
            this.white = white;
        }

        public FieldRow this[int index]
        {
            get
            {
                if (index >= 0 && index < 8)
                {
                    return fieldTable[index];
                }
                return null;
            }
        }

        internal void Reset()
        {
            foreach (var fieldRow in fieldTable)
            {
                foreach (var field in fieldRow)
                {
                    field.Reset();
                    field.BlackAim = false;
                    field.WhiteAim = false;
                }
            }

            white.AimFields();
            black.AimFields();

        }

        internal void Update()
        {
            foreach (var fieldRow in fieldTable)
            {
                foreach (var field in fieldRow)
                {
                    field.Update();
                    field.BlackAim = false;
                    field.WhiteAim = false;
                }
            }
            white.AimFields();
            black.AimFields();
        }



        internal bool IsFieldSafeFor(Player player, Field destinedField)
        {
            return player.PlayerColor == PlayerColors.White ? !destinedField.BlackAim : !destinedField.WhiteAim;
        }

        private bool FieldIsAimedBy(Piece occupant, Field destinedField)
        {
            return occupant.ThisFieldIsAimed(destinedField);
        }


    }
}
