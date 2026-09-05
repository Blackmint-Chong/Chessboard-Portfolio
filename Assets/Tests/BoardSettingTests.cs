using NUnit.Framework;
using Chess;

public class BoardSettingTests
{
    // A Test behaves as an ordinary method
    [Test]
    public void SideToMoveTests()
    {
        Board board1 = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        Assert.AreEqual(board1.SideToMove, Team.White);

        Board board2 = Board.FromFEN("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq - 0 1");
        Assert.AreEqual(board2.SideToMove, Team.Black);

        Board board3 = Board.FromFEN("rnbqkbnr/ppppp2p/8/5ppQ/2B1P3/8/PPPP1PPP/RNB1K1NR b KQkq - 0 1");
        Assert.AreEqual(board3.SideToMove, Team.Black);
    }

    [Test]
    public void CastleTests()
    {
        Board board1 = Board.FromFEN("r1bqkbnr/pppp1ppp/2n5/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R b KQkq - 3 3"); // base case
        Assert.IsTrue(board1.CanBlackCastleKingside);
        Assert.IsTrue(board1.CanBlackCastleQueenside);
        Assert.IsTrue(board1.CanWhiteCastleKingside);
        Assert.IsTrue(board1.CanWhiteCastleQueenside);

        Board board2 = Board.FromFEN("r1bqk2r/1ppp1ppp/p1n2n2/2b1p3/P1B1P3/R2P1N2/1PP2PPP/1NBQK2R b Kkq - 1 6"); // Ra3 : Bhite can't castle queenside
        Assert.IsTrue(board2.CanBlackCastleKingside);
        Assert.IsTrue(board2.CanBlackCastleQueenside);
        Assert.IsTrue(board2.CanWhiteCastleKingside);
        Assert.IsFalse(board2.CanWhiteCastleQueenside);

        Board board3 = Board.FromFEN("r1bqk2r/pppp1ppp/2n2n2/2b1p3/2B1P3/3P1N2/PPP2PPP/RNBQ1RK1 b kq - 2 5"); // White already castled
        Assert.IsTrue(board3.CanBlackCastleKingside);
        Assert.IsTrue(board3.CanBlackCastleQueenside);
        Assert.IsFalse(board3.CanWhiteCastleKingside);
        Assert.IsFalse(board3.CanWhiteCastleQueenside);

        Board board4 = Board.FromFEN("r1bq3r/ppppkppp/2n2n2/2b1p3/2B1P3/3P1N2/PPP2PPP/RNBQ1RK1 w - - 3 6"); // ...Ke7 : Black can't castle
        Assert.IsFalse(board4.CanBlackCastleKingside);
        Assert.IsFalse(board4.CanBlackCastleQueenside);
        Assert.IsFalse(board4.CanWhiteCastleKingside);
        Assert.IsFalse(board4.CanWhiteCastleQueenside);

        Board board5 = Board.FromFEN("rn1qkb1r/p1pp1ppp/bp3n2/4p3/4P3/5NPB/PPPP1P1P/RNBQK2R w KQkq - 2 5"); // black a6 bishop is attacking f1, but white has castling right
        Assert.IsTrue(board5.CanBlackCastleKingside);
        Assert.IsTrue(board5.CanBlackCastleQueenside);
        Assert.IsTrue(board5.CanWhiteCastleKingside);
        Assert.IsTrue(board5.CanWhiteCastleQueenside);
    }

    [Test]
    public void EnPassantTest()
    {
        Board board1 = Board.FromFEN("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
        Position position1 = new('e', 3);
        Assert.AreEqual(board1.EnPassantTarget, position1);

        Board board2 = Board.FromFEN("rnbqkbnr/pppp1ppp/4p3/8/4P3/8/PPPP1PPP/RNBQKBNR w KQkq - 0 2");
        Assert.IsNull(board2.EnPassantTarget);
    }

    [Test]
    public void ClockTest()
    {
        Board board1 = Board.FromFEN("r1bqkb1r/p4ppp/2p2n2/nB2p1N1/8/8/PPPP1PPP/RNBQK2R w KQkq - 0 8");
        Assert.AreEqual(board1.HalfmoveClock, 0);
        Assert.AreEqual(board1.FullmoveNumber, 8);

        Board board2 = Board.FromFEN("r6r/p5k1/2p4p/8/8/8/4q3/7K w - - 14 42");
        Assert.AreEqual(board2.HalfmoveClock, 14);
        Assert.AreEqual(board2.FullmoveNumber, 42);
    }

    [Test]
    public void PiecePositionTest()
    {
        Board board = Board.FromFEN("r2qk2r/p4ppp/2p1bnn1/3pp3/Pp2P3/1B1PPN2/1PPNQ1PP/2KR3R b kq - 5 14");

        Assert.IsTrue(board.TryGetPiece(new Position('e', 2), out var e2Queen));
        Assert.AreEqual(e2Queen.Type, PieceType.Queen);
        Assert.AreEqual(e2Queen.Team, Team.White);

        Assert.IsTrue(board.TryGetPiece(new Position('f', 6), out var f6Knight));
        Assert.AreEqual(f6Knight.Type, PieceType.Knight);
        Assert.AreEqual(f6Knight.Team, Team.Black);

        Assert.IsFalse(board.TryGetPiece(new Position('d', 4), out var blank));
    }

    [Test]
    public void ToFENTest()
    {
        const string fen = "r2qk2r/p4ppp/2p1bnn1/3pp3/Pp2P3/1B1PPN2/1PPNQ1PP/2KR3R b kq - 5 14";
        Board board = Board.FromFEN(fen);

        Assert.AreEqual(fen, board.ToFEN());

        Board nextBoard = Board.FromFEN("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1");
        Assert.AreEqual("rnbqkbnr/pppppppp/8/8/4P3/8/PPPP1PPP/RNBQKBNR b KQkq e3 0 1", nextBoard.ToFEN());
    }
}
