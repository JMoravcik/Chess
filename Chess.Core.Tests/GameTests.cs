using Microsoft.VisualStudio.TestTools.UnitTesting;
using Chess.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess.Core.MoveResults;
using Newtonsoft.Json;

namespace Chess.Core.Tests
{
    [TestClass()]
    public class GameTests
    {
        public class Scenario
        {
            public string Name { get; set; }
            public List<List<int>> Map { get; set; }
            public List<string> Moves { get; set; }
        }

        Dictionary<Type, int> ExpectedCounts = new Dictionary<Type, int>()
        {
            {typeof(Pieces.Pawn),  8},
            {typeof(Pieces.Rook),  2},
            {typeof(Pieces.Knight),  2},
            {typeof(Pieces.Bishop),  2},
            {typeof(Pieces.Queen),  1},
            {typeof(Pieces.King),  1},
        };

        Game Game;
        [TestInitialize]
        public void StartUp()
        {
            Game = new Game();
        }

        [TestMethod()]
        public void JoinGameTest()
        {
            bool playerChangedStateCalled = false;
            var player1 = Game.JoinGame("John Doe");
            player1.PlayerStateChanged += () =>
            {
                playerChangedStateCalled = true;
            };
            Assert.AreEqual(player1.Name, "John Doe");
            Assert.AreEqual(player1.PlayerColor, PlayerColors.Black);
            Assert.AreEqual(player1.PlayerState, PlayerStates.WaitingForOponent);
            Assert.AreEqual(player1.Pieces.Count, 0);
            var player2 = Game.JoinGame("Jane Doe");
            Assert.AreEqual(player1.Name, "John Doe");
            Assert.AreEqual(player1.PlayerColor, PlayerColors.Black);
            Assert.AreEqual(player1.PlayerState, PlayerStates.WaitingForTurn);
            Assert.AreEqual(player1.Pieces.Count, 16);
            CheckCorrectCountsOfPieces(player1);
            Assert.IsTrue(playerChangedStateCalled);

            Assert.AreEqual(player2.Name, "Jane Doe");
            Assert.AreEqual(player2.PlayerColor, PlayerColors.White);
            Assert.AreEqual(player2.PlayerState, PlayerStates.OnTurn);
            Assert.AreEqual(player2.Pieces.Count, 16);

            CheckCorrectCountsOfPieces(player2);
            CheckInitialChessboardSetting(player1, player2);

        }

        #region JoinGame functions
        private void CheckInitialChessboardSetting(Player black, Player white)
        {
            CheckCorrectRook(black, 0, 0);
            CheckCorrectRook(black, 0, 7);
            CheckCorrectKnight(black, 0, 1);
            CheckCorrectKnight(black, 0, 6);
            CheckCorrectBishop(black, 0, 2);
            CheckCorrectBishop(black, 0, 5);
            CheckCorrectQueen(black, 0, 3);
            CheckCorrectKing(black, 0, 4);
            CheckCorrectPawn(black, 1, 0);
            CheckCorrectPawn(black, 1, 1);
            CheckCorrectPawn(black, 1, 2);
            CheckCorrectPawn(black, 1, 3);
            CheckCorrectPawn(black, 1, 4);
            CheckCorrectPawn(black, 1, 5);
            CheckCorrectPawn(black, 1, 6);
            CheckCorrectPawn(black, 1, 7);

            CheckCorrectRook(white, 7, 0);
            CheckCorrectRook(white, 7, 7);
            CheckCorrectKnight(white, 7, 1);
            CheckCorrectKnight(white, 7, 6);
            CheckCorrectBishop(white, 7, 2);
            CheckCorrectBishop(white, 7, 5);
            CheckCorrectQueen(white, 7, 3);
            CheckCorrectKing(white, 7, 4);
            CheckCorrectPawn(white, 6, 0);
            CheckCorrectPawn(white, 6, 1);
            CheckCorrectPawn(white, 6, 2);
            CheckCorrectPawn(white, 6, 3);
            CheckCorrectPawn(white, 6, 4);
            CheckCorrectPawn(white, 6, 5);
            CheckCorrectPawn(white, 6, 6);
            CheckCorrectPawn(white, 6, 7);
        }

        private void CheckCorrectPawn(Player player, int row, int col)
        {
            var Chessboard = Game.Chessboard;
            if (Chessboard[row][col].Occupant is Pieces.Pawn pawn)
            {
                Assert.AreEqual(pawn.Owner, player);
                Assert.AreEqual(pawn.Position, Chessboard[row][col]);
            }
            else Assert.Fail();
        }

