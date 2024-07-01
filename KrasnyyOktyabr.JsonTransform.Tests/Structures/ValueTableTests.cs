using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Structures.Tests;

[TestClass]
public class ValueTableTests
{
    public required TestContext TestContext
    {
        get; set;
    }

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
        string column2 = "NumberColumn";

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

    [TestMethod]
    public void Collapse_ShouldGroupByColumnsProvided()
    {
        // Arrange
        string column1 = "Column1";
        string column2 = "Column2";

        ValueTable table = new([column1, column2]);

        table.AddLine();
        table.SetValue(column1, "Group1");
        table.SetValue(column2, "Group1");

        table.AddLine();
        table.SetValue(column1, "Group1");
        table.SetValue(column2, "Group1");

        table.AddLine();
        table.SetValue(column1, "Group2");
        table.SetValue(column2, "Group2");

        table.AddLine();
        table.SetValue(column1, "Group3");
        table.SetValue(column2, "Group3");

        table.AddLine();
        table.SetValue(column1, "Group3");
        table.SetValue(column2, "Group3");

        // Act
        table.Collapse([column1, column2]);

        // Assert
        Assert.AreEqual(3, table.Count);
    }

    [TestMethod()]
    public void Collapse_WhenColumnsToSumProvided_ShouldGroupAndSum()
    {
        // Arrange
        string column1 = "String1Column";
        string column2 = "String2Column";
        string column3 = "Number1Column";
        string column4 = "Number2Column";

        ValueTable table = new([column1, column2, column3, column4]);

        table.AddLine();
        table.SetValue(column1, "Group1");
        table.SetValue(column2, "Group1");
        table.SetValue(column3, 1);
        table.SetValue(column4, 2);

        table.AddLine();
        table.SetValue(column1, "Group1");
        table.SetValue(column2, "Group1");
        table.SetValue(column3, 3);
        table.SetValue(column4, 4);

        table.AddLine();
        table.SetValue(column1, "Group2");
        table.SetValue(column2, "Group2");
        table.SetValue(column3, 5);
        table.SetValue(column4, 5);

        table.AddLine();
        table.SetValue(column1, "Group3");
        table.SetValue(column2, "Group3");
        table.SetValue(column3, 6);
        table.SetValue(column4, 7);

        table.AddLine();
        table.SetValue(column1, "Group3");
        table.SetValue(column2, "Group3");
        table.SetValue(column3, 8);
        table.SetValue(column4, 9);

        // Act
        table.Collapse([column1, column2], [column3, column4]);

        // Assert
        Assert.AreEqual(3, table.Count);
    }

    [TestMethod]
    public async Task Collapse_ShouldNotThrowOverflowException()
    {
        // Arrange
        string[] valueTableColumnToGroup = ["GroupColumn1", "GroupColumn2", "GroupColumn3"];
        string valueTableColumnToCollapse = "SumColumn";
        string[] valueTableColumns = [.. valueTableColumnToGroup, valueTableColumnToCollapse];
        ValueTable valueTable = new(valueTableColumns);

        JArray valueTableData = await GetCurrentTestDataAsync<JArray>();

        foreach (JToken valueTableDataItem in valueTableData)
        {
            JArray? valueTableRow = valueTableDataItem as JArray;

            if (valueTableRow is null)
            {
                throw new ArgumentException($"Must to be array: {valueTableRow}");
            }

            valueTable.AddLine();

            for (int i = 0; i < valueTableColumns.Length; i++)
            {
                valueTable.SetValue(valueTableColumns[i], valueTableRow[i]);
            }
        }

        // Act
        valueTable.Collapse(valueTableColumnToGroup, [valueTableColumnToCollapse]);

        // Assert
        Assert.AreEqual(262, valueTable.Count);
    }

    [TestMethod]
    public void AddColumn_ShouldAddColumn()
    {
        // Arrange
        string column1 = "Column1";
        string column2 = "Column2";

        ValueTable table = new([column1]);

        table.AddLine();
        table.SetValue(column1, "TestValue1");

        table.AddLine();
        table.SetValue(column1, "TestValue2");

        // Act
        table.AddColumn(column2);

        // Assert
        Assert.AreEqual(2, table.Columns.Count);

        table.SetValue(column2, "TestValue3");
    }

    /// <exception cref="NullReferenceException"></exception>
    private async Task<T> GetCurrentTestDataAsync<T>() where T : JToken
    {
        return await LoadJsonAsync<T>(GetType(), TestContext.TestName ?? throw new NullReferenceException());
    }
}
