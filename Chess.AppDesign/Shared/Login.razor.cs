using Chess.AppDesign.IServices;
using Chess.Shared.Communications.Users;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Shared
{
    public partial class Login
    {
        [Inject] IUserService UserService { get; set; }
        LoginRequest loginRequest { get; set; } = new LoginRequest();
        RegisterRequest registerRequest { get; set; } = new RegisterRequest();

        private Task LogIn()
        {
            return UserService.Login(loginRequest);
        }
        private Task Register()
        {
            return UserService.Register(registerRequest);
        }
    }
}