        private void CheckCorrectRook(Player player, int row, int col)
        {
            var Chessboard = Game.Chessboard;
            if (Chessboard[row][col].Occupant is Pieces.Rook rook)
            {
                Assert.AreEqual(rook.Owner, player);
                Assert.IsTrue(rook.CanDoCastling);
                Assert.AreEqual(rook.Position, Chessboard[row][col]);
            }
            else Assert.Fail();
        }

        private void CheckCorrectKnight(Player player, int row, int col)
        {
            if (Game.Chessboard[row][col].Occupant is Pieces.Knight knight)
            {
                Assert.AreEqual(knight.Owner, player);
                Assert.AreEqual(knight.Position, Game.Chessboard[row][col]);
            }
            else Assert.Fail();
        }

        private void CheckCorrectBishop(Player player, int row, int col)
        {
            if (Game.Chessboard[row][col].Occupant is Pieces.Bishop bishop)
            {
                Assert.AreEqual(bishop.Owner, player);
                Assert.AreEqual(bishop.Position, Game.Chessboard[row][col]);
            }
            else Assert.Fail();
        }

        private void CheckCorrectQueen(Player player, int row, int col)
        {
            if (Game.Chessboard[row][col].Occupant is Pieces.Queen queen)
            {
                Assert.AreEqual(queen.Owner, player);
                Assert.AreEqual(queen.Position, Game.Chessboard[row][col]);
            }
            else Assert.Fail();
        }

        private void CheckCorrectKing(Player player, int row, int col)
        {
            var Chessboard = Game.Chessboard;
            if (Chessboard[row][col].Occupant is Pieces.King king)
            {
                Assert.AreEqual(king.Owner, player);
                Assert.IsTrue(king.CanDoCastling);
                Assert.AreEqual(king.Position, Chessboard[row][col]);
            }
            else Assert.Fail();
        }

        private void CheckCorrectCountsOfPieces(Player player)
        {
            Dictionary<Type, int> ActualCounts = new Dictionary<Type, int>();
            foreach (var piece in player.Pieces)
            {
                if (!ActualCounts.ContainsKey(piece.GetType()))
                {
                    ActualCounts.Add(piece.GetType(), 0);
                }
                ActualCounts[piece.GetType()]++;
            }
            foreach (var expectedCount in ExpectedCounts)
            {
                Assert.AreEqual(expectedCount.Value, ActualCounts[expectedCount.Key]);
            }

        }
        #endregion

        [TestMethod()]
        public void LeftGameTest()
        {
            var player1 = Game.JoinGame("John Doe");
            Game.LeftGame(player1);
            Assert.AreEqual(Game.GameState, GameStates.WaitingForPlayers);
            player1 = Game.JoinGame("John Doe");
            var player2 = Game.JoinGame("Jane Doe");
            Game.LeftGame(player1);
            Assert.AreEqual(Game.GameState, GameStates.Ended);
            Assert.AreEqual(player2.PlayerState, PlayerStates.Winner);
        }

        [TestMethod()]
        public void UpgradePawnAndCompletePlayerMoveTest()
        {
            Game = new Game(LoadPromotionMap());
            bool changedToPromoting = false;
            bool changedToWaitingForTurn = false;
            var black = Game.JoinGame("John Doe");
            var white = Game.JoinGame("Jane Doe");
            white.PlayerStateChanged += () =>
            {
                if (white.PlayerState == PlayerStates.Promoting)
                {
                    changedToPromoting = true;
                }
            };

            var pawn = Game.Chessboard.Get("7E").Occupant;
            Assert.AreEqual(pawn.GetType(), typeof(Pieces.Pawn));
            var result = Game.PlayerMove(white, pawn, "8E");
            Assert.IsTrue(changedToPromoting);
            white.PlayerStateChanged += () =>
            {
                if (white.PlayerState == PlayerStates.WaitingForTurn)
                {
                    changedToWaitingForTurn = true;
                }
            };

            if (result is UnfinishedMove unfinishedMove)
            {
                result = Game.UpgradePawnAndCompletePlayerMove<Pieces.Pawn>(white, unfinishedMove.Pawn);
                Assert.AreEqual(result.GetType(), typeof(InvalidMove));
                result = Game.UpgradePawnAndCompletePlayerMove<Pieces.King>(white, unfinishedMove.Pawn);
                Assert.AreEqual(result.GetType(), typeof(InvalidMove));
                result = Game.UpgradePawnAndCompletePlayerMove<Pieces.Queen>(white, unfinishedMove.Pawn);
                Assert.AreEqual(result.GetType(), typeof(SuccessfullMove));
                Assert.IsTrue(changedToWaitingForTurn);
            }
            else Assert.Fail();
        }

