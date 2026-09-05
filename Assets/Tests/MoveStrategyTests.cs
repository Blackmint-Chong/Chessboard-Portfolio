using System.Linq;
using System.Net.WebSockets;
using Chess;
using NUnit.Framework;

public class MoveStrategyTests
{
    [Test]
    public void KnightMoveStrategyTest()
    {
        var board = Board.FromFEN("r2qk2r/p4ppp/2p1bnn1/3pp3/Pp2P3/1B1PPN2/1PPNQ1PP/2KR3R b kq - 5 14");
        var strategy = new KnightMoveStrategy();

        var moves1 = strategy.GetPseudoMoves(new Position('g', 6), board).ToList();

        var expected1 = new[]
        {
            "f8", "e7", "f4", "h4"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected1, moves1);

        var moves2 = strategy.GetPseudoMoves(Position.From("f3"), board);

        var expected2 = new[]
        {
            "e5", "g5", "h4", "g1", "e1", "d4"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected2, moves2);
    }

    [Test]
    public void BishopMoveStrategyTest()
    {
        var board = Board.FromFEN("rq3rk1/4bppp/p4n2/1p2p1B1/3nP3/2N3Q1/PPB2PPP/3RR1K1 w - - 4 18");
        var strategy = new BishopMoveStrategy();

        var moves1 = strategy.GetPseudoMoves(Position.From("c2"), board);
        var expected1 = new[]
        {
            "b3", "a4",
            "b1",
            "d3"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected1, moves1);

        var moves2 = strategy.GetPseudoMoves(Position.From("g5"), board);
        var expected2 = new[]
        {
            "f6",
            "h6",
            "h4",
            "f4", "e3", "d2", "c1"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected2, moves2);
    }

    [Test]
    public void QueenMoveStrategyTest()
    {
        var board = Board.FromFEN("rq3rk1/4bppp/p4n2/1p2p1B1/3nP3/2N3Q1/PPB2PPP/3RR1K1 w - - 4 18");
        var strategy = new QueenMoveStrategy();

        var moves = strategy.GetPseudoMoves(Position.From("g3"), board);
        var expected = new[]
        {
            "f4", "e5",
            "g4",
            "h4",
            "h3",
            "f3", "e3", "d3"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected, moves);
    }

    [Test]
    public void RookMoveStrategyTest()
    {
        var board = Board.FromFEN("rq3rk1/4bppp/p4n2/1p2p1B1/3nP3/2N3Q1/PPB2PPP/3RR1K1 w - - 4 18");
        var strategy = new RookMoveStrategy();

        var moves1 = strategy.GetPseudoMoves(Position.From("d1"), board);
        var expected1 = new[]
        {
            "c1", "b1", "a1",
            "d2", "d3", "d4"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected1, moves1);

        var moves2 = strategy.GetPseudoMoves(Position.From("a8"), board);
        var expected2 = new[]
        {
            "a7"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected2, moves2);
    }

    [Test]
    public void PawnMoveStrategyTest()
    {
        var board1 = Board.FromFEN("rq3rk1/4bppp/p4n2/1p2p1B1/3nP3/2N3Q1/PPB2PPP/3RR1K1 w - - 4 18");
        var strategy = new PawnMoveStrategy();

        var moves1 = strategy.GetPseudoMoves(Position.From("b5"), board1);
        var expected1 = new[]
        {
            "b4"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected1, moves1);

        var moves2 = strategy.GetPseudoMoves(Position.From("b2"), board1);
        var expected2 = new[]
        {
            "b3", "b4"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected2, moves2);

        // piece taking
        var board2 = Board.FromFEN("r1bqkb1r/pppp1ppp/2n5/1n2P3/P7/5N2/1PP2PPP/RNBQ1RK1 b kq a3 0 7");

        var moves3 = strategy.GetPseudoMoves(Position.From("a4"), board2);
        var expected3 = new[]
        {
            "a5", "b5"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected3, moves3);

        // en passant
        var board3 = Board.FromFEN("r1bqkb1r/ppp2ppp/8/3pP3/P2Q4/8/1PP2PPP/RNB2RK1 w kq d6 0 10");

        var moves4 = strategy.GetPseudoMoves(Position.From("e5"), board3);
        var expected4 = new[]
        {
            "d6", "e6"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected4, moves4);

        // AttackSquares Test
        var moves5 = strategy.GetAttackSquares(Position.From("a4"), board2);
        var expected5 = new[]
        {
            "b5"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(expected5, moves5);
    }

    [Test]
    public void KingMoveStrategyTest()
    {
        var strategy = new KingMoveStrategy();

        var board1 = Board.FromFEN("rnbqk2r/ppp1bppp/4pn2/3p4/2PP4/6P1/PP2PPBP/RNBQK1NR w KQkq - 2 5");
        var Wmove1 = strategy.GetPseudoMoves(Position.From("e1"), board1);
        var Wexpected1 = new[]
        {
            "d2", "f1"
        }
        .Select(Position.From)
        .ToList();
        var Bmove1 = strategy.GetPseudoMoves(Position.From("e8"), board1);
        var Bexpected1 = new[]
        {
            "d7", "f8",
            "g8"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(Wexpected1, Wmove1);
        CollectionAssert.AreEquivalent(Bexpected1, Bmove1);

        var board2 = Board.FromFEN("rnbq1rk1/ppp1bppp/4pn2/3p4/2PP4/5NP1/PP2PPBP/RNBQ1RK1 b - - 5 6");
        var Wmove2 = strategy.GetPseudoMoves(Position.From("g1"), board2);
        var Wexpected2 = new[]
        {
            "h1"
        }
        .Select(Position.From)
        .ToList();
        var Bmove2 = strategy.GetPseudoMoves(Position.From("g8"), board2);
        var Bexpected2 = new[]
        {
            "h8"
        }
        .Select(Position.From)
        .ToList();

        CollectionAssert.AreEquivalent(Wexpected2, Wmove2);
        CollectionAssert.AreEquivalent(Bexpected2, Bmove2);
    }

    [Test]
    public void BlackQueensideCastling_UsesBlackCastlingRight()
    {
        var board = Board.FromFEN("r3k3/8/8/8/8/8/8/4K3 b q - 0 1");
        var castlingMove = new Move(Position.From("e8"), Position.From("c8"));

        Assert.IsTrue(board.GetLegalMoves().Contains(castlingMove));
    }

    [Test]
    public void QueensideCastling_AllowsAttackedBFile()
    {
        var whiteBoard = Board.FromFEN("1r2k3/8/8/8/8/8/8/R3K3 w Q - 0 1");
        var blackBoard = Board.FromFEN("r3k3/8/8/8/8/8/8/1R2K3 b q - 0 1");

        Assert.IsTrue(whiteBoard.GetLegalMoves().Contains(
            new Move(Position.From("e1"), Position.From("c1"))));
        Assert.IsTrue(blackBoard.GetLegalMoves().Contains(
            new Move(Position.From("e8"), Position.From("c8"))));
    }
}
