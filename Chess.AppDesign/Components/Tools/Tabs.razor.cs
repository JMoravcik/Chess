using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.Tools
{
    public partial class Tabs
    {
        [Parameter] public RenderFragment? ChildContent { get; set; }

        public string ActiveTab { get; set; }
        private List<string> tabs { get; set; } = new List<string>();

        public void AddTab(string tab)
        {
            tabs.Add(tab);
            if (tabs.Count == 1)
            {
                ActiveTab = tabs[0];
            }
            this.InvokeAsync(() => StateHasChanged());
        }

        private void tabClicked(string tab)
        {
            ActiveTab = tab;
        }
    }
}
