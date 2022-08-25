using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.Tools
{
    public partial class TableCell
    {
        [CascadingParameter] public ITable Tables { get; set; }

        [Parameter] public RenderFragment ChildContent { get; set; }

        [Parameter] public string Title { get; set; }

        protected override void OnInitialized()
        {
            Tables.AddHeader(Title);
            base.OnInitialized();
        }
    }
}
