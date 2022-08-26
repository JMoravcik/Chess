using Chess.Core.MoveResults;
using Chess.Core.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Chess.Core.Tests")]
namespace Chess.Core
{
    public delegate void GameStateChanged(Game game);
    public class Game
    {
        public event GameStateChanged GameStateChanged;
        public const int Cols = 8;
        public const int Rows = 8;

        GameStates _gameStates;
        public GameStates GameState 
        {
            get => _gameStates;
            private set
            {
                _gameStates = value;
                if (value == GameStates.Playing)
                {
                    StartGame();
                }
                GameStateChanged?.Invoke(this);
            }
        }


        Player black, white;
        Player _currentlyPlaying;
        Player CurrentlyPlayingPlayer
        {
            get => _currentlyPlaying;
            set
            {
                _currentlyPlaying = value;
                _currentlyPlaying.PlayerState = PlayerStates.OnTurn;
                PlayerWaitingForTurn.PlayerState = PlayerStates.WaitingForTurn;
            }
        }
        Player PlayerWaitingForTurn => black == CurrentlyPlayingPlayer ? white : black;
        Chessboard chessboard { get; set; }

        public IChessboard Chessboard => chessboard;

        private readonly List<List<int>> map = null;

        private void StartGame()
        {
            PrepareChessboard();
            CurrentlyPlayingPlayer = white;
        }

        private void SwitchTurn()
        {
            CurrentlyPlayingPlayer = PlayerWaitingForTurn;
        }

        private void PrepareChessboard()
        {
            chessboard = new Chessboard(black, white);
            if (map == null)
            {
                SetDefaultChessboard();
            }
            else
            {
                SetCustomChessboard();
            }
        }

        private void SetCustomChessboard()
        {
            ValidateCustomMapFormat();
            for (int i = 0; i < Rows; i++)
            {
                var row = map[i];
                for (int j = 0; j < Cols; j++)
                {
                    int n = row[j];
                    chessboard[i][j].InitialPiece(CreatePiece(n));
                }
            }
        }

        private Piece CreatePiece(int id)
        {
            if (id == 0) return null;
            else if (id < 7)
            {
                return id switch
                {
                    Pawn.PieceId => new Pawn(white, Chessboard),
                    Rook.PieceId => new Rook(white, Chessboard),
                    Knight.PieceId => new Knight(white, Chessboard),
                    Bishop.PieceId => new Bishop(white, Chessboard),
                    Queen.PieceId => new Queen(white, Chessboard),
                    King.PieceId => new King(white, Chessboard),
                    _ => null
                };
            }
            else
            {
                id -= 6;
                return id switch
                {
                    Pawn.PieceId => new Pawn(black, Chessboard),
                    Rook.PieceId => new Rook(black, Chessboard),
                    Knight.PieceId => new Knight(black, Chessboard),
                    Bishop.PieceId => new Bishop(black, Chessboard),
                    Queen.PieceId => new Queen(black, Chessboard),
                    King.PieceId => new King(black, Chessboard),
                    _ => null
                };
            }

        }

        private void ValidateCustomMapFormat()
        {
            bool blackKing = false;
            bool whiteKing = false;
            if (map.Count != Rows) throw new Exception("Bad format of custom map (there is not 8 rows)");
            for (int i = 0; i < 8; i++)
            {
                var row = map[i];
                if (row.Count != Cols) throw new Exception($"Bad format of custom map (in row n.{i} there is not 8 rows)");
                for (int j = 0; j < Cols; j++)
                {
                    int n = row[j];
                    if (n < 0 || n > 12) throw new Exception($"Bad format of custom map (in row '{i}' column '{j}' there is invalid id of piece '{n}')");
                    if (n == King.PieceId) whiteKing = true;
                    else if (n - 6 == King.PieceId) blackKing = true;
                }
            }

            if (!whiteKing || !blackKing) throw new Exception("custom map have to have king of both side present! (id 6 and 12)");
        }

