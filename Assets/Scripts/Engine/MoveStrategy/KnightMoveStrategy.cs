using System;
using System.Collections.Generic;

namespace Chess
{
    public class KnightMoveStrategy : IMoveStrategy
    {
        private IEnumerable<Position> GetMovingSquares(Position position)
        {
            (int df, int dr)[] KnightOffsets =
            {
                    (1, 2), (2, 1),
                    (-1, 2), (-2, 1),
                    (1, -2), (2, -1),
                    (-1, -2), (-2, -1)
            };
            foreach ((int df, int dr) in KnightOffsets)
            {
                if (position.TryOffset(df, dr, out var result))
                    yield return result;
            }
        }

        public IEnumerable<Position> GetPseudoMoves(Position position, Board board)
        {
            if (!board.TryGetPiece(position, out var piece))
                throw new InvalidOperationException($"No piece at {position}");

            foreach (var targetSquare in GetMovingSquares(position))
            {
                if (!board.TryGetPiece(targetSquare, out var targetPiece) || targetPiece.Team != piece.Team)
                    yield return targetSquare;
            }
        }

        public IEnumerable<Position> GetAttackSquares(Position position, Board board)
            => GetMovingSquares(position);
    }
}