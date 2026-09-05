using System;
using System.Collections.Generic;
using System.Linq;

namespace Chess
{
    public enum MoveResult
    {
        DefaultMove,
        CaptureMove,
        CheckMove,
        IllegalMove
    }

    public class Board
    {
        private Dictionary<Position, Piece> pieces;
        private List<Move> pseudoMoves = new();
        private List<Move> legalMoves = new();
        public Team SideToMove { get; private set; }
        public bool CanWhiteCastleKingside { get; private set; }
        public bool CanWhiteCastleQueenside { get; private set; }
        public bool CanBlackCastleKingside { get; private set; }
        public bool CanBlackCastleQueenside { get; private set; }
        public Position? EnPassantTarget { get; private set; }
        public int HalfmoveClock { get; private set; }
        public int FullmoveNumber { get; private set; }

        private Board()
        {
        }

        private Board(Board source)
        {
            pieces = new Dictionary<Position, Piece>(source.pieces);
            SideToMove = source.SideToMove;
            CanWhiteCastleKingside = source.CanWhiteCastleKingside;
            CanWhiteCastleQueenside = source.CanWhiteCastleQueenside;
            CanBlackCastleKingside = source.CanBlackCastleKingside;
            CanBlackCastleQueenside = source.CanBlackCastleQueenside;
            EnPassantTarget = source.EnPassantTarget;
            HalfmoveClock = source.HalfmoveClock;
            FullmoveNumber = source.FullmoveNumber;
        }

        public static Board FromFEN(string fen)
        {
            var board = new Board();
            board.LoadFromFEN(fen);
            board.GenerateMoves();
            return board;
        }

        private void LoadFromFEN(string fen)
        {
            if (string.IsNullOrWhiteSpace(fen))
                throw new ArgumentException("FEN is null or empty.", nameof(fen));

            var fields = fen.Split(' ');
            if (fields.Length != 6)
                throw new FormatException("FEN must contain 6 space-separated fields.");

            pieces = new Dictionary<Position, Piece>(32);

            var ranks = fields[0].Split('/');
            if (ranks.Length != 8)
                throw new FormatException("Piece placement field must contain 8 ranks.");

            for (int fenRank = 0; fenRank < 8; fenRank++)
            {
                var rankText = ranks[fenRank];
                int file = 0;
                int rank = 7 - fenRank;

                for (int i = 0; i < rankText.Length; i++)
                {
                    char c = rankText[i];
                    if (c >= '1' && c <= '8')
                    {
                        file += c - '0';
                        continue;
                    }

                    Team team = char.IsUpper(c) ? Team.White : Team.Black;
                    PieceType type = char.ToLowerInvariant(c) switch
                    {
                        'p' => PieceType.Pawn,
                        'r' => PieceType.Rook,
                        'n' => PieceType.Knight,
                        'b' => PieceType.Bishop,
                        'q' => PieceType.Queen,
                        'k' => PieceType.King,
                        _ => throw new FormatException($"Invalid piece symbol '{c}'.")
                    };

                    if (file > 7)
                        throw new FormatException("Too many squares in rank.");

                    pieces[new Position(file, rank)] = new Piece(type, team);
                    file++;
                }

                if (file != 8)
                    throw new FormatException("Each rank must have exactly 8 squares.");
            }

            SideToMove = fields[1] switch
            {
                "w" => Team.White,
                "b" => Team.Black,
                _ => throw new FormatException("Active color must be 'w' or 'b'.")
            };

            string castling = fields[2];
            if (castling == "-")
            {
                CanWhiteCastleKingside = false;
                CanWhiteCastleQueenside = false;
                CanBlackCastleKingside = false;
                CanBlackCastleQueenside = false;
            }
            else
            {
                CanWhiteCastleKingside = castling.Contains('K');
                CanWhiteCastleQueenside = castling.Contains('Q');
                CanBlackCastleKingside = castling.Contains('k');
                CanBlackCastleQueenside = castling.Contains('q');
            }

            string ep = fields[3];
            if (ep == "-")
            {
                EnPassantTarget = null;
            }
            else
            {
                if (ep.Length != 2 || ep[0] < 'a' || ep[0] > 'h' || ep[1] < '1' || ep[1] > '8')
                    throw new FormatException("Invalid en passant target square.");

                EnPassantTarget = new Position(ep[0], ep[1] - '0');
            }

            if (!int.TryParse(fields[4], out var halfmove))
                throw new FormatException("Invalid halfmove clock.");
            HalfmoveClock = halfmove;

            if (!int.TryParse(fields[5], out var fullmove))
                throw new FormatException("Invalid fullmove number.");
            FullmoveNumber = fullmove;
        }