        [TestMethod]
        public void ChessScenariosTests()
        {
            List<Scenario> Scenarios;
            using (StreamReader streamReader = new StreamReader("Scenarios.txt"))
            {
                var json = streamReader.ReadToEnd();
                Scenarios = JsonConvert.DeserializeObject<List<Scenario>>(json);
            }
            foreach (var scenario in Scenarios)
            {
                Game = new Game(scenario.Map);
                var black = Game.JoinGame("John Doe");
                var white = Game.JoinGame("Jane Doe");
                TestScenario(scenario, white, black);
            }
        }

        private void TestScenario(Scenario scenario, Player white, Player black)
        {
            UnfinishedMove unfinishedMove = null;
            foreach (var move in scenario.Moves)
            {
                string[] parameters = move.Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries);
                Player currentPlayer = black.PlayerState == PlayerStates.WaitingForTurn ? white : black;
                if (parameters[0] == "playing")
                {
                    Assert.AreEqual(GameStates.Playing, Game.GameState, $"test case '{scenario.Name}' failed!");
                    continue;
                }
                else if (parameters[0] == "winner")
                {
                    Assert.AreEqual(GameStates.Ended, Game.GameState, $"test case '{scenario.Name}' failed!");
                    if (parameters[1] == "white")
                    {
                        Assert.AreEqual(PlayerStates.Winner, white.PlayerState, $"test case '{scenario.Name}' failed!");
                        Assert.AreEqual(PlayerStates.Looser, black.PlayerState, $"test case '{scenario.Name}' failed!");
                    }
                    else if (parameters[2] == "black")
                    {
                        Assert.AreEqual(PlayerStates.Winner, black.PlayerState, $"test case '{scenario.Name}' failed!");
                        Assert.AreEqual(PlayerStates.Looser, white.PlayerState, $"test case '{scenario.Name}' failed!");
                    }
                    else Assert.Fail($"test case '{scenario.Name}' failed!");
                    continue;
                }
                else if (parameters[0] == "draw")
                {
                    Assert.AreEqual(PlayerStates.Drawer, black.PlayerState, $"test case '{scenario.Name}' failed!");
                    Assert.AreEqual(PlayerStates.Drawer, white.PlayerState, $"test case '{scenario.Name}' failed!");
                    continue;
                }

                if (currentPlayer.PlayerState == PlayerStates.Promoting)
                {
                    var result = Game.UpgradePawnAndCompletePlayerMove(currentPlayer, unfinishedMove.Pawn, Convert.ToInt32(parameters[0]));

                    Assert.AreEqual(result.GetType().Name, parameters[1], $"test case '{scenario.Name}' failed!");
                }
                else
                {
                    var piece = Game.Chessboard.Get(parameters[0]).Occupant;

                    var result = Game.PlayerMove(currentPlayer, piece, Game.Chessboard.Get(parameters[1]));
                    Assert.AreEqual(result.GetType().Name, parameters[2], $"test case '{scenario.Name}' failed!");
                    if (result is UnfinishedMove unfinished)
                    {
                        unfinishedMove = unfinished;
                    }
                }
            }
        }


        private List<List<int>> DefaultMap()
        {
            return new List<List<int>>()
            {
                new List<int>() { 8, 9, 10, 11, 12, 10, 9, 8 },
                new List<int>() { 7, 7,  7,  7,  7,  7, 7, 7 },
                new List<int>() { 0, 0,  0,  0,  0,  0, 0, 0 },
                new List<int>() { 0, 0,  0,  0,  0,  0, 0, 0 },
                new List<int>() { 0, 0,  0,  0,  0,  0, 0, 0 },
                new List<int>() { 0, 0,  0,  0,  0,  0, 0, 0 },
                new List<int>() { 1, 1,  1,  1,  1,  1, 1, 1 },
                new List<int>() { 2, 3,  4,  5,  6,  4, 3, 2 },
            };
        }

        private List<List<int>> LoadPromotionMap()
        {
            string json;
            using (StreamReader streamReader = new StreamReader("PromotionMap.txt"))
                json = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<List<int>>>(json);
        }

        private List<List<List<int>>> LoadPromotionMaps()
        {
            string json;
            using (StreamReader streamReader = new StreamReader("Scenarios.txt"))
                json = streamReader.ReadToEnd();
            return JsonConvert.DeserializeObject<List<List<List<int>>>>(json);
        }


    }
}