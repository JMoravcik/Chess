using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.TopMenu
{
    public partial class MenuButton
    {
        [Parameter] public EventCallback Clicked { get; set; }
        [Parameter] public string Title { get; set; }
        private bool inProcess = false;
        private async Task OnClick()
        {
            inProcess = true;
            await Clicked.InvokeAsync();
            inProcess = false;
            this.InvokeAsync(() => StateHasChanged());
        }
    }
}
