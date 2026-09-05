namespace Chess
{
    public class RookMoveStrategy : SlidingMoveStrategy
    {
        protected override (int df, int dr)[] Directions =>
            new[] { (1, 0), (-1, 0), (0, 1), (0, -1) };
    }
}

