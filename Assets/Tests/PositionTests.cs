using NUnit.Framework;
using Chess;
public class PositionTests
{
    [Test]
    public void PositionInitializationTest()
    {
        Position p1 = new(5, 4);
        Assert.AreEqual(p1.File, 5);
        Assert.AreEqual(p1.Rank, 4);

        Position p2 = new('e', 4);
        Assert.AreEqual(4, p2.File);
        Assert.AreEqual(3, p2.Rank);
    }

    [Test]
    public void PositionTryOffsetTest()
    {
        Position p1 = new(0, 0);
        Assert.IsTrue(p1.TryOffset(2, 2, out p1));
        Assert.AreEqual(p1.File, 2);
        Assert.AreEqual(p1.Rank, 2);

        Position p2 = new(0, 0);
        Assert.IsFalse(p2.TryOffset(2, -1, out p2));

        Position p3 = new(0, 0);
        Assert.IsFalse(p3.TryOffset(2, 10, out p3));

        Position p4 = new(0, 0);
        Assert.IsFalse(p3.TryOffset(-1, 1, out p3));

        Position p5 = new(0, 0);
        Assert.IsFalse(p3.TryOffset(10, 1, out p3));
    }
}
