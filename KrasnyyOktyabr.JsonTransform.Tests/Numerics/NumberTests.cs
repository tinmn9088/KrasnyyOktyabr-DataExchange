namespace KrasnyyOktyabr.JsonTransform.Numerics.Tests;

[TestClass]
public class NumberTests
{
    [TestMethod]
    public void Number_WhenInt_ShouldCreateInt()
    {
        int value = 66;

        Number intNumber = new(value);

        Assert.IsNull(intNumber.Decimal);
        Assert.AreEqual(intNumber.Long, value);
    }

    [TestMethod]
    public void Number_WhenDouble_ShouldCreateDouble()
    {
        decimal value = 66.6M;

        Number intNumber = new(value);

        Assert.IsNull(intNumber.Long);
        Assert.AreEqual(intNumber.Decimal, value);
    }

    [TestMethod]
    public void Addition_WhenIntAndInt_ShouldReturnInt()
    {
        Number left = new(3);
        Number right = new(2);

        Number result = left + right;

        Assert.IsNull(result.Decimal);
        Assert.AreEqual(5, result.Long);
    }

    [TestMethod]
    public void Addition_WhenIntAndDouble_ShouldReturnDouble()
    {
        Number left = new(3);
        Number right = new(2.1M);

        Number result = left + right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(5.1M, result.Decimal);
    }

    [TestMethod]
    public void Addition_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(3.1M);
        Number right = new(2);

