using Chess.Shared.Communications.Users;
using Chess.Shared.Dtos;
using Chess.Shared.Extensions;
using Chess.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Services
{
    public class UserService : Service
    {
        public UserService(IDatabase database) : base(database)
        {
        }

        public async Task SetToken(UserDto userDto)
        {
            userDto.Token = Guid.NewGuid().ToString();
            await Database.UpdateUser(userDto);
        }

        public async Task<RegisterResponse> Register(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.NickName))
            {
                return new() { ErrorMessage = "You have to set nick-name!" };
            }
            else if (string.IsNullOrEmpty(registerRequest.Password))
            {
                return new() { ErrorMessage = "You have to set password!" };
            }
            else if (registerRequest.Password != registerRequest.RepeatPassword)
            {
                return new() { ErrorMessage = "Passwords don't match!" };
            }
            else if (registerRequest.Password.Length < 8 || registerRequest.Password.Length > 15)
            {
                return new() { ErrorMessage = "Bad password length! Password have to be between 8 - 15 characters long" };
            }
            else if (await Database.UserExists(registerRequest.NickName))
            {
                return new() { ErrorMessage = "This nick name is already registered!" };
            }

            var user = await Database.AddUser(new Shared.Dtos.UserDto()
            {
                NickName = registerRequest.NickName,
                Elo = 900,
            }, registerRequest.Password.ToSha256());
            await SetToken(user);
            return new()
            {
                User = user
            };
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            var user = await Database.GetUser(loginRequest.Nickname, loginRequest.Password);
            if (user == null)
            {
                return new LoginResponse()
                {
                    ErrorMessage = "wrong nickname or password"
                };
            }
            else
            {
                return new LoginResponse()
                {
                    User = user,
                };
            }
        }

        public async Task<LoginWithTokenResponse> LoginWithToken(LoginWithTokenRequest request)
        {
            var user = await Database.GetUser(request.Token);
            if (user == null)
            {
                return new LoginWithTokenResponse()
                {
                    ErrorMessage = "token is already expired!"
                };
            }
            else
            {
                return new LoginWithTokenResponse()
                {
                    User = user,
                };
            }
        }
    }
}