        public MoveResult ApplyMove(Move move, out Board board)
        {
            return ApplyMove(move, generateMoves: true, out board);
        }

        private MoveResult ApplyMove(Move move, bool generateMoves, out Board board)
        {
            if (!pieces.TryGetValue(move.From, out var piece))
                throw new KeyNotFoundException($"No piece at {move.From}");

            if (piece.Team != SideToMove)
                throw new InvalidOperationException("Cannot move opponent piece.");

            var movingTeam = SideToMove;
            var next = new Board(this);
            next.EnPassantTarget = null;

            bool isCapture = next.pieces.ContainsKey(move.To);
            bool isEnPassantCapture =
                piece.Type == PieceType.Pawn &&
                EnPassantTarget.HasValue &&
                move.To.Equals(EnPassantTarget.Value) &&
                move.From.File != move.To.File &&
                next.IsEmpty(move.To);

            next.pieces.Remove(move.From);
            next.pieces.Remove(move.To);

            if (isEnPassantCapture)
            {
                int captureRank = piece.Team == Team.White ? move.To.Rank - 1 : move.To.Rank + 1;
                next.pieces.Remove(new Position(move.To.File, captureRank));
                isCapture = true;
            }

            Piece movedPiece = move.Promotion.HasValue
                ? new Piece(move.Promotion.Value, piece.Team)
                : piece;
            next.pieces[move.To] = movedPiece;

            if (piece.Type == PieceType.King)
            {
                if (piece.Team == Team.White)
                {
                    next.CanWhiteCastleKingside = false;
                    next.CanWhiteCastleQueenside = false;
                }
                else
                {
                    next.CanBlackCastleKingside = false;
                    next.CanBlackCastleQueenside = false;
                }

                // Castling: king moves two files.
                if (Math.Abs(move.To.File - move.From.File) == 2)
                {
                    int rookFromFile = move.To.File > move.From.File ? 7 : 0;
                    int rookToFile = move.To.File > move.From.File ? 5 : 3;
                    var rookFrom = new Position(rookFromFile, move.From.Rank);
                    var rookTo = new Position(rookToFile, move.From.Rank);

                    if (next.pieces.TryGetValue(rookFrom, out var rook) &&
                        rook.Type == PieceType.Rook &&
                        rook.Team == piece.Team)
                    {
                        next.pieces.Remove(rookFrom);
                        next.pieces[rookTo] = rook;
                    }
                }
            }
            else if (piece.Type == PieceType.Rook)
            {
                if (piece.Team == Team.White)
                {
                    if (move.From.File == 0 && move.From.Rank == 0)
                        next.CanWhiteCastleQueenside = false;
                    else if (move.From.File == 7 && move.From.Rank == 0)
                        next.CanWhiteCastleKingside = false;
                }
                else
                {
                    if (move.From.File == 0 && move.From.Rank == 7)
                        next.CanBlackCastleQueenside = false;
                    else if (move.From.File == 7 && move.From.Rank == 7)
                        next.CanBlackCastleKingside = false;
                }
            }

            // Capturing rook on its initial square removes that side's castling right.
            if (isCapture)
            {
                if (move.To.File == 0 && move.To.Rank == 0)
                    next.CanWhiteCastleQueenside = false;
                else if (move.To.File == 7 && move.To.Rank == 0)
                    next.CanWhiteCastleKingside = false;
                else if (move.To.File == 0 && move.To.Rank == 7)
                    next.CanBlackCastleQueenside = false;
                else if (move.To.File == 7 && move.To.Rank == 7)
                    next.CanBlackCastleKingside = false;
            }

            if (piece.Type == PieceType.Pawn && Math.Abs(move.To.Rank - move.From.Rank) == 2)
            {
                int passedRank = (move.From.Rank + move.To.Rank) / 2;
                next.EnPassantTarget = new Position(move.From.File, passedRank);
            }

            next.HalfmoveClock = (piece.Type == PieceType.Pawn || isCapture) ? 0 : HalfmoveClock + 1;
            next.FullmoveNumber = SideToMove == Team.Black ? FullmoveNumber + 1 : FullmoveNumber;
            next.SideToMove = SideToMove == Team.White ? Team.Black : Team.White;

            if (next.IsInCheck(movingTeam))
            {
                board = this;
                return MoveResult.IllegalMove;
            }

            if (generateMoves)
                next.GenerateMoves();

            board = next;

            bool isCheck = next.IsInCheck(next.SideToMove);
            if (isCheck) return MoveResult.CheckMove;
            else if (isCapture) return MoveResult.CaptureMove;
            else return MoveResult.DefaultMove;
        }

