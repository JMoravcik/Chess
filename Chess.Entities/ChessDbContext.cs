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

        public ChessDbContext()
            : base()
        {
        }

        public ChessDbContext(DbContextOptions<ChessDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(@"Server=localhost;Database=chess-dev;Trusted_Connection=True;");
        }

        public async Task<UserDto> GetUser(string nickName, string password)
        {
            try
            {
                var userDb = await UserDbs.FirstOrDefaultAsync(u => u.NickName == nickName && u.Password == password);
                return (UserDto)userDb;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public async Task<UserDto> AddUser(UserDto userDto, string password)
        {
            var result = UserDbs.AddEntities<UserDb, UserDto>("[Program]", (UserDb)userDto);
            result[0].Password = password;
            await SaveChangesAsync();
            return userDto;
        }

        public async Task<UserDto> GetUser(string token)
        {
            var userDb = await UserDbs.FirstOrDefaultAsync(u => u.Token == token);
            return (UserDto)userDb;
        }

        public async Task<UserDto> UpdateUser(UserDto userDto)
        {
            var result = UserDbs.UpdateEntities<UserDb, UserDto>(userDto.NickName, (UserDb)userDto);
            await this.SaveChangesAsync();
            return userDto;
        }

        public Task<bool> UserExists(string nickName)
        {
            return UserDbs.AnyAsync(u => u.NickName == nickName);
        }
    }
}
