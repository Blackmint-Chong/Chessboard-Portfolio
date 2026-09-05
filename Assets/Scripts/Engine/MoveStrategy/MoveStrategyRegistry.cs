using System;
using System.Collections.Generic;

namespace Chess
{
    public static class MoveStrategyRegistry
    {
        private static readonly Dictionary<PieceType, IMoveStrategy> Strategies =
            new()
            {
                {PieceType.Knight, new KnightMoveStrategy()},
                {PieceType.Bishop, new BishopMoveStrategy()},
                {PieceType.Rook, new RookMoveStrategy()},
                {PieceType.Queen, new QueenMoveStrategy()},
                {PieceType.King, new KingMoveStrategy()},
                {PieceType.Pawn, new PawnMoveStrategy()}
            };

        public static IMoveStrategy Get(PieceType type)
        {
            if (!Strategies.TryGetValue(type, out var strategy))
                throw new InvalidOperationException($"Unknown piece type: {type}");

            return strategy;
        }
    }
}
