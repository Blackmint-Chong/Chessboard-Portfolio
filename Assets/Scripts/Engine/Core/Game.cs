using System;
using System.Collections.Generic;

namespace Chess
{
    public enum DrawReason
    {
        None,
        Stalemate,
        ThreefoldRepetition,
        FiftyMoveRule,
        InsufficientMaterial
    }

    public class Game
    {
        public List<Board> BoardHistory { get; }
        public List<Move> MoveHistory { get; }
        public Board CurrentBoard => BoardHistory[^1];
        public Move? LastMove => MoveHistory.Count > 0 ? MoveHistory[^1] : null;
        private readonly Board initialBoard;
        private readonly Dictionary<string, int> repetitionCounts;
        private readonly List<string> repetitionKeyHistory;

        public Game(Board initialBoard)
        {
            this.initialBoard = initialBoard;
            BoardHistory = new List<Board> { initialBoard };
            MoveHistory = new List<Move>();
            repetitionCounts = new Dictionary<string, int>();
            repetitionKeyHistory = new List<string>();

            RecordBoard(initialBoard);
        }

        public MoveResult ApplyMove(Move move)
        {
            if (IsGameOver()) return MoveResult.IllegalMove;
            if (!CurrentBoard.IsLegalMove(move)) return MoveResult.IllegalMove;

            MoveResult result = CurrentBoard.ApplyMove(move, out var nextBoard);
            BoardHistory.Add(nextBoard);
            MoveHistory.Add(move);
            RecordBoard(nextBoard);

            return result;
        }

        public void Undo()
        {
            if (BoardHistory.Count <= 1) throw new InvalidOperationException($"Cannot undo in initial board");

            RemoveCurrentBoardRecord();
            BoardHistory.RemoveAt(BoardHistory.Count - 1);
            MoveHistory.RemoveAt(MoveHistory.Count - 1);
        }

        public void Reset()
        {
            BoardHistory.Clear();
            MoveHistory.Clear();
            repetitionCounts.Clear();
            repetitionKeyHistory.Clear();

            BoardHistory.Add(initialBoard);
            RecordBoard(initialBoard);
        }

        public bool IsCheckMate()
            => CurrentBoard.IsCheckMate();

        public bool IsGameOver()
            => IsCheckMate() || IsDraw();

        public bool IsDraw()
            => GetDrawReason() != DrawReason.None;

        public DrawReason GetDrawReason()
        {
            if (IsThreefoldRepetition())
                return DrawReason.ThreefoldRepetition;
            if (CurrentBoard.IsStaleMate())
                return DrawReason.Stalemate;
            if (CurrentBoard.IsInsufficientMaterialDraw())
                return DrawReason.InsufficientMaterial;
            if (CurrentBoard.IsFiftyMoveRule())
                return DrawReason.FiftyMoveRule;

            return DrawReason.None;
        }

        public bool IsThreefoldRepetition()
        {
            var currentKey = repetitionKeyHistory[^1];
            return repetitionCounts[currentKey] >= 3;
        }

        private void RecordBoard(Board board)
        {
            var key = board.GetRepetitionKey();
            repetitionKeyHistory.Add(key);

            repetitionCounts.TryGetValue(key, out int count);
            repetitionCounts[key] = count + 1;
        }

        private void RemoveCurrentBoardRecord()
        {
            var key = repetitionKeyHistory[^1];
            repetitionKeyHistory.RemoveAt(repetitionKeyHistory.Count - 1);

            if (repetitionCounts[key] == 1)
            {
                repetitionCounts.Remove(key);
                return;
            }

            repetitionCounts[key]--;
        }
    }
}
