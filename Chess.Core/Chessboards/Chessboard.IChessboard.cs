using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public partial class Chessboard : IChessboard
    {
        public Field Get(string coordinates)
        {
            if (coordinates.Length != 2)
            {
                throw new Exception("too many characters in coordinates!");
            }

            coordinates = coordinates.ToLower();
            int row = 7 - (coordinates[0] - '1');
            int column = coordinates[1] - 'a';
            if (row < 0 || row > 7 
             || column < 0 || column > 7)
            {
                throw new Exception($"bad coordinates! '{coordinates}'");
            }

            return fieldTable[row][column];
        }

        public IEnumerator<FieldRow> GetEnumerator()
        {
            foreach (var fieldRow in fieldTable)
                yield return fieldRow;
        }

        public void ResetAim()
        {
            foreach (var fieldRow in fieldTable)
            {
                foreach (var field in fieldRow)
                {
                    field.BlackAim = false;
                    field.WhiteAim = false;
                }
            }
            black.AimFields();
            white.AimFields();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool IChessboard.IsFieldSafeFor(Player player, Field destinedField)
        {
            return IsFieldSafeFor(player, destinedField);
        }
    }
}
