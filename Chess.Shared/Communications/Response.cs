using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Communications
{
    public abstract class Response
    {
        public string ErrorMessage { get; set; }
    }
}
