using Chess.Core.Services;
using Chess.Entities;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Chess.WebAsm.Server.Controllers
{
    [Route("api/[controller]")]
    public abstract class AController<TService> : Controller
        where TService : Service
    {
        protected TService Service { get; set; }
        private IDatabase database;

        private readonly bool AuthorizationNeeded;

        public AController(IDatabase database, TService service, bool authorizationNeeded = false)
        {
            Service = service;
            this.database = database;
            AuthorizationNeeded = authorizationNeeded;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.ContainsKey(UserDto.HeaderTokenKey))
            {
                string token = context.HttpContext.Request.Headers[UserDto.HeaderTokenKey];

                if (database == null) return;

                Service.User = await database.GetUser(token);
            }

            if (AuthorizationNeeded)
            {
                if (Service.User == null)
                {
                    context.Result = new UnauthorizedResult();
                }
            }
            await next();
        }

    }
}
