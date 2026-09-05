using Chess;
using System;
using System.Linq;
using UnityEngine;

namespace ChessUI
{
    public class GameController : MonoBehaviour
    {
        public BoardView boardView;
        public PromotionSelectController promotionSelectController;
        public UndoUIController undoUIController;
        public ResetUIController resetUIController;
        public MoveAudioPlayer moveAudioPlayer;
        public ButtonsController buttonsController;
        public GameStatusView gameStatusView;
        private Game game;

        void Start()
        {
            game = new Game(Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

            if (promotionSelectController != null)
            {
                promotionSelectController.Init(this);
            }

            if (undoUIController != null)
            {
                undoUIController.Init(this);
            }

            if (resetUIController != null)
            {
                resetUIController.Init(this);
            }

            if (buttonsController != null)
            {
                buttonsController.Init(this);
            }

            boardView.Draw(game.CurrentBoard, game.LastMove);
            gameStatusView?.Render(game);
        }

        public void TryApplyMove(Position from, Position to)
        {
            if (game.IsGameOver())
            {
                boardView.UnselectAllSquare();
                return;
            }

            var promotionMoves = game.CurrentBoard
                .GetPseudoMoves()
                .Where(move => move.From.Equals(from) && move.To.Equals(to) && move.Promotion.HasValue)
                .ToList();

            if (promotionMoves.Count > 0)
            {
                boardView.UnselectAllSquare();

                if (promotionSelectController == null)
                {
                    throw new NullReferenceException("promotionSelectController is not assigned");
                }

                promotionSelectController.Open(from, to, game.CurrentBoard.SideToMove);
                return;
            }

            var candidateMoves = game.CurrentBoard
                .GetLegalMoves()
                .Where(move => move.From.Equals(from) && move.To.Equals(to))
                .ToList();

            if (candidateMoves.Count == 0)
            {
                boardView.UnselectAllSquare();
                return;
            }

            boardView.UnselectAllSquare();
            ApplyMove(candidateMoves[0]);
        }

        public void ApplyMove(Move move)
        {
            var result = game.ApplyMove(move);
            if (result == MoveResult.IllegalMove)
            {
                boardView.UnselectAllSquare();
                return;
            }

            var nextBoard = game.CurrentBoard;
            boardView.Draw(nextBoard, game.LastMove);
            gameStatusView?.Render(game);

            if (game.IsCheckMate())
            {
                HandleCheckMate(nextBoard);
            }

            if (game.IsDraw())
            {
                HandleDraw(nextBoard);
            }

            MoveSfxType sfxType = result switch
            {
                MoveResult.CheckMove => MoveSfxType.Check,
                MoveResult.CaptureMove => MoveSfxType.Capture,
                MoveResult.DefaultMove => MoveSfxType.Default,
                _ => throw new InvalidOperationException($"Unexpected move result: {result}")
            };

            moveAudioPlayer?.Play(sfxType);
        }

        public void ApplyPromotion(Position from, Position to, PieceType promotion)
        {
            ApplyMove(new Move(from, to, promotion));
        }

        private void HandleCheckMate(Board board)
        {
            var winningTeam = GetWinningTeam(board);
            var losingTeam = board.SideToMove;

            boardView.ShowCheckmateKings(
                board.GetKingPosition(winningTeam),
                board.GetKingPosition(losingTeam));

            Debug.Log($"Checkmate. Winner: {winningTeam}");
        }

        private void HandleDraw(Board board)
        {
            boardView.ShowDrawKings(
                board.GetKingPosition(Team.White),
                board.GetKingPosition(Team.Black));

            Debug.Log($"Draw.");
        }

        private Team GetWinningTeam(Board board)
        {
            return board.SideToMove == Team.White ? Team.Black : Team.White;
        }

        public bool IsPromotionSelectionOpen()
        {
            return promotionSelectController != null && promotionSelectController.IsOpen;
        }

        public Game GetGame()
        {
            return game;
        }

        public bool CanUndo()
            => game.BoardHistory.Count > 1;

        public void Undo()
        {
            promotionSelectController?.Close();
            boardView.UnselectAllSquare();
            game.Undo();
            boardView.Draw(game.CurrentBoard, game.LastMove);
            gameStatusView?.Render(game);
        }

        public void Reset()
        {
            promotionSelectController?.Close();
            boardView.UnselectAllSquare();
            game.Reset();
            boardView.Draw(game.CurrentBoard, game.LastMove);
            gameStatusView?.Render(game);
        }
    }
}
