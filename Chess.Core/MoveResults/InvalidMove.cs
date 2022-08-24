using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.MoveResults
{
    public class InvalidMove : MoveResult
    {
        public readonly string ErrorMessage;

        internal InvalidMove(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
    }
}
