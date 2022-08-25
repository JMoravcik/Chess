using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entities.Models
{
    public partial class UserDb : Model<UserDto>
    {
        public string NickName { get; set; }
        public string Password { get; set; }
        public int Elo { get; set; }
        public string? Token { get; set; }
    }
}
