using Chess.AppDesign.ILinkers;
using Chess.AppDesign.IServices;
using Chess.Shared.Communications.Users;
using Chess.Shared.Dtos;
using Chess.Shared.Extensions;

namespace Chess.WebAsm.Client.Services
{
    public class UserService : IUserService
    {
        public bool? _isLoggedIn = null;
        public bool? IsLoggedIn 
        {
            get => _isLoggedIn;
            private set
            {
                _isLoggedIn = value;
                OnUserChanged?.Invoke();
            }
        }

        public UserDto? _user = null;
        public UserDto? User 
        {
            get => _user;
            private set
            {
                _user = value;
                if (value != null)
                {
                    httpService.SetToken(value.Token);
                    _ = memoryService.Set(UserDto.HeaderTokenKey, value.Token);
                    chessHubService.Start(value)
                        .ContinueWith(r =>
                        {
                            IsLoggedIn = true;
                        });
                }
                else
                {
                    _ = memoryService.Remove(UserDto.HeaderTokenKey);
                    chessHubService.Stop()
                        .ContinueWith(r =>
                        {
                            IsLoggedIn = false;
                        });
                }
            }
        }

        private readonly MemoryService memoryService;
        private readonly IJsService jsService;
        private readonly IUserLinker userLinker;
        private readonly HttpService httpService;
        private readonly IChessHubService chessHubService;

        public event UserChanged OnUserChanged;

        public UserService(MemoryService memoryService, IJsService jsService, IUserLinker userLinker, HttpService httpService, IChessHubService chessHubService)
        {
            this.memoryService = memoryService;
            this.jsService = jsService;
            this.userLinker = userLinker;
            this.httpService = httpService;
            this.chessHubService = chessHubService;
        }

        public async Task DefineStatus()
        {
            var userToken = await memoryService.Get<string>(UserDto.HeaderTokenKey);
            if (userToken != null)
            {
                var response = await userLinker.LoginWithToken(new LoginWithTokenRequest() { Token = userToken });
                User = response.User;
            }
            else
            {
                User = null;
            }     
        }

        public async Task Login(LoginRequest loginRequest)
        {
            var response = await userLinker.Login(loginRequest);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                await jsService.Alert(response.ErrorMessage);
                return;
            }
            User = response.User;
        }

        public async Task Register(RegisterRequest loginRequest)
        {
            var response = await userLinker.Register(loginRequest);
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                await jsService.Alert(response.ErrorMessage);
                return;
            }
            User = response.User;

        }

        public async Task Logout()
        {
            var response = await userLinker.Logout();
            if (!string.IsNullOrEmpty(response.ErrorMessage))
            {
                await jsService.Alert(response.ErrorMessage);
            }
            User = null;
        }
    }
}
