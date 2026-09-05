using System;
using System.Collections.Generic;

namespace Chess
{
    public class PawnMoveStrategy : IMoveStrategy
    {
        private IEnumerable<Position> GetDiagonalTargets(Position position, Team team)
        {
            int dr = team == Team.White ? 1 : -1;

            foreach (int df in new[] { -1, 1 })
            {
                if (position.TryOffset(df, dr, out var diagonal))
                    yield return diagonal;
            }
        }

        public IEnumerable<Position> GetAttackSquares(Position position, Board board)
        {
            if (!board.TryGetPiece(position, out var piece))
                throw new InvalidOperationException($"No piece at {position})");

            foreach (var diagonal in GetDiagonalTargets(position, piece.Team))
                yield return diagonal;
        }

        public IEnumerable<Position> GetPseudoMoves(Position position, Board board)
        {
            if (!board.TryGetPiece(position, out var piece))
                throw new InvalidOperationException($"No piece at {position})");

            int dr = piece.Team == Team.White ? 1 : -1;

            if (position.TryOffset(0, dr, out var oneForward) && board.IsEmpty(oneForward))
            {
                yield return oneForward;

                bool isStartingRank =
                    (piece.Team == Team.White && position.Rank == 1) ||
                    (piece.Team == Team.Black && position.Rank == 6);

                if (isStartingRank &&
                    position.TryOffset(0, dr * 2, out var twoForward) &&
                    board.IsEmpty(twoForward))
                {
                    yield return twoForward;
                }
            }

            //capture
            foreach (var diagonal in GetDiagonalTargets(position, piece.Team))
            {
                if (board.TryGetPiece(diagonal, out var targetPiece) &&
                    targetPiece.Team != piece.Team)
                {
                    yield return diagonal;
                }
            }

            //en passant
            if (board.EnPassantTarget is Position enPassantTarget)
            {
                foreach (int df in new[] { -1, 1 })
                {
                    if (position.TryOffset(df, dr, out var diagonal) &&
                        diagonal.Equals(enPassantTarget))
                    {
                        yield return diagonal;
                    }
                }
            }
        }
    }
}
