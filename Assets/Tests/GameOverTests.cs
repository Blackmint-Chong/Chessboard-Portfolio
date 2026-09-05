using Chess;
using NUnit.Framework;
using UnityEditorInternal;

public class GameOverTests
{
    [Test]
    public void CheckMateTest()
    {
        Assert.IsTrue(Board.FromFEN("rnbqkbnr/ppppp2p/8/5ppB/4P3/8/PPPP1PPP/RNBQK1NR b KQkq - 1 3").IsCheckMate());

        Assert.IsFalse(Board.FromFEN("rnbqkb1r/ppppp2p/5n2/5ppQ/4P3/3B4/PPPP1PPP/RNB1K1NR b KQkq - 3 4").IsCheckMate());
    }

    [Test]
    public void StaleMateTest()
    {
        Assert.IsTrue(Board.FromFEN("5bnr/4p1pq/4Qpkr/7p/7P/4P3/PPPP1PP1/RNB1KBNR b KQ - 2 10").IsStaleMate());

        Assert.IsFalse(Board.FromFEN("r1bqkb1r/ppp2ppp/2n2n2/3pp1N1/2B1P3/8/PPPP1PPP/RNBQK2R w KQkq d6 0 5").IsStaleMate());
    }

    [Test]
    public void InsufficientMaterialTest()
    {
        // king - king
        Assert.IsTrue(Board.FromFEN("3k4/8/8/8/8/8/8/3K4 w - - 0 1").IsInsufficientMaterialDraw());

        // king + knight - king
        Assert.IsTrue(Board.FromFEN("3k4/8/8/8/8/8/8/3KN3 w - - 0 1").IsInsufficientMaterialDraw());

        // king + bishop - king
        Assert.IsTrue(Board.FromFEN("3k4/8/8/8/8/8/8/3KB3 w - - 0 1").IsInsufficientMaterialDraw());

        // king + same color bishops - king
        Assert.IsTrue(Board.FromFEN("3k4/8/8/8/8/2B5/7B/3KB3 w - - 0 1").IsInsufficientMaterialDraw());

        // king + same color bishops - king + same color bishops
        Assert.IsTrue(Board.FromFEN("3k4/4b1b1/8/b7/8/8/1B6/3KB1B1 w - - 0 1").IsInsufficientMaterialDraw());


        // False case (not draw)
        // king + pawn - king
        Assert.IsFalse(Board.FromFEN("3k4/8/8/8/8/8/5P2/3K4 w - - 0 1").IsInsufficientMaterialDraw());

        // king + knight + knight - king
        Assert.IsFalse(Board.FromFEN("4k3/8/8/8/8/8/3NN3/4K3 w - - 0 1").IsInsufficientMaterialDraw());

        // king + different color bishops - king
        Assert.IsFalse(Board.FromFEN("4k3/8/8/8/8/8/3BB3/4K3 w - - 0 1").IsInsufficientMaterialDraw());

        // king + white bishop - king + black bishop
        Assert.IsFalse(Board.FromFEN("4k3/3b4/8/8/8/8/3B4/4K3 w - - 0 1").IsInsufficientMaterialDraw());

        // king + bishop + knight - king
        Assert.IsFalse(Board.FromFEN("4k3/8/8/8/8/8/3B1N2/4K3 w - - 0 1").IsInsufficientMaterialDraw());

        // initial state
        Assert.IsFalse(Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1").IsInsufficientMaterialDraw());
    }

    [Test]
    public void ThreefoldRepetitionTest()
    {
        var game = new Game(Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

        game.ApplyMove(new Move(new Position('g', 1), new Position('f', 3)));
        game.ApplyMove(new Move(new Position('g', 8), new Position('f', 6)));
        game.ApplyMove(new Move(new Position('f', 3), new Position('g', 1)));
        game.ApplyMove(new Move(new Position('f', 6), new Position('g', 8)));

        Assert.IsFalse(game.IsThreefoldRepetition());

        game.ApplyMove(new Move(new Position('g', 1), new Position('f', 3)));
        game.ApplyMove(new Move(new Position('g', 8), new Position('f', 6)));
        game.ApplyMove(new Move(new Position('f', 3), new Position('g', 1)));
        game.ApplyMove(new Move(new Position('f', 6), new Position('g', 8)));

        Assert.IsTrue(game.IsThreefoldRepetition());

        game.Undo();

        Assert.IsFalse(game.IsThreefoldRepetition());
    }

    [Test]
    public void Game_RejectsMoveAfterThreefoldRepetitionDraw()
    {
        var game = new Game(Board.FromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));

        game.ApplyMove(new Move(new Position('g', 1), new Position('f', 3)));
        game.ApplyMove(new Move(new Position('g', 8), new Position('f', 6)));
        game.ApplyMove(new Move(new Position('f', 3), new Position('g', 1)));
        game.ApplyMove(new Move(new Position('f', 6), new Position('g', 8)));
        game.ApplyMove(new Move(new Position('g', 1), new Position('f', 3)));
        game.ApplyMove(new Move(new Position('g', 8), new Position('f', 6)));
        game.ApplyMove(new Move(new Position('f', 3), new Position('g', 1)));
        game.ApplyMove(new Move(new Position('f', 6), new Position('g', 8)));

        var boardBeforeAttempt = game.CurrentBoard;
        int historyCountBeforeAttempt = game.BoardHistory.Count;

        var result = game.ApplyMove(new Move(new Position('g', 1), new Position('f', 3)));

        Assert.AreEqual(MoveResult.IllegalMove, result);
        Assert.AreSame(boardBeforeAttempt, game.CurrentBoard);
        Assert.AreEqual(historyCountBeforeAttempt, game.BoardHistory.Count);
    }

    [Test]
    public void Game_RejectsMoveAfterFiftyMoveRuleDraw()
    {
        var game = new Game(Board.FromFEN("4k3/8/8/8/8/8/8/R3K3 w - - 100 1"));
        var boardBeforeAttempt = game.CurrentBoard;

        var result = game.ApplyMove(new Move(Position.From("a1"), Position.From("a2")));

        Assert.AreEqual(MoveResult.IllegalMove, result);
        Assert.AreSame(boardBeforeAttempt, game.CurrentBoard);
        Assert.AreEqual(1, game.BoardHistory.Count);
    }

    [Test]
    public void Game_RejectsMoveAfterInsufficientMaterialDraw()
    {
        var game = new Game(Board.FromFEN("4k3/8/8/8/8/8/8/4K3 w - - 0 1"));
        var boardBeforeAttempt = game.CurrentBoard;

        var result = game.ApplyMove(new Move(Position.From("e1"), Position.From("d1")));

        Assert.AreEqual(MoveResult.IllegalMove, result);
        Assert.AreSame(boardBeforeAttempt, game.CurrentBoard);
        Assert.AreEqual(1, game.BoardHistory.Count);
    }

    [Test]
    public void GetDrawReason_ReturnsSpecificReason()
    {
        var normalGame = new Game(Board.FromFEN(
            "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"));
        var stalemateGame = new Game(Board.FromFEN(
            "5bnr/4p1pq/4Qpkr/7p/7P/4P3/PPPP1PP1/RNB1KBNR b KQ - 2 10"));
        var insufficientMaterialGame = new Game(Board.FromFEN(
            "4k3/8/8/8/8/8/8/4K3 w - - 0 1"));
        var fiftyMoveGame = new Game(Board.FromFEN(
            "4k3/8/8/8/8/8/8/R3K3 w - - 100 1"));

        Assert.AreEqual(DrawReason.None, normalGame.GetDrawReason());
        Assert.AreEqual(DrawReason.Stalemate, stalemateGame.GetDrawReason());
        Assert.AreEqual(
            DrawReason.InsufficientMaterial,
            insufficientMaterialGame.GetDrawReason());
        Assert.AreEqual(DrawReason.FiftyMoveRule, fiftyMoveGame.GetDrawReason());

        normalGame.ApplyMove(new Move(Position.From("g1"), Position.From("f3")));
        normalGame.ApplyMove(new Move(Position.From("g8"), Position.From("f6")));
        normalGame.ApplyMove(new Move(Position.From("f3"), Position.From("g1")));
        normalGame.ApplyMove(new Move(Position.From("f6"), Position.From("g8")));
        normalGame.ApplyMove(new Move(Position.From("g1"), Position.From("f3")));
        normalGame.ApplyMove(new Move(Position.From("g8"), Position.From("f6")));
        normalGame.ApplyMove(new Move(Position.From("f3"), Position.From("g1")));
        normalGame.ApplyMove(new Move(Position.From("f6"), Position.From("g8")));

        Assert.AreEqual(
            DrawReason.ThreefoldRepetition,
            normalGame.GetDrawReason());
    }
}
