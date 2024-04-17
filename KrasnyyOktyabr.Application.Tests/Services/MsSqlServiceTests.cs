using System.Runtime.Versioning;
using static KrasnyyOktyabr.Application.Services.MsSqlService;

namespace KrasnyyOktyabr.Application.Services.Tests;

[SupportedOSPlatform("windows")]
[TestClass]
public class MsSqlServiceTests
{
    [TestMethod]
    [ExpectedException(typeof(ValueMappingForSqlCommandNotFoundException))]
    public void BuildInsertQueryText_WhenUnsupportedValue_ShouldThrowValueMappingForSqlCommandNotFoundException()
    {
        string table = "TestTable";
        Dictionary<string, dynamic> columnsValues = new() { { "UnsupportedValue", new MsSqlServiceTests() } };

        BuildInsertQueryText(table, columnsValues);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildInsertQueryText_WhenIllegalTableName_ShouldThrowArgumentException()
    {
        string table = "IllegalCharacterIs]";
        Dictionary<string, dynamic> columnsValues = [];

        BuildInsertQueryText(table, columnsValues);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void BuildInsertQueryText_WhenIllegalColumnName_ShouldThrowArgumentException()
    {
        string table = "TestTable";
        Dictionary<string, dynamic> columnsValues = new() { { "IllegalCharacterIs]", null! } };

        BuildInsertQueryText(table, columnsValues);
    }

    [TestMethod]
    public void BuildInsertQueryText_ShouldBuildInsertQueryText()
    {
        string table = "TestTable";
        Dictionary<string, dynamic> columnsValues = new()
            {
                { "StringColumn", "Value" },
                { "IntColumn", 123 },
                { "RealColumn", 0.33 },
                { "NullColumn", null! },
            };
        string expected = "INSERT INTO [TestTable] ([StringColumn],[IntColumn],[RealColumn],[NullColumn]) VALUES ('Value',123,0.33,NULL)";

        string actual = BuildInsertQueryText(table, columnsValues);

        Assert.AreEqual(expected, actual);
    }
}
