using Chess.Core.Services;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Chess.WebAsm.Server.Hubs
{
    public class ChessHubFilter : IHubFilter
    {
        public ChessHubFilter(GameService gameService)
        {
            gameService = gameService;
        }

        private readonly GameService gameService;
        private readonly IDatabase database;

        public async ValueTask<object> InvokeMethodAsync(
            HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            Console.WriteLine($"Calling hub method '{invocationContext.HubMethodName}'");
            try
            {
                if (invocationContext.Context.Items.ContainsKey(invocationContext.Context.ConnectionId))
                {
                    gameService.User = invocationContext.Context.Items[invocationContext.Context.ConnectionId] as UserDto;
                }

                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception calling '{invocationContext.HubMethodName}': {ex}");
                throw;
            }
        }
    }
}
