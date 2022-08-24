using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Dtos
{
    public class UserDto : Dto
    {
        public string NickName { get; set; }
        public int Elo { get; set; }
    }
}