        public bool TryGetPiece(Position position, out Piece piece)
            => pieces.TryGetValue(position, out piece);

        public bool IsEmpty(Position position)
            => !pieces.ContainsKey(position);

        public IEnumerable<Move> GetPseudoMoves()
            => pseudoMoves;

        public IEnumerable<Move> GetLegalMoves()
            => legalMoves;

        private IEnumerable<Move> GeneratePseudoMoves()
        {
            foreach (var (from, piece) in pieces)
            {
                if (piece.Team != SideToMove)
                    continue;

                var strategy = MoveStrategyRegistry.Get(piece.Type);

                foreach (var to in strategy.GetPseudoMoves(from, this))
                {
                    bool isPromotion =
                        piece.Type == PieceType.Pawn &&
                        ((piece.Team == Team.White && to.Rank == 7) ||
                         (piece.Team == Team.Black && to.Rank == 0));

                    if (!isPromotion)
                    {
                        yield return new Move(from, to);
                        continue;
                    }

                    yield return new Move(from, to, PieceType.Queen);
                    yield return new Move(from, to, PieceType.Rook);
                    yield return new Move(from, to, PieceType.Bishop);
                    yield return new Move(from, to, PieceType.Knight);
                }
            }
        }

        private IEnumerable<Move> GenerateLegalMoves()
        {
            var movingTeam = SideToMove;

            foreach (var move in pseudoMoves)
            {
                if (ApplyMove(move, generateMoves: false, out var _) != MoveResult.IllegalMove)
                    yield return move;
            }
        }

        private void GenerateMoves()
        {
            pseudoMoves = GeneratePseudoMoves().ToList();
            legalMoves = GenerateLegalMoves().ToList();
        }

        public bool IsPositionAttacked(Position position, Team byTeam)
        {
            foreach (var (pos, piece) in pieces)
            {
                if (piece.Team != byTeam)
                    continue;

                var strategy = MoveStrategyRegistry.Get(piece.Type);

                foreach (var attack in strategy.GetAttackSquares(pos, this))
                {
                    if (attack.Equals(position))
                        return true;
                }
            }
            return false;
        }

        public bool IsInCheck(Team team)
        {
            var kingPos = FindKing(team);
            var opponent = team == Team.White ? Team.Black : Team.White;

            return IsPositionAttacked(kingPos, opponent);
        }

        private Position FindKing(Team team)
        {
            foreach (var (pos, piece) in pieces)
            {
                if (piece.Team == team && piece.Type == PieceType.King)
                    return pos;
            }

            throw new InvalidOperationException($"King not found for team {team}");
        }

        public Position GetKingPosition(Team team)
        {
            return FindKing(team);
        }

        private bool HasAnyLegalMove()
            => legalMoves.Count > 0;

        public bool IsLegalMove(Move move)
        {
            return legalMoves.Contains(move);
        }

