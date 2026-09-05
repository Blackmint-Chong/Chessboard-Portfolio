using System;
using System.Collections.Generic;

namespace Chess
{
    public class KingMoveStrategy : IMoveStrategy
    {
        private IEnumerable<Position> GetAroundSquares(Position position)
        {
            (int df, int dr)[] KingOffsets =
            {
                    (-1, 1), (0, 1), (1, 1),
                    (-1, 0), (1, 0),
                    (-1, -1), (0, -1), (1, -1)
            };

            foreach ((int df, int dr) in KingOffsets)
            {
                if (position.TryOffset(df, dr, out var result))
                    yield return result;
            }
        }

        public IEnumerable<Position> GetAttackSquares(Position position, Board board)
        {
            if (!board.TryGetPiece(position, out var piece))
                throw new InvalidOperationException($"No piece at {position})");

            foreach (var target in GetAroundSquares(position))
                yield return target;
        }

        public IEnumerable<Position> GetPseudoMoves(Position position, Board board)
        {
            if (!board.TryGetPiece(position, out var piece))
                throw new InvalidOperationException($"No piece at {position})");

            foreach (var aroundPosition in GetAroundSquares(position))
            {
                if (!board.TryGetPiece(aroundPosition, out var targetPiece) || targetPiece.Team != piece.Team)
                    yield return aroundPosition;
            }

            Team opponentTeam = piece.Team == Team.White ? Team.Black : Team.White;
            if (!board.IsInCheck(piece.Team))
            {
                if (piece.Team == Team.White)
                {
                    var f1 = Position.From("f1");
                    var g1 = Position.From("g1");

                    var d1 = Position.From("d1");
                    var c1 = Position.From("c1");
                    var b1 = Position.From("b1");

                    if (board.CanWhiteCastleKingside &&
                        board.IsEmpty(f1) &&
                        board.IsEmpty(g1) &&
                        !board.IsPositionAttacked(f1, opponentTeam) &&
                        !board.IsPositionAttacked(g1, opponentTeam))
                    {
                        yield return g1;
                    }

                    if (board.CanWhiteCastleQueenside &&
                        board.IsEmpty(d1) &&
                        board.IsEmpty(c1) &&
                        board.IsEmpty(b1) &&
                        !board.IsPositionAttacked(d1, opponentTeam) &&
                        !board.IsPositionAttacked(c1, opponentTeam))
                    {
                        yield return c1;
                    }
                }
                else
                {
                    var f8 = Position.From("f8");
                    var g8 = Position.From("g8");

                    var d8 = Position.From("d8");
                    var c8 = Position.From("c8");
                    var b8 = Position.From("b8");

                    if (board.CanBlackCastleKingside &&
                        board.IsEmpty(f8) &&
                        board.IsEmpty(g8) &&
                        !board.IsPositionAttacked(f8, opponentTeam) &&
                        !board.IsPositionAttacked(g8, opponentTeam))
                    {
                        yield return g8;
                    }

                    if (board.CanBlackCastleQueenside &&
                        board.IsEmpty(d8) &&
                        board.IsEmpty(c8) &&
                        board.IsEmpty(b8) &&
                        !board.IsPositionAttacked(d8, opponentTeam) &&
                        !board.IsPositionAttacked(c8, opponentTeam))
                    {
                        yield return c8;
                    }
                }
            }
        }
    }
}
