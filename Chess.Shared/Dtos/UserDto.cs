using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Dtos
{
    public class UserDto : Dto
    {
        public const string HeaderTokenKey = "userTokenKey";
        public string? Token { get; set; }
        public string NickName { get; set; }
        public int Elo { get; set; }
        public Guid? Game { get; set; }
    }
}
