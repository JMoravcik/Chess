using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.Edits
{
    public enum StringEditTypes
    {
        text,
        password
    }
    public partial class StringEdit
    {
        [Parameter] public StringEditTypes Type { get; set; } = StringEditTypes.text;
        private void OnChange(ChangeEventArgs args)
        {
            string val = args.Value?.ToString();
            value = val;
            valueChanged.InvokeAsync(value);
        }
    }
}
