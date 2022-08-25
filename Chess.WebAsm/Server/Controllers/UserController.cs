using Chess.Core.Services;
using Chess.Shared;
using Chess.Shared.Communications.Users;
using Chess.Shared.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chess.WebAsm.Server.Controllers
{
    public class UserController : AController<UserService>
    {
        public UserController(IDatabase database, UserService userService) : base(database, userService, false)
        {

        }

        [HttpPost(Routes.User_Register)]
        public async Task<RegisterResponse> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var response = await Service.Register(request);
                return response;
            }
            catch (Exception e)
            {
                return new() { ErrorMessage = e.Message };
            }
        }

        [HttpPost(Routes.User_Login)]
        public async Task<LoginResponse> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await Service.Login(request);
                return response;
            }
            catch (Exception e)
            {
                return new() { ErrorMessage = e.Message };
            }
        }

        [HttpPost(Routes.User_LoginWithToken)]
        public async Task<LoginWithTokenResponse> LoginWithToken([FromBody] LoginWithTokenRequest request)
        {
            try
            {
                LoginWithTokenResponse response = await Service.LoginWithToken(request);
                return response;
            }
            catch (Exception e)
            {
                return new() { ErrorMessage = e.Message };
            }
        }
    }
}
