using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.Tools
{
    public partial class Tab
    {
        [CascadingParameter] public Tabs tabs { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public RenderFragment ChildContent { get; set; }

        protected override void OnInitialized()
        {
            tabs.AddTab(Title);
        }
    }
}
