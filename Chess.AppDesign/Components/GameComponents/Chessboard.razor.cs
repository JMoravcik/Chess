using Chess.AppDesign.IServices;
using Chess.Shared;
using Chess.Shared.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.AppDesign.Components.GameComponents
{
    public partial class Chessboard : IDisposable
    {
        class Field
        {
            public Field(string coordinates, string imageUrl, int realRow, int realCol)
            {
                Coordinates = coordinates;
                ImageUrl = imageUrl;
                RealRow = realRow;
                RealCol = realCol;
            }

            public string Coordinates { get; }
            public string ImageUrl { get; set; }
            public int RealRow { get; }
            public int RealCol { get; }
        }

        [Inject] private IUserService userService { get; set; }
        [Inject] private IChessHubService chessHubService { get; set; }
        [Inject] private IJsService jsService { get; set; }

        private bool GameInteraction = false;

        Field[][] Rows { get; set; }
        PlayerColors playerColor { get; set; }

        protected override Task OnInitializedAsync()
        {
            chessHubService.GameStarted += ChessHubService_GameStarted;
            chessHubService.UserReconnected += ChessHubService_UserReconnected;
            chessHubService.GameMove += ChessHubService_GameMove;
            chessHubService.Promoting += ChessHubService_Promoting;
            chessHubService.InvalidMove += ChessHubService_InvalidMove;
            chessHubService.GameEnded += ChessHubService_GameEnded;
            chessHubService.GetData();
            return base.OnInitializedAsync();
        }

        private void ChessHubService_GameEnded(GameEndedData gameEndedData)
        {
            GameInteraction = false;
            PlayerStates state = gameEndedData.Player1.Id == userService.User.Id ? gameEndedData.Player1State : gameEndedData.Player2State;
            if (state == PlayerStates.Winner)
            {
                jsService.Alert("CONGRATZ!! You win.");
            }
            else if (state == PlayerStates.Looser)
            {
                jsService.Alert("You have lost this match");
            }
            else if (state == PlayerStates.Drawer)
            {
                jsService.Alert("This match is a draw");
            }
        }

        private void ChessHubService_InvalidMove(string errorMessage)
        {
            jsService.Alert(errorMessage);
        }

        private async Task PromoteTo(int id)
        {
            await chessHubService.Promotion(pawnMove, id);
            pawnMove = null;
        }


        string pawnMove = null;
        private void ChessHubService_Promoting(string pawn)
        {
            pawnMove = pawn;
            this.InvokeAsync(() => StateHasChanged());
        }

        private void ChessHubService_GameMove(string move)
        {
            var split = move.Split(' ');
            var field1 = GetField(split[0]);
            var field2 = GetField(split[1]);

            if (split.Length == 2)
            {
                field2.ImageUrl = field1.ImageUrl;
                field1.ImageUrl = string.Empty;
            }
            else if (split.Length == 3)
            {
                field1.ImageUrl = string.Empty;
                field2.ImageUrl = GetImageFromId(int.Parse(split[2]));
            }
            this.InvokeAsync(() => StateHasChanged());
        }

        Field GetField(string coords)
        {
            foreach (var row in Rows)
            {
                foreach (var field in row)
                {
                    if (field.Coordinates.ToLower() == coords.ToLower()) return field;
                }
            }
            return null;
        }

        private void ChessHubService_UserReconnected(GameData gameData)
        {
            if (Rows == null)
            {
                ChessHubService_GameStarted(gameData);
            }

        }

        private void ChessHubService_GameStarted(GameData gameData)
        {

            if (gameData.White.Id == userService.User.Id)
            {
                GameInteraction = true;
                playerColor = PlayerColors.White;
                Rows = new Field[8][];
                for (int i = 0; i < 8; i++)
                {
                    Rows[i] = new Field[8];
                    for (int j = 0; j < 8; j++)
                    {
                        string imageUrl = gameData.IdsMap[i][j] == 0 ? "" : GetImageFromId(gameData.IdsMap[i][j]);
                        Rows[i][j] = new Field($"{i + 1}{(char)('A' + j)}", imageUrl, i, j);
                    }
                }
            }
            else if (gameData.Black.Id == userService.User.Id)
            {
                GameInteraction = true;
                Rows = new Field[8][];
                playerColor = PlayerColors.Black;
                for (int i = 7; i >= 0; i--)
                {
                    Rows[i] = new Field[8];
                    for (int j = 0; j < 8; j++)
                    {
                        string imageUrl = gameData.IdsMap[i][j] == 0 ? "" : GetImageFromId(gameData.IdsMap[i][j]);
                        Rows[i][j] = new Field($"{i + 1}{(char)('A' + j)}", imageUrl, 7 - i, j);
                    }
                }
            }
            this.InvokeAsync(() => StateHasChanged());
        }

        private string GetFieldColor(Field field)
        {
            if ((field.RealCol + field.RealRow) % 2 == 0)
            {
                return playerColor == PlayerColors.White ? "#ffffff" : "#aaaaaa";
            }
            else
            {
                return playerColor == PlayerColors.White ? "#aaaaaa" : "#ffffff";
            }
        }

        private string GetImageFromId(int id) => id switch
        {
            1 => "./images/White_Pawn.svg",
            2 => "./images/White_Rook.svg",
            3 => "./images/White_Knight.svg",
            4 => "./images/White_Bishop.svg",
            5 => "./images/White_Queen.svg",
            6 => "./images/White_King.svg",
            7 => "./images/Black_Pawn.svg",
            8 => "./images/Black_Rook.svg",
            9 => "./images/Black_Knight.svg",
            10 => "./images/Black_Bishop.svg",
            11 => "./images/Black_Queen.svg",
            12 => "./images/Black_King.svg",
        };

        private async Task OnEmptyFieldClick(Field field)
        {
            if (!GameInteraction) return;
            if (choosenField == null) return;

            await chessHubService.PlayerMove($"{choosenField.Coordinates} {field.Coordinates}");
            choosenField = null;
            this.InvokeAsync(() => StateHasChanged());
        }

        private Field choosenField = null;
        private async Task OnFieldClick(Field field)
        {
            if (!GameInteraction) return;
            if (choosenField == field)
            {
                choosenField = null;
                return;
            }

            if (choosenField == null)
            {
                choosenField = field;
                return;
            }

            await chessHubService.PlayerMove($"{choosenField.Coordinates} {field.Coordinates}");
            choosenField = null;
            this.InvokeAsync(() => StateHasChanged());
        }

        public void Dispose()
        {
            chessHubService.GameStarted -= ChessHubService_GameStarted;
        }
    }
}
