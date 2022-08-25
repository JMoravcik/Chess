using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Shared
{
    public static class Routes
    {
        public const string ServerUrl = "https://localhost:7255";
        public const string ChessHub = "/chesshub";
        public const string User_Login = "login";
        public const string User_Register = "register";
        public const string User_LoginWithToken = "login-with-token";

        public static string GetUrl(string relativeUrl)
        {
            return $"{ServerUrl}/{relativeUrl}";
        }
    }
}