        public bool IsCheckMate()
        {
            return IsInCheck(SideToMove) && !HasAnyLegalMove();
        }

        public bool IsStaleMate()
        {
            return !IsInCheck(SideToMove) && !HasAnyLegalMove();
        }

        public string GetRepetitionKey()
        {
            return string.Join(" ", GetPiecePlacementField(), GetActiveColorField(), GetCastlingField(), GetEnPassantField());
        }

        public string ToFEN()
        {
            return string.Join(
                " ",
                GetPiecePlacementField(),
                GetActiveColorField(),
                GetCastlingField(),
                GetEnPassantField(),
                HalfmoveClock,
                FullmoveNumber);
        }

        public bool IsInsufficientMaterialDraw()
        {
            var nonKingPieces = pieces
                .Where(entry => entry.Value.Type != PieceType.King)
                .Select(entry => (entry.Key, entry.Value))
                .ToList();

            if (nonKingPieces.Count == 0)
                return true;

            if (nonKingPieces.Any(entry =>
                entry.Value.Type != PieceType.Bishop &&
                entry.Value.Type != PieceType.Knight))
            {
                return false;
            }

            if (nonKingPieces.All(entry => entry.Value.Type == PieceType.Bishop))
                return AreAllBishopsOnSameColor(nonKingPieces);

            return nonKingPieces.Count == 1 && nonKingPieces[0].Value.Type == PieceType.Knight;
        }

        private static bool AreAllBishopsOnSameColor(IEnumerable<(Position Position, Piece Piece)> bishopEntries)
        {
            bool? squareColor = null;

            foreach (var (position, _) in bishopEntries)
            {
                bool isLightSquare = (position.File + position.Rank) % 2 == 0;
                if (!squareColor.HasValue)
                {
                    squareColor = isLightSquare;
                    continue;
                }

                if (squareColor.Value != isLightSquare)
                    return false;
            }

            return true;
        }

        private string GetPiecePlacementField()
        {
            var ranks = new List<string>(8);

            for (int rank = 7; rank >= 0; rank--)
            {
                int emptyCount = 0;
                var rankText = new System.Text.StringBuilder(8);

                for (int file = 0; file < 8; file++)
                {
                    var position = new Position(file, rank);
                    if (!pieces.TryGetValue(position, out var piece))
                    {
                        emptyCount++;
                        continue;
                    }

                    if (emptyCount > 0)
                    {
                        rankText.Append(emptyCount);
                        emptyCount = 0;
                    }

                    rankText.Append(GetPieceSymbol(piece));
                }

                if (emptyCount > 0)
                    rankText.Append(emptyCount);

                ranks.Add(rankText.ToString());
            }

            return string.Join("/", ranks);
        }

        private string GetActiveColorField()
            => SideToMove == Team.White ? "w" : "b";

        private string GetCastlingField()
        {
            var castling = new System.Text.StringBuilder(4);

            if (CanWhiteCastleKingside)
                castling.Append('K');
            if (CanWhiteCastleQueenside)
                castling.Append('Q');
            if (CanBlackCastleKingside)
                castling.Append('k');
            if (CanBlackCastleQueenside)
                castling.Append('q');

            return castling.Length > 0 ? castling.ToString() : "-";
        }

        private string GetEnPassantField()
        {
            if (!EnPassantTarget.HasValue)
                return "-";

            var position = EnPassantTarget.Value;
            return $"{(char)('a' + position.File)}{position.Rank + 1}";
        }

        private static char GetPieceSymbol(Piece piece)
        {
            char symbol = piece.Type switch
            {
                PieceType.Pawn => 'p',
                PieceType.Rook => 'r',
                PieceType.Knight => 'n',
                PieceType.Bishop => 'b',
                PieceType.Queen => 'q',
                PieceType.King => 'k',
                _ => throw new ArgumentOutOfRangeException(nameof(piece.Type))
            };

            return piece.Team == Team.White ? char.ToUpperInvariant(symbol) : symbol;
        }

        public bool IsFiftyMoveRule() => HalfmoveClock >= 100;
    }
}
