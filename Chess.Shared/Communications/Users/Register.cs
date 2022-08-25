using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Communications.Users
{
    public class RegisterRequest
    {
        public string NickName { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }

    public class RegisterResponse : Response
    {
        public UserDto User { get; set; }
    }
}
