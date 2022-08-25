using Chess.Shared.Communications.Users;
using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.IServices
{
    public delegate void UserChanged();
    public interface IUserService
    {
        event UserChanged OnUserChanged;

        /// <summary>
        /// null = undefined state on start
        /// </summary>
        bool? IsLoggedIn { get; }
        public UserDto User { get; }
        Task DefineStatus();

        Task Login(LoginRequest loginRequest);
        Task Register(RegisterRequest loginRequest);
    }
}