        Number result = left + right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(5.1M, result.Decimal);
    }

    [TestMethod]
    public void Addition_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(3.1M);
        Number right = new(2.5M);

        Number result = left + right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(5.6M, result.Decimal);
    }

    [TestMethod]
    public void Subtraction_WhenIntAndInt_ShouldReturnInt()
    {
        Number left = new(3);
        Number right = new(2);

        Number result = left - right;

        Assert.IsNull(result.Decimal);
        Assert.AreEqual(1, result.Long);
    }

    [TestMethod]
    public void Subtraction_WhenIntAndDouble_ShouldReturnDouble()
    {
        Number left = new(3);
        Number right = new(2.5M);

        Number result = left - right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(0.5M, result.Decimal);
    }

    [TestMethod]
    public void Subtraction_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(3.1M);
        Number right = new(2);

        Number result = left - right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(1.1M, result.Decimal);
    }

    [TestMethod]
    public void Subtraction_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(3.6M);
        Number right = new(2.5M);

        Number result = left - right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(1.1M, result.Decimal);
    }

    [TestMethod]
    public void Multiplication_WhenIntAndInt_ShouldReturnInt()
    {
        Number left = new(3);
        Number right = new(2);

        Number result = left * right;

        Assert.IsNull(result.Decimal);
        Assert.AreEqual(6, result.Long);
    }

    [TestMethod]
    public void Multiplication_WhenIntAndDouble_ShouldReturnDouble()
    {
        Number left = new(3);
        Number right = new(2.5M);

        Number result = left * right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(7.5M, result.Decimal);
    }

    [TestMethod]
    public void Multiplication_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(3.1M);
        Number right = new(2);

        Number result = left * right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(6.2M, result.Decimal);
    }

    [TestMethod]
    public void Multiplication_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(0.5M);
        Number right = new(2.5M);

        Number result = left * right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(1.25M, result.Decimal);
    }

    [TestMethod]
    public void Division_WhenIntAndIntWithMultiplicity_ShouldReturnInt()
    {
        Number left = new(6);
        Number right = new(2);

        Number result = left / right;

        Assert.IsNull(result.Decimal);
        Assert.AreEqual(3, result.Long);
    }

    [TestMethod]
    public void Division_WhenIntAndInt_ShouldReturnDouble()
    {
        Number left = new(6);
        Number right = new(4);

        Number result = left / right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(1.5M, result.Decimal);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Division_WhenIntAndIntZero_ShouldThrowDivideByZeroException()
    {
        Number left = new(6);
        Number right = new(0);

        Number _ = left / right;
    }

    [TestMethod]
    public void Division_WhenIntAndDouble_ShouldReturnDouble()
    {
        Number left = new(5);
        Number right = new(2.0M);

        Number result = left / right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(2.5M, result.Decimal);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Division_WhenIntAndDoubleZero_ShouldThrowDivideByZeroException()
    {
        Number left = new(5);
        Number right = new(0.0M);

        Number _ = left / right;
    }

    [TestMethod]
    public void Division_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(2.4M);
        Number right = new(2);

        Number result = left / right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(1.2M, result.Decimal);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Division_WhenDoubleAndIntZero_ShouldThrowDivideByZeroException()
    {
        Number left = new(2.4M);
        Number right = new(0);

        Number _ = left / right;
    }

    [TestMethod]
    public void Division_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(2.4M);
        Number right = new(0.5M);

        Number result = left / right;

        Assert.IsNull(result.Long);
        Assert.AreEqual(4.8M, result.Decimal);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Division_WhenDoubleAndDoubleZero_ShouldThrowDivideByZeroException()
    {
        Number left = new(2.4M);
        Number right = new(0.0M);

        Number _ = left / right;
    }

    [TestMethod]
    public void Equality_ShouldReturnTrueIfEqual()
    {
        Assert.IsTrue(new Number(1) == new Number(1));
        Assert.IsFalse(new Number(1) != new Number(1));

        Assert.IsTrue(new Number(1) == new Number(1.0M));
        Assert.IsFalse(new Number(1) != new Number(1.0M));

        Assert.IsTrue(new Number(1.0M) == new Number(1));
        Assert.IsFalse(new Number(1.0M) != new Number(1));

        Assert.IsTrue(new Number(1.0M) == new Number(1.0M));
        Assert.IsFalse(new Number(1.0M) != new Number(1.0M));
    }

    [TestMethod]
    public void Inequality_ShouldReturnTrueIfNotEqual()
    {
        Assert.IsTrue(new Number(1) != new Number(2));
        Assert.IsFalse(new Number(1) == new Number(2));

        Assert.IsTrue(new Number(1) != new Number(2.0M));
        Assert.IsFalse(new Number(1) == new Number(2.0M));

        Assert.IsTrue(new Number(1.0M) != new Number(2));
        Assert.IsFalse(new Number(1.0M) == new Number(2));

        Assert.IsTrue(new Number(1.0M) != new Number(2.0M));
        Assert.IsFalse(new Number(1.0M) == new Number(2.0M));
    }

    [TestMethod]
    public void Less_ShouldReturnTrueIfLess()
    {
        Assert.IsTrue(new Number(1) < new Number(2));
        Assert.IsFalse(new Number(2) < new Number(1));

        Assert.IsTrue(new Number(1) < new Number(2.0M));
        Assert.IsFalse(new Number(2) < new Number(1.0M));

        Assert.IsTrue(new Number(1.0M) < new Number(2));
        Assert.IsFalse(new Number(2.0M) < new Number(1));

        Assert.IsTrue(new Number(1.0M) < new Number(2.0M));
        Assert.IsFalse(new Number(2.0M) < new Number(1.0M));
    }

    [TestMethod]
    public void Greater_ShouldReturnTrueIfGreater()
    {
        Assert.IsTrue(new Number(2) > new Number(1));
        Assert.IsFalse(new Number(1) > new Number(2));

        Assert.IsTrue(new Number(2) > new Number(1.0M));
        Assert.IsFalse(new Number(1) > new Number(2.0M));

        Assert.IsTrue(new Number(2.0M) > new Number(1));
        Assert.IsFalse(new Number(1.0M) > new Number(2));

        Assert.IsTrue(new Number(2.0M) > new Number(1.0M));
        Assert.IsFalse(new Number(1.0M) > new Number(2.0M));
    }

    [TestMethod]
    public void LessOrEqual_ShouldReturnTrueIfLess()
    {
        Assert.IsTrue(new Number(2) <= new Number(2));
        Assert.IsTrue(new Number(2) <= new Number(2.0M));
        Assert.IsTrue(new Number(2.0M) <= new Number(2));
        Assert.IsTrue(new Number(2.0M) <= new Number(2.0M));
    }

    [TestMethod]
    public void GreaterOrEqual_ShouldReturnTrueIfGreater()
    {
        Assert.IsTrue(new Number(2) >= new Number(2));
        Assert.IsTrue(new Number(2) >= new Number(2.0M));
        Assert.IsTrue(new Number(2.0M) >= new Number(2));
        Assert.IsTrue(new Number(2.0M) >= new Number(2.0M));
    }

    [TestMethod]
    public void ToString_ShouldReturnString()
    {
        Assert.AreEqual("1", new Number(1).ToString());
        Assert.AreEqual("1", new Number(1.0M).ToString());
        Assert.AreEqual("1.5", new Number(1.5M).ToString());
    }
}
