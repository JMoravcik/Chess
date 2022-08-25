using Chess.Core.Managers;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core.Services
{
    public class GameService : Service
    {
        private readonly GameManager gameManager;

        public GameService(IDatabase database) : base(database)
        {
            this.gameManager = gameManager;
        }

    }
}
