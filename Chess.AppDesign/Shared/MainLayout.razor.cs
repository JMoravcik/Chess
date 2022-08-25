using Chess.AppDesign.IServices;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] IUserService UserService { get; set; }

        protected override void OnInitialized()
        {
            UserService.OnUserChanged += UpdateView;
            base.OnInitialized();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (UserService.IsLoggedIn == null)
                {
                    await UserService.DefineStatus();
                }
            }
            await base.OnAfterRenderAsync(firstRender);
        }

        public void UpdateView()
        {
            this.InvokeAsync(() => StateHasChanged());
        }

        public void Dispose()
        {
            UserService.OnUserChanged -= UpdateView;
        }
    }
}
