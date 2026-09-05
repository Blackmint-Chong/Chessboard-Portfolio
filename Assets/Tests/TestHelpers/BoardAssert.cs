using Chess;
using NUnit.Framework;

public static class BoardAssert
{
    public static void AreEqual(Board actual, Board expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(expected, Is.Not.Null);

        AssertMetadata(actual, expected);
        AssertPieces(actual, expected);
    }

    private static void AssertMetadata(Board actual, Board expected)
    {
        Assert.AreEqual(actual.SideToMove, expected.SideToMove);
        Assert.AreEqual(actual.CanBlackCastleKingside, expected.CanBlackCastleKingside);
        Assert.AreEqual(actual.CanBlackCastleQueenside, expected.CanBlackCastleQueenside);
        Assert.AreEqual(actual.CanWhiteCastleKingside, expected.CanWhiteCastleKingside);
        Assert.AreEqual(actual.CanWhiteCastleQueenside, expected.CanWhiteCastleQueenside);
        Assert.AreEqual(actual.EnPassantTarget, expected.EnPassantTarget);
        Assert.AreEqual(actual.HalfmoveClock, expected.HalfmoveClock);
        Assert.AreEqual(actual.FullmoveNumber, expected.FullmoveNumber);
    }

    private static void AssertPieces(Board actual, Board expected)
    {
        for (int file = 0; file < 8; file++)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                var position = new Position(file, rank);
                bool actualHasPiece = actual.TryGetPiece(position, out var actualPiece);
                bool expectedHasPiece = expected.TryGetPiece(position, out var expectedPiece);
                Assert.AreEqual(actualHasPiece, expectedHasPiece);
                if (actualHasPiece && expectedHasPiece)
                {
                    Assert.AreEqual(actualPiece.Type, expectedPiece.Type);
                    Assert.AreEqual(actualPiece.Team, expectedPiece.Team);
                }
            }
        }
    }
}
