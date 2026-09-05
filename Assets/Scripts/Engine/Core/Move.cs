namespace Chess
{
    public readonly struct Move
    {
        public Position From { get; }
        public Position To { get; }
        public PieceType? Promotion { get; }
        public Move(Position from, Position to)
        {
            From = from;
            To = to;
            Promotion = null;
        }
        public Move(Position from, Position to, PieceType promotion)
        {
            From = from;
            To = to;
            Promotion = promotion;
        }
    }
}
