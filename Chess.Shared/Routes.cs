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
        public const string User_Logout = "logout";

        public const string ChessHubServer_Connected = "Connected";
        public const string ChessHubServer_GetActiveUserList = "GetActiveUserList";

        public const string ChessHubClient_StartGame = "StartGame";
        public const string ChessHubClient_ActiveUserList = "ActiveUserList";
        public const string ChessHubClient_UserConnected = "UserConnected";
        public const string ChessHubClient_UserDisconnected = "UserDisconnected";
        public const string ChessHubClient_UserReconnected = "UserReconnected";
        public const string ChessHubClient_UserLeft = "UserLeft";

        public static string GetUrl(string relativeUrl)
        {
            return $"{ServerUrl}/{relativeUrl}";
        }
    }
}
