using Chess.Core.Managers;
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
        public static IServiceCollection UseChessCore(this IServiceCollection services)
        {
            services.AddSingleton<GameManager>();
            return services;
        }
    }
}
