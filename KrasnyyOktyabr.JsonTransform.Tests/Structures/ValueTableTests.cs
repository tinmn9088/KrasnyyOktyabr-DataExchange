using KrasnyyOktyabr.JsonTransform.Structures;

namespace KrasnyyOktyabr.JsonTransform.Tests.Structures;

[TestClass]
public class ValueTableTests
{
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ValueTable_WhenNoColumnProvided_ShouldThrowArgumentException()
    {
        new ValueTable([]);
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void SelectLine_WhenIndexNotExist_ShouldThrowIndexOutOfRangeException()
    {
        ValueTable table = new(["Column"]);
        table.SelectLine(2);
    }

    [TestMethod]
    public void ValueTable_ShouldStoreValues()
    {
        // Arrange
        string column1 = "StringColumn";
        string column2 = "StringColumn";

        // Act
        ValueTable table = new([column1, column2]);

        table.AddLine();
        table.SetValue(column1, "TestValue");
        table.SetValue(column2, 666);

        table.AddLine();
        table.SetValue(column2, 999);

        // Assert
        Assert.AreEqual(2, table.Count);

        table.SelectLine(0);
        Assert.AreEqual(666, table.GetValue(column2));
    }
}
