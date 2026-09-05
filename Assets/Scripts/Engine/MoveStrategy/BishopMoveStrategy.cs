namespace Chess
{
    public class BishopMoveStrategy : SlidingMoveStrategy
    {
        protected override (int df, int dr)[] Directions =>
            new[] { (1, 1), (1, -1), (-1, 1), (-1, -1) };
    }
}

