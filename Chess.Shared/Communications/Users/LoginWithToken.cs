using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Communications.Users
{
    public class LoginWithTokenRequest
    {
        public string Token { get; set; }
    }

    public class LoginWithTokenResponse : Response
    {
        public UserDto User { get; set; }
    }
}