        private void SetDefaultChessboard()
        {
            for (int i = 0; i < 8; i++)
            {
                chessboard[1][i].InitialPiece(new Pawn(black, chessboard));
                chessboard[6][i].InitialPiece(new Pawn(white, chessboard));
            }
            chessboard[0][0].InitialPiece(new Rook(black, chessboard));
            chessboard[7][0].InitialPiece(new Rook(white, chessboard));
            chessboard[0][1].InitialPiece(new Knight(black, chessboard));
            chessboard[7][1].InitialPiece(new Knight(white, chessboard));
            chessboard[0][2].InitialPiece(new Bishop(black, chessboard));
            chessboard[7][2].InitialPiece(new Bishop(white, chessboard));
            chessboard[0][3].InitialPiece(new Queen(black, chessboard));
            chessboard[7][3].InitialPiece(new Queen(white, chessboard));
            chessboard[0][4].InitialPiece(new King(black, chessboard));
            chessboard[7][4].InitialPiece(new King(white, chessboard));
            chessboard[0][5].InitialPiece(new Bishop(black, chessboard));
            chessboard[7][5].InitialPiece(new Bishop(white, chessboard));
            chessboard[0][6].InitialPiece(new Knight(black, chessboard));
            chessboard[7][6].InitialPiece(new Knight(white, chessboard));
            chessboard[0][7].InitialPiece(new Rook(black, chessboard));
            chessboard[7][7].InitialPiece(new Rook(white, chessboard));

        }
        /// <summary>
        /// Create Game with default map
        /// </summary>
        public Game()
        {
        }

        /// <summary>
        /// Create Game with custom map
        /// </summary>
        /// <param name="map">map with ids of pieces on map
        /// Empty field = 0
        /// Pawn = 1 for white 7 for black
        /// Rook = 2 for white 8 for black
        /// Knight = 3 for white 9 for black
        /// Bishop = 4 for white 10 for black
        /// Queen = 5 for white 11 for black
        /// King = 6 for white 12 for black
        /// </param>
        public Game(List<List<int>> map)
        {
            this.map = map;
        }

        public Player JoinGame(string name)
        {
            if (GameState != GameStates.WaitingForPlayers) return null;

            if (black == null) 
            {
                black = new Player(this, name, PlayerColors.Black);
                black.PlayerState = PlayerStates.WaitingForOponent;
                return black;
            } 
            else if (white == null) 
            {
                white = new Player(this, name, PlayerColors.White);
                GameState = GameStates.Playing;
                return white;
            } else
            {
                return null;
            }
        }

