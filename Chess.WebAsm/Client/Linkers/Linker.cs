using Chess.Shared;
using Chess.Shared.Communications;
using Chess.WebAsm.Client.Services;
using System.Dynamic;

namespace Chess.WebAsm.Client.Linkers
{
    public abstract class Linker
    {
        public Linker(HttpService httpService)
        {
            HttpService = httpService;
        }

        private HttpService HttpService { get; }

        private string GetUrl(string relativeUrl)
        {
            string controller = GetType().Name.Replace("Linker", "").ToLower();
            string result =  $"{Routes.ServerUrl}/api/{controller}/{relativeUrl}";
            return result;
        }

        protected async Task<TResponse> Get<TResponse>(string relativeUrl)
            where TResponse : Response, new()
        {
            try
            {
                var response = await HttpService.Get<TResponse>(GetUrl(relativeUrl));
                return response;

            }
            catch (Exception e)
            {
                return new TResponse() { ErrorMessage = e.Message };
            }
        }

        protected async Task<TResponse> Post<TRequest, TResponse>(string relativeUrl, TRequest request)
            where TResponse : Response, new()
        {
            try
            {
                var response = await HttpService.Post<TRequest, TResponse>(GetUrl(relativeUrl), request);
                return response;
            }
            catch (Exception e)
            {
                return new TResponse() { ErrorMessage = e.Message };

            }
        }

        protected async Task<TResponse> Get<TRequest, TResponse>(string relativeUrl, TRequest request)
            where TResponse : Response, new()
        {
            try
            {
                var response = await HttpService.Get<TRequest, TResponse>(GetUrl(relativeUrl), request);
                return response;
            }
            catch (Exception e)
            {
                return new TResponse() { ErrorMessage = e.Message };
            }
        }
    }
}
