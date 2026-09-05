using System.Collections.Generic;

namespace Chess
{
    public interface IMoveStrategy
    {
        IEnumerable<Position> GetPseudoMoves(Position position, Board board);
        IEnumerable<Position> GetAttackSquares(Position position, Board board);
    }
}

