using Chess.Core.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Core
{
    public delegate void PlayerStateChanged();
    public class Player
    {
        public event PlayerStateChanged PlayerStateChanged;

        private PlayerStates _playerState;
        public PlayerStates PlayerState 
        { 
            get => _playerState;
            internal set 
            {
                _playerState = value;
                PlayerStateChanged?.Invoke();
            } 
        }

        public PlayerColors PlayerColor { get; }

        private readonly Game game;
        public readonly string Name;

        internal Player(Game game, string name, PlayerColors playerColor)
        {
            Name = name;
            this.PlayerColor = playerColor;
        }

        internal List<Piece> Pieces = new List<Piece>();

        internal bool IsInCheck()
        {
            var king = GetPiece<King>();
            bool isAimed = PlayerColor == PlayerColors.White ? king.Position.BlackAim : king.Position.WhiteAim;
            return isAimed;
        }

        public IEnumerable<Piece> GetAllAimedOpponentsPieces()
        {
            foreach (var piece in Pieces)
            {
                foreach (var opponentsPiece in piece.GetAimedOpponentsPieces())
                {
                    yield return opponentsPiece;
                }
            }
        }

        internal void AimFields()
        {
            foreach (var piece in Pieces)
            {
                piece.AimFields();
            }
        }

        internal T GetPiece<T>()
            where T : Piece
        {
            return Pieces.FirstOrDefault(p => p.GetType() == typeof(T)) as T;
        }
    }
}
