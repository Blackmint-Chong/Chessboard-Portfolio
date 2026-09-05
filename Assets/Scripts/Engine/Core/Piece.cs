namespace Chess
{
    public class Piece
    {
        public PieceType Type { get; }
        public Team Team { get; }

        public Piece(PieceType type, Team team)
        {
            Type = type;
            Team = team;
        }
    }
}

