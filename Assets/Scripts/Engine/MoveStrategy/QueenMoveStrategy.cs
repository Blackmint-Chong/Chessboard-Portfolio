namespace Chess
{
    public class QueenMoveStrategy : SlidingMoveStrategy
    {
        protected override (int df, int dr)[] Directions =>
            new[] { (0, 1), (1, 1), (1, 0), (1, -1), (0, -1), (-1, -1), (-1, 0), (-1, 1) };
    }
}

