using Chess.Shared.Communications.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.ILinkers
{
    public interface IUserLinker
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
        Task<LoginWithTokenResponse> LoginWithToken(LoginWithTokenRequest loginWithTokenRequest);
        Task<RegisterResponse> Register(RegisterRequest registerRequest);
        Task<LogoutResponse> Logout();
    }
}
