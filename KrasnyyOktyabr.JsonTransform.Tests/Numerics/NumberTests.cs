namespace KrasnyyOktyabr.JsonTransform.Numerics.Tests;

[TestClass]
public class NumberTests
{
    [TestMethod]
    public void Number_WhenInt_ShouldCreateInt()
    {
        int value = 66;

        Number intNumber = new(value);

        Assert.IsNull(intNumber.Double);
        Assert.AreEqual(intNumber.Int, value);
    }

    [TestMethod]
    public void Number_WhenDouble_ShouldCreateDouble()
    {
        double value = 66.6;

        Number intNumber = new(value);

        Assert.IsNull(intNumber.Int);
        Assert.AreEqual(intNumber.Double, value);
    }

    [TestMethod]
    public void Addition_WhenIntAndInt_ShouldReturnInt()
    {
        Number left = new(3);
        Number right = new(2);

        Number result = left + right;

        Assert.IsNull(result.Double);
        Assert.AreEqual(5, result.Int);
    }

    [TestMethod]
    public void Addition_WhenIntAndDouble_ShouldReturnDouble()
    {
        Number left = new(3);
        Number right = new(2.1);

        Number result = left + right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(5.1, result.Double);
    }

    [TestMethod]
    public void Addition_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(3.1);
        Number right = new(2);

        Number result = left + right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(5.1, result.Double);
    }

    [TestMethod]
    public void Addition_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(3.1);
        Number right = new(2.5);

        Number result = left + right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(5.6, result.Double);
    }

    [TestMethod]
    public void Subtraction_WhenIntAndInt_ShouldReturnInt()
    {
        Number left = new(3);
        Number right = new(2);

        Number result = left - right;

        Assert.IsNull(result.Double);
        Assert.AreEqual(1, result.Int);
    }

    [TestMethod]
    public void Subtraction_WhenIntAndDouble_ShouldReturnDouble()
    {
        Number left = new(3);
        Number right = new(2.5);

        Number result = left - right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(0.5, result.Double);
    }

    [TestMethod]
    public void Subtraction_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(3.1);
        Number right = new(2);

        Number result = left - right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(1.1, result.Double);
    }

    [TestMethod]
    public void Subtraction_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(3.6);
        Number right = new(2.5);

        Number result = left - right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(1.1, result.Double);
    }

    [TestMethod]
    public void Multiplication_WhenIntAndInt_ShouldReturnInt()
    {
        Number left = new(3);
        Number right = new(2);

        Number result = left * right;

        Assert.IsNull(result.Double);
        Assert.AreEqual(6, result.Int);
    }

    [TestMethod]
    public void Multiplication_WhenIntAndDouble_ShouldReturnDouble()
    {
        Number left = new(3);
        Number right = new(2.5);

        Number result = left * right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(7.5, result.Double);
    }

    [TestMethod]
    public void Multiplication_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(3.1);
        Number right = new(2);

        Number result = left * right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(6.2, result.Double);
    }

    [TestMethod]
    public void Multiplication_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(0.5);
        Number right = new(2.5);

        Number result = left * right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(1.25, result.Double);
    }

    [TestMethod]
    public void Division_WhenIntAndIntWithMultiplicity_ShouldReturnInt()
    {
        Number left = new(6);
        Number right = new(2);

        Number result = left / right;

        Assert.IsNull(result.Double);
        Assert.AreEqual(3, result.Int);
    }

    [TestMethod]
    public void Division_WhenIntAndInt_ShouldReturnDouble()
    {
        Number left = new(6);
        Number right = new(4);

        Number result = left / right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(1.5, result.Double);
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
        Number right = new(2.0);

        Number result = left / right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(2.5, result.Double);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Division_WhenIntAndDoubleZero_ShouldThrowDivideByZeroException()
    {
        Number left = new(5);
        Number right = new(0.0);

        Number _ = left / right;
    }

    [TestMethod]
    public void Division_WhenDoubleAndInt_ShouldReturnDouble()
    {
        Number left = new(2.4);
        Number right = new(2);

        Number result = left / right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(1.2, result.Double);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Division_WhenDoubleAndIntZero_ShouldThrowDivideByZeroException()
    {
        Number left = new(2.4);
        Number right = new(0);

        Number _ = left / right;
    }

    [TestMethod]
    public void Division_WhenDoubleAndDouble_ShouldReturnDouble()
    {
        Number left = new(2.4);
        Number right = new(0.5);

        Number result = left / right;

        Assert.IsNull(result.Int);
        Assert.AreEqual(4.8, result.Double);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Division_WhenDoubleAndDoubleZero_ShouldThrowDivideByZeroException()
    {
        Number left = new(2.4);
        Number right = new(0.0);

        Number _ = left / right;
    }

    [TestMethod]
    public void Equality_ShouldReturnTrueIfEqual()
    {
        Assert.IsTrue(new Number(1) == new Number(1));
        Assert.IsFalse(new Number(1) != new Number(1));

        Assert.IsTrue(new Number(1) == new Number(1.0));
        Assert.IsFalse(new Number(1) != new Number(1.0));

        Assert.IsTrue(new Number(1.0) == new Number(1));
        Assert.IsFalse(new Number(1.0) != new Number(1));

        Assert.IsTrue(new Number(1.0) == new Number(1.0));
        Assert.IsFalse(new Number(1.0) != new Number(1.0));
    }

    [TestMethod]
    public void Inequality_ShouldReturnTrueIfNotEqual()
    {
        Assert.IsTrue(new Number(1) != new Number(2));
        Assert.IsFalse(new Number(1) == new Number(2));

        Assert.IsTrue(new Number(1) != new Number(2.0));
        Assert.IsFalse(new Number(1) == new Number(2.0));

        Assert.IsTrue(new Number(1.0) != new Number(2));
        Assert.IsFalse(new Number(1.0) == new Number(2));

        Assert.IsTrue(new Number(1.0) != new Number(2.0));
        Assert.IsFalse(new Number(1.0) == new Number(2.0));
    }

    [TestMethod]
    public void Less_ShouldReturnTrueIfLess()
    {
        Assert.IsTrue(new Number(1) < new Number(2));
        Assert.IsFalse(new Number(2) < new Number(1));

        Assert.IsTrue(new Number(1) < new Number(2.0));
        Assert.IsFalse(new Number(2) < new Number(1.0));

        Assert.IsTrue(new Number(1.0) < new Number(2));
        Assert.IsFalse(new Number(2.0) < new Number(1));

        Assert.IsTrue(new Number(1.0) < new Number(2.0));
        Assert.IsFalse(new Number(2.0) < new Number(1.0));
    }

    [TestMethod]
    public void Greater_ShouldReturnTrueIfGreater()
    {
        Assert.IsTrue(new Number(2) > new Number(1));
        Assert.IsFalse(new Number(1) > new Number(2));

        Assert.IsTrue(new Number(2) > new Number(1.0));
        Assert.IsFalse(new Number(1) > new Number(2.0));

        Assert.IsTrue(new Number(2.0) > new Number(1));
        Assert.IsFalse(new Number(1.0) > new Number(2));

        Assert.IsTrue(new Number(2.0) > new Number(1.0));
        Assert.IsFalse(new Number(1.0) > new Number(2.0));
    }

    [TestMethod]
    public void ToString_ShouldReturnString()
    {
        Assert.AreEqual("1", new Number(1).ToString());
        Assert.AreEqual("1", new Number(1.0).ToString());
        Assert.AreEqual("1.5", new Number(1.5).ToString());
    }
}