        /// <summary>
        /// use when player is leaving match
        /// </summary>
        /// <param name="player">player leaving match</param>
        /// <returns>true if player successfully leaved, false if player was not even in game</returns>
        public bool LeftGame(Player player)
        {
            if (white == player)
            {
                white = null;
                if (black != null)
                {
                    black.PlayerState = PlayerStates.Winner;
                    GameState = GameStates.Ended;
                }
            }
            else if (black == player)
            {
                black = null;
                if (white != null)
                {
                    white.PlayerState = PlayerStates.Winner;
                    GameState = GameStates.Ended;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public MoveResult PlayerMove(Player player, Piece piece, string coordinates)
        {
            var field = Chessboard.Get(coordinates);
            return PlayerMove(player, piece, field);
        }


        internal MoveResult PlayerMove(Player player, string move)
        {
            string[] moveSplit = move.Split(' ');
            var fromField = Chessboard.Get(moveSplit[0]);
            if (fromField.Occupant == null) return new InvalidMove("You are moving empty place!");
            var toField = Chessboard.Get(moveSplit[1]);

            return PlayerMove(player, fromField.Occupant, toField);
        }

        public MoveResult PlayerMove(Player player, Piece piece, Field toField)
        {
            if (piece == null || toField == null) return new InvalidMove("piece or toField have no instance of object!");
            if (piece.Owner != player) new InvalidMove("you are using opponents piece!");
            if (GameState != GameStates.Playing)
            {
                return new InvalidMove("Game has already ended");
            }
            if (player != CurrentlyPlayingPlayer || player.PlayerState != PlayerStates.OnTurn)
            {
                return new InvalidMove("It is not your turn, yet");
            }
            if (!piece.MovePieceTo(toField))
            {
                return new InvalidMove("Invalid move for this piece!");
            }
            if (CurrentlyPlayingPlayer.IsInCheck())
            {
                chessboard.Reset();
                return new InvalidMove("Your move ends in check!");
            }

            if (piece is Pawn pawn && pawn.UpgradeAvailable)
            {
                player.PlayerState = PlayerStates.Promoting;
                return new UnfinishedMove(pawn);
            }
            return CompletePlayersMove();
        }

        public MoveResult UpgradePawnAndCompletePlayerMove(Player player, Pawn pawn, int pieceId)
        {
            switch (pieceId)
            {
                case Pawn.PieceId: return UpgradePawnAndCompletePlayerMove<Pawn>(player, pawn);
                case Rook.PieceId: return UpgradePawnAndCompletePlayerMove<Rook>(player, pawn);
                case Knight.PieceId: return UpgradePawnAndCompletePlayerMove<Knight>(player, pawn);
                case Bishop.PieceId: return UpgradePawnAndCompletePlayerMove<Bishop>(player, pawn);
                case Queen.PieceId: return UpgradePawnAndCompletePlayerMove<Queen>(player, pawn);
                case King.PieceId: return UpgradePawnAndCompletePlayerMove<King>(player, pawn);
                default: return new InvalidMove("Bad Move!");
            }
        }
        public MoveResult UpgradePawnAndCompletePlayerMove<T>(Player player, Pawn pawn)
            where T : Piece
        {
            if (!pawn.UpgradeAvailable) return new InvalidMove("This piece is not piece what is upgrading!");
            if (typeof(T) == typeof(Pawn) || typeof(T) == typeof(King))
            {
                return new InvalidMove($"You cannot promote {nameof(Pawn)} to {typeof(T).Name}");
            }

            pawn.PromoteTo<T>();
            return CompletePlayersMove();
        }


        private MoveResult CompletePlayersMove()
        {
            chessboard.Update();

            if (CurrentPlayerWonMatch())
            {
                CurrentlyPlayingPlayer.PlayerState = PlayerStates.Winner;
                PlayerWaitingForTurn.PlayerState = PlayerStates.Looser;
                GameState = GameStates.Ended;
            }
            else if (NextPlayerCannotMoveAnywhere())
            {
                CurrentlyPlayingPlayer.PlayerState = PlayerStates.Drawer;
                PlayerWaitingForTurn.PlayerState = PlayerStates.Drawer;
                GameState = GameStates.Ended;
            }
            else
            {
                SwitchTurn();
            }
            return new SuccessfullMove();
        }

        private bool NextPlayerCannotMoveAnywhere()
        {
            foreach (var piece in PlayerWaitingForTurn.Pieces)
            {
                var move = piece.GetAvailableMoves().FirstOrDefault();
                if (move != null) return false;
            }

            return true;
        }

        private bool CurrentPlayerWonMatch()
        {
            if (PlayerWaitingForTurn.IsInCheck())
            {
                foreach (var piece in PlayerWaitingForTurn.Pieces)
                {
                    if (CanPreventCheckmate(piece))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool CanPreventCheckmate(Piece piece)
        {
            foreach (var availableMove in piece.GetAvailableMoves())
            {
                piece.MovePieceTo(availableMove);
                bool MovePreventedCheckmate = !PlayerWaitingForTurn.IsInCheck();
                chessboard.Reset();
                if (MovePreventedCheckmate)
                {
                    return true;
                }
            }

            return false;
        }

        internal MoveResult UpgradePawnAndCompletePlayerMove(Player player, string pawnCoordination, int pieceId)
        {
            var field = Chessboard.Get(pawnCoordination);
            if (field.Occupant == null) return new InvalidMove("There is no pawn!");
            if (!(field.Occupant is Pawn pawn)) return new InvalidMove("There is some piece but its not pawn!");
            return UpgradePawnAndCompletePlayerMove(player, pawn, pieceId);
        }
    }
}
