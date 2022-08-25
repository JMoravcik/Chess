using Chess.AppDesign.IServices;
using Microsoft.JSInterop;

namespace Chess.WebAsm.Client.Services
{
    public class JsService : IJsService
    {
        private readonly IJSRuntime jsRuntime;

        public JsService(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public async Task Alert(string message)
        {
            await jsRuntime.InvokeVoidAsync("alert", message);
        }
    }
}
