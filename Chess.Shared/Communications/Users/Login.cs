using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Communications.Users
{
    public class LoginRequest
    {
        public string Nickname { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse : Response
    {
        public UserDto User { get; set; }
    }
}
