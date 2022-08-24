using Chess.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared.Interfaces
{
    public interface IDatabase
    {
        Task<UserDto> GetUser(string nickName, string password);
        Task<UserDto> GetUser(string token);
        Task<UserDto> AddUser(UserDto userDto);
    }
}
