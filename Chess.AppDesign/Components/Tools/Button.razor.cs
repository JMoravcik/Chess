using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.Tools
{
    public partial class Button
    {
        [Parameter] public string Title { get; set; }
        [Parameter] public EventCallback Clicked { get; set; }
        [Parameter] public bool Disable { get; set; } = false;

        private bool disabled => Disable || inProcess;

        bool inProcess = false;
        private async Task OnClick()
        {
            inProcess = true;
            await Clicked.InvokeAsync();
            inProcess = false;
            this.InvokeAsync(() => StateHasChanged());
        }

    }
}
