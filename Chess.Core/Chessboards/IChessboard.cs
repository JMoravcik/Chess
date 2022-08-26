using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public interface IChessboard : IEnumerable<FieldRow>
    {
        internal bool IsFieldSafeFor(Player player, Field destinedField);
        public FieldRow this[int index] { get; }

        /// <summary>
        /// Gets Field by right chess coordinations 
        /// </summary>
        /// <param name="coordinates">string with to characters meaning coordination "1a" == field (0, 0) "8h" == field (7, 7) </param>
        /// <returns>field in destination (otherwise it will throw Exception) </returns>
        public Field Get(string coordinates);

        public void ResetAim();

        public int[][] GetChessboardSymbolMap();
    }
}
