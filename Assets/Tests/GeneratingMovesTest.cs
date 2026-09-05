using NUnit.Framework;
using Chess;
using System.Security;

public class GeneratingMovesTest
{
    [Test]
    public void GeneratePseudoMovesTest()
    {
        // initial board
        var board1 = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        var actual1 = board1.GetPseudoMoves();
        var expected1 = MoveAssert.MovesFromUci(
            "a2a3", "a2a4",
            "b2b3", "b2b4",
            "c2c3", "c2c4",
            "d2d3", "d2d4",
            "e2e3", "e2e4",
            "f2f3", "f2f4",
            "g2g3", "g2g4",
            "h2h3", "h2h4",
            "b1a3", "b1c3",
            "g1f3", "g1h3"
        );
        MoveAssert.AreEquivalent(actual1, expected1);

        // pawn capture
        var board2 = Board.FromFEN("rnbqkbnr/ppp1pppp/8/3p4/4P3/8/PPPP1PPP/RNBQKBNR w KQkq d6 0 2");
        var actual2 = board2.GetPseudoMoves();
        var expected2 = MoveAssert.MovesFromUci(
            "a2a3", "a2a4",
            "b2b3", "b2b4",
            "c2c3", "c2c4",
            "d2d3", "d2d4",
            "e4d5", "e4e5",
            "f2f3", "f2f4",
            "g2g3", "g2g4",
            "h2h3", "h2h4",
            "b1a3", "b1c3",
            "g1f3", "g1h3", "g1e2",
            "d1e2", "d1f3", "d1g4", "d1h5",
            "f1e2", "f1d3", "f1c4", "f1b5", "f1a6",
            "e1e2"
        );
        MoveAssert.AreEquivalent(actual2, expected2);

        // Castling
        var board3 = Board.FromFEN("r1bqkb1r/pppp1ppp/2n2n2/4p3/2B1P3/5N2/PPPP1PPP/RNBQK2R w KQkq - 4 4");
        var actual3 = board3.GetPseudoMoves();
        var expected3 = MoveAssert.MovesFromUci(
            "c4b5", "c4a6", "c4d5", "c4e6", "c4f7", "c4b3", "c4d3", "c4e2", "c4f1",
            "f3d4", "f3e5", "f3g5", "f3h4", "f3g1",
            "a2a3", "a2a4",
            "b2b3", "b2b4",
            "c2c3",
            "d2d3", "d2d4",
            "g2g3", "g2g4",
            "h2h3", "h2h4",
            "b1a3", "b1c3",
            "d1e2",
            "e1e2", "e1f1", "e1g1",
            "h1g1", "h1f1"
        );
        MoveAssert.AreEquivalent(actual3, expected3);

        // En Passant
        var board4 = Board.FromFEN("r1bqkb1r/ppp2ppp/8/3pP3/P2Q4/8/1PP2PPP/RNB2RK1 w kq d6 0 10");
        var actual4 = board4.GetPseudoMoves();
        var expected4 = MoveAssert.MovesFromUci(
            "e5d6", "e5e6",
            "a4a5",
            "d4c5", "d4b6", "d4a7", "d4d5", "d4e4", "d4f4", "d4g4", "d4h4", "d4e3", "d4d3", "d4d2", "d4d1", "d4c3", "d4c4", "d4b4",
            "b2b3", "b2b4",
            "c2c3", "c2c4",
            "f2f3", "f2f4",
            "g2g3", "g2g4",
            "h2h3", "h2h4",
            "a1a2", "a1a3",
            "b1a3", "b1c3", "b1d2",
            "c1d2", "c1e3", "c1f4", "c1g5", "c1h6",
            "f1e1", "f1d1",
            "g1h1"
        );
        MoveAssert.AreEquivalent(actual4, expected4);

        //Promotion
        var board5 = Board.FromFEN("r1b2b1r/pppPkppp/8/8/P2Q3q/8/1PP2PPP/RNB2RK1 w - - 1 12");
        var actual5 = board5.GetPseudoMoves();
        var expected5 = MoveAssert.MovesFromUci(
            "d7c8q", "d7c8r", "d7c8b", "d7c8n", "d7d8q", "d7d8r", "d7d8b", "d7d8n",
            "a4a5",
            "d4c5", "d4b6", "d4a7", "d4d5", "d4d6", "d4e5", "d4f6", "d4g7", "d4e4", "d4f4", "d4g4", "d4h4", "d4e3", "d4d3", "d4d2", "d4d1", "d4c3", "d4c4", "d4b4",
            "b2b3", "b2b4",
            "c2c3", "c2c4",
            "f2f3", "f2f4",
            "g2g3", "g2g4",
            "h2h3",
            "a1a2", "a1a3",
            "b1a3", "b1c3", "b1d2",
            "c1d2", "c1e3", "c1f4", "c1g5", "c1h6",
            "f1e1", "f1d1",
            "g1h1"
        );
        MoveAssert.AreEquivalent(actual5, expected5);
    }

    [Test]
    public void GenerateLegalMovesTest()
    {
        // initial board: legal == pseudo
        var board1 = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
        var actual1 = board1.GetLegalMoves();
        var expected1 = MoveAssert.MovesFromUci(
            "a2a3", "a2a4",
            "b2b3", "b2b4",
            "c2c3", "c2c4",
            "d2d3", "d2d4",
            "e2e3", "e2e4",
            "f2f3", "f2f4",
            "g2g3", "g2g4",
            "h2h3", "h2h4",
            "b1a3", "b1c3",
            "g1f3", "g1h3"
        );
        MoveAssert.AreEquivalent(actual1, expected1);

        // king is in check
        var board2 = Board.FromFEN("r1bqk2r/ppp2ppp/2n5/4p3/2B5/1QNnP3/PP1P1PPP/R1B1K2R w KQkq - 7 11");
        var actual2 = board2.GetLegalMoves();
        var expected2 = MoveAssert.MovesFromUci(
            "c4d3",
            "e1d1", "e1e2", "e1f1"
        );
        MoveAssert.AreEquivalent(actual2, expected2);

        // Checkmate
        var board3 = Board.FromFEN("1k6/8/8/N6p/5p2/1P3P2/7r/1R1K1r2 w - - 0 42");
        var actual3 = board3.GetLegalMoves();
        var expected3 = MoveAssert.MovesFromUci();
        MoveAssert.AreEquivalent(actual3, expected3);
    }
}
