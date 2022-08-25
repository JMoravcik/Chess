using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.Edits
{
    public partial class Edit<TValue>
    {
        [Parameter] public TValue value { get; set; }
        [Parameter] public EventCallback<TValue> valueChanged { get; set; }
        [Parameter] public string Label { get; set; }
    }
}
