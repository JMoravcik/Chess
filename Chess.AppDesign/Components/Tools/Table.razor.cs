using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.Tools
{
    public interface ITable
    {
        public void AddHeader(string header);
    }
    public partial class Table<TRow> : ITable
    {
        [Parameter] public string Style { get; set; }
        [Parameter] public RenderFragment<TRow> Row { get; set; }

        [Parameter] public EventCallback<TRow> RowClicked { get; set; }
        
        [Parameter] public List<TRow> Data { get; set; } = new List<TRow>();

        private List<string> headers { get; set; } = new List<string>();

        public void AddHeader(string header)
        {
            if (!headers.Contains(header))
            {
                headers.Add(header);
                this.InvokeAsync(() => StateHasChanged());
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                InvokeAsync(() => StateHasChanged());
            }
        }

        private string RowClasses => RowClicked.HasDelegate ? "clickable" : "";

        private void OnRowClick(TRow row)
        {
            RowClicked.InvokeAsync(row);
        }

        private IEnumerable<TRow> GetRows()
        {
            if (Data == null || Data.Count == 0)
                yield break;
            foreach (var row in Data)
            {
                yield return row;
            }
        }
    }
}
