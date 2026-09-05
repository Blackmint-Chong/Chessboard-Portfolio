using System;
using System.Collections.Generic;

namespace Chess
{
    public abstract class SlidingMoveStrategy : IMoveStrategy
    {
        protected abstract (int df, int dr)[] Directions { get; }

        private IEnumerable<Position> GetMovingAreas(Position position, Board board, bool containsAllyPiece)
        {
            if (!board.TryGetPiece(position, out var piece))
                throw new InvalidOperationException($"No piece at {position})");

            foreach (var (df, dr) in Directions)
            {
                var current = position;

                while (true)
                {
                    if (!current.TryOffset(df, dr, out var next))
                        break;

                    current = next;

                    if (!board.TryGetPiece(current, out var targetPiece)) // blank
                    {
                        yield return current;
                    }
                    else
                    {
                        if (targetPiece.Team != piece.Team) // capture
                        {
                            yield return current;
                        }
                        else // ally piece
                        {
                            if (containsAllyPiece)
                            {
                                yield return current;
                            }
                        }

                        break;
                    }
                }
            }
        }

        public IEnumerable<Position> GetPseudoMoves(Position position, Board board)
            => GetMovingAreas(position, board, false);

        public IEnumerable<Position> GetAttackSquares(Position position, Board board)
            => GetMovingAreas(position, board, true);
    }

}
