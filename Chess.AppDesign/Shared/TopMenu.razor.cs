using Chess.AppDesign.IServices;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Shared
{
    public partial class TopMenu
    {
        [Inject] IUserService UserService { get; set; }

        private Task Logout()
        {
            return UserService.Logout();
        }
    }
}
