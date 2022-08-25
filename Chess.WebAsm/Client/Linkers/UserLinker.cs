using Chess.AppDesign.ILinkers;
using Chess.Shared;
using Chess.Shared.Communications.Users;
using Chess.WebAsm.Client.Services;

namespace Chess.WebAsm.Client.Linkers
{
    public class UserLinker : Linker, IUserLinker
    {
        public UserLinker(HttpService httpService) : base(httpService)
        {
        }

        public Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            return Post<LoginRequest, LoginResponse>(Routes.User_Login, loginRequest);
        }

        public Task<LoginWithTokenResponse> LoginWithToken(LoginWithTokenRequest loginWithTokenRequest)
        {
            return Post<LoginWithTokenRequest, LoginWithTokenResponse>(Routes.User_LoginWithToken, loginWithTokenRequest);
        }

        public Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            return Post<RegisterRequest, RegisterResponse>(Routes.User_Register, registerRequest);
        }

        public Task<LogoutResponse> Logout()
        {
            return Get<LogoutResponse>(Routes.User_Logout);
        }
    }
}
