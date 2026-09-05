using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using NUnit.Framework;

public static class MoveAssert
{
    public static void AreEquivalent(IEnumerable<Move> actual, IEnumerable<Move> expected)
    {
        var actualUci = actual.Select(ToUci).ToList();
        var expectedUci = expected.Select(ToUci).ToList();
        CollectionAssert.AreEquivalent(expectedUci, actualUci);
    }

    public static List<Move> MovesFromUci(params string[] uciMoves)
        => uciMoves.Select(MoveFromUci).ToList();

    public static Move MoveFromUci(string uci)
    {
        if (string.IsNullOrWhiteSpace(uci) || (uci.Length != 4 && uci.Length != 5))
            throw new ArgumentException($"Invalid UCI move: {uci}", nameof(uci));

        var from = Position.From(uci.Substring(0, 2));
        var to = Position.From(uci.Substring(2, 2));

        if (uci.Length == 4)
            return new Move(from, to);

        return new Move(from, to, ParsePromotion(uci[4]));
    }

    private static PieceType ParsePromotion(char c)
        => char.ToLowerInvariant(c) switch
        {
            'q' => PieceType.Queen,
            'r' => PieceType.Rook,
            'b' => PieceType.Bishop,
            'n' => PieceType.Knight,
            _ => throw new ArgumentException($"Invalid promotion piece: {c}")
        };

    private static string ToUci(Move move)
    {
        char fromFile = (char)('a' + move.From.File);
        char fromRank = (char)('1' + move.From.Rank);
        char toFile = (char)('a' + move.To.File);
        char toRank = (char)('1' + move.To.Rank);

        if (!move.Promotion.HasValue)
            return $"{fromFile}{fromRank}{toFile}{toRank}";

        char promo = move.Promotion.Value switch
        {
            PieceType.Queen => 'q',
            PieceType.Rook => 'r',
            PieceType.Bishop => 'b',
            PieceType.Knight => 'n',
            _ => throw new ArgumentException($"Invalid promotion type: {move.Promotion.Value}")
        };

        return $"{fromFile}{fromRank}{toFile}{toRank}{promo}";
    }
}
