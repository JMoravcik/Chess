using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Data
{
    public class GameData
    {
        public int[][] IdsMap { get; set; }
        public UserDto Black { get; set; }
        public UserDto White { get; set; }
    }
}
