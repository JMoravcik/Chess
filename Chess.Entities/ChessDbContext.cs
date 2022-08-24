using AutoMapper;
using Chess.Entities.Models;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Entities
{
    public class ChessDbContext : DbContext, IDatabase
    {
        public DbSet<UserDb> UserDbs { get; set; }

        public async Task<UserDto> GetUser(string nickName, string password)
        {
            var userDb = await UserDbs.FirstOrDefaultAsync(u => u.NickName == nickName && u.Password == password);
            return (UserDto)userDb;
        }

        public async Task<UserDto> AddUser(UserDto userDto)
        {
            UserDbs.AddEntities("[Program]", userDto);
            await SaveChangesAsync();

            return userDto;
        }


    }
}
