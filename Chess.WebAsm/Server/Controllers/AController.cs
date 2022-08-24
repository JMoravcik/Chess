using Chess.Entities;
using Chess.Shared.Dtos;
using Chess.Shared.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Chess.WebAsm.Server.Controllers
{
    [ApiController]
    public abstract class AController : Controller
    {
        protected UserDto User { get; set; }
        private IDatabase database;

        private readonly bool AuthorizationNeeded;

        public AController(IDatabase database, bool authorizationNeeded = false)
        {
            this.database = database;
            AuthorizationNeeded = authorizationNeeded;
        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.HttpContext.Request.Headers.ContainsKey(UserDto.HeaderTokenKey))
            {
                string token = context.HttpContext.Request.Headers[UserDto.HeaderTokenKey];

                if (database == null) return;

                User = await database.GetUser(token);
            }

            if (AuthorizationNeeded)
            {
                if (User == null)
                {
                    context.Result = new UnauthorizedResult();
                }
            }

            await next();

            base.OnActionExecutionAsync(context, next);
        }

    }
}
