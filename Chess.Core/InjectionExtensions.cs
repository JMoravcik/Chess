using Chess.Core.Managers;
using Chess.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public static class InjectionExtensions
    {
        public static IServiceCollection AddChessCore(this IServiceCollection services)
        {
            services.AddSingleton<GameManager>();
            services.AddScoped<UserService>();
            return services;
        }
    }
}
