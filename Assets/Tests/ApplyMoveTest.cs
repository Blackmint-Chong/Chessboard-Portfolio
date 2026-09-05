using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Chess;

public class ApplyMoveTest
{
    [Test]
    public void OrdinaryApplyMoveTest()
    {
        Board board = Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");

        //1. d4
        Move move1w = new(Position.From("d2"), Position.From("d4"));
        board.ApplyMove(move1w, out var board1);
        Board expected1 = Board.FromFEN("rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1");
        BoardAssert.AreEqual(board1, expected1);

        //1. ...d5 2. c4 ... e6
        Move move1b = new(Position.From("d7"), Position.From("d5"));
        Move move2w = new(Position.From("c2"), Position.From("c4"));
        Move move2b = new(Position.From("e7"), Position.From("e6"));
        board1.ApplyMove(move1b, out var board1AfterMove1b);
        board1AfterMove1b.ApplyMove(move2w, out var board1AfterMove2w);
        board1AfterMove2w.ApplyMove(move2b, out var board2);
        Board expected2 = Board.FromFEN("rnbqkbnr/ppp2ppp/4p3/3p4/2PP4/8/PP2PPPP/RNBQKBNR w KQkq - 0 3");
        BoardAssert.AreEqual(board2, expected2);
    }

    [Test]
    public void CapturingApplyMoveTest()
    {
        Board board = Board.FromFEN("r1bqkb1r/pp1n1ppp/2p1pn2/3p4/2PP4/2NBPN2/PP3PPP/R1BQK2R b KQkq - 2 6");

        // 6. ...dxc4 7. Bxc4 ... b5 8. Bd3
        Move move6b = new(Position.From("d5"), Position.From("c4"));
        Move move7w = new(Position.From("d3"), Position.From("c4"));
        Move move7b = new(Position.From("b7"), Position.From("b5"));
        Move move8w = new(Position.From("c4"), Position.From("d3"));
        board.ApplyMove(move6b, out var boardAfterMove6b);
        boardAfterMove6b.ApplyMove(move7w, out var boardAfterMove7w);
        boardAfterMove7w.ApplyMove(move7b, out var boardAfterMove7b);
        boardAfterMove7b.ApplyMove(move8w, out var board1);
        Board expected1 = Board.FromFEN("r1bqkb1r/p2n1ppp/2p1pn2/1p6/3P4/2NBPN2/PP3PPP/R1BQK2R b KQkq - 1 8");
        BoardAssert.AreEqual(board1, expected1);
    }

    [Test]
    public void CastlingApplyMoveTest()
    {
        Board board1 = Board.FromFEN("r2qkb1r/pb1n1ppp/2p1pn2/1p6/3P4/2NBPN2/PP3PPP/R1BQK2R w KQkq - 2 9");

        // 9. O-O
        Move move9w = new(Position.From("e1"), Position.From("g1"));
        board1.ApplyMove(move9w, out var board2);
        Board expected1 = Board.FromFEN("r2qkb1r/pb1n1ppp/2p1pn2/1p6/3P4/2NBPN2/PP3PPP/R1BQ1RK1 b kq - 3 9");

        BoardAssert.AreEqual(board2, expected1);

        Board board3 = Board.FromFEN("r2qk2r/1b1nb1pp/p3pn2/1pp5/4P3/2NB1N2/PP2QPPP/R1B2RK1 b kq - 1 13");

        // 13. ...O-O
        Move move13b = new(Position.From("e8"), Position.From("g8"));
        board3.ApplyMove(move13b, out var board4);
        Board expected2 = Board.FromFEN("r2q1rk1/1b1nb1pp/p3pn2/1pp5/4P3/2NB1N2/PP2QPPP/R1B2RK1 w - - 2 14");
        BoardAssert.AreEqual(board4, expected2);
    }

    [Test]
    public void EnPassantApplyMoveTest()
    {
        Board board = Board.FromFEN("r1bqkb1r/ppp2ppp/8/3pP3/P2Q4/8/1PP2PPP/RNB2RK1 w kq d6 0 10");
        //10.exd6
        Move move = new(Position.From("e5"), Position.From("d6"));
        board.ApplyMove(move, out var actual);
        Board expected = Board.FromFEN("r1bqkb1r/ppp2ppp/3P4/8/P2Q4/8/1PP2PPP/RNB2RK1 b kq - 0 10");
        BoardAssert.AreEqual(actual, expected);
    }

    [Test]
    public void PromotionApplyMoveTest()
    {
        Board board = Board.FromFEN("r3kb1r/ppP2ppp/5q2/8/P2Q4/7b/1PP2PPP/RNB2RK1 w kq - 1 12");
        Move move = new(Position.From("c7"), Position.From("c8"), PieceType.Queen);
        board.ApplyMove(move, out var actual);
        Board expected = Board.FromFEN("r1Q1kb1r/pp3ppp/5q2/8/P2Q4/7b/1PP2PPP/RNB2RK1 b kq - 0 12");
        BoardAssert.AreEqual(actual, expected);
    }

    [Test]
    public void ApplyMove_ReturnsIllegalMove_WhenMoveLeavesKingInCheck()
    {
        Board board = Board.FromFEN("4r1k1/P7/8/8/8/8/8/4K3 w - - 0 1");
        Move illegalPromotion = new(Position.From("a7"), Position.From("a8"), PieceType.Queen);

        var result = board.ApplyMove(illegalPromotion, out var actual);

        Assert.AreEqual(MoveResult.IllegalMove, result);
        BoardAssert.AreEqual(actual, board);
        Assert.False(board.IsLegalMove(illegalPromotion));
    }

    [Test]
    public void Game_RejectsRepeatedIllegalPromotionWithoutCorruptingBoard()
    {
        Board initialBoard = Board.FromFEN("4r1k1/P7/8/8/8/8/8/4K3 w - - 0 1");
        Game game = new(initialBoard);
        Move illegalPromotion = new(Position.From("a7"), Position.From("a8"), PieceType.Queen);

        var firstAttempt = game.ApplyMove(illegalPromotion);
        var secondAttempt = game.ApplyMove(illegalPromotion);

        Assert.AreEqual(MoveResult.IllegalMove, firstAttempt);
        Assert.AreEqual(MoveResult.IllegalMove, secondAttempt);
        Assert.AreEqual(1, game.BoardHistory.Count);
        BoardAssert.AreEqual(game.CurrentBoard, initialBoard);
    }
}
