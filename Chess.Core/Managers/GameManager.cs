using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Managers
{
    public class GameManager
    {
        Dictionary<Guid, Game> Games { get; set; } = new Dictionary<Guid, Game>();

    }
}
