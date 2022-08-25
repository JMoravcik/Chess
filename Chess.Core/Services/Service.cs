using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Services
{
    public abstract class Service
    {
        public UserDto User { get; set; }
        public Service(IDatabase database)
        {
            Database = database;
        }

        protected IDatabase Database { get; }
    }
}
