using Newtonsoft.Json.Linq;
using static KrasnyyOktyabr.JsonTransform.IContext;
using static KrasnyyOktyabr.JsonTransform.Tests.TestsHelper;

namespace KrasnyyOktyabr.JsonTransform.Tests;

[TestClass]
public class ContextTests
{
    public required TestContext TestContext
    {
        get; set;
    }

    private static readonly JObject s_emptyJsonObject = new();

    private Context? _context;

    [TestInitialize]
    public void Initialize()
    {
        string? testName = TestContext.TestName;

        if (testName != null && SkipTestInitialize(GetType(), testName))
        {
            return;
        }

        _context = new Context(s_emptyJsonObject);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemorySet_WhenNameNull_ShouldThrowArgumentNullException()
    {
        _context!.MemorySet(null!, null!);
    }

    [TestMethod]
    public void MemorySet_ShouldAddValueToMemory()
    {
        string name = "TestName";
        string value = "TestValue";

        _context!.MemorySet(name, value);
        string actual = (string)(_context.MemoryGet(name) ?? throw new NullReferenceException());

        Assert.AreEqual(value, actual);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void MemoryGet_WhenNameNull_ShouldThrowArgumentNullException()
    {
        _context!.MemoryGet(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(IContext.MemoryValueNotFoundException))]
    public void MemoryGet_WhenValueAbsent_ShouldThrowMemoryValueNotFoundException()
    {
        _context!.MemoryGet("AbsentValueKey");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void InputSelect_WhenPathNull_ShouldThrowArgumentNullException()
    {
        _context!.InputSelect(null!);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void InputSelect_WhenPathEmpty_ShouldThrowArgumentNullException()
    {
        _context!.InputSelect(string.Empty);
    }

    [TestMethod]
    [SkipTestInitialize]
    public void InputSelect_WhenPathLeadsToPrimitiveValue_ShouldReturnPrimitiveValue()
    {
        string key = "primitive";
        int value = 6;
        JObject input = new()
        {
            { key, value },
        };
        string path = key;

        _context = new Context(input);
        JToken? result = _context!.InputSelect(path);

        Assert.IsNotNull(result);
        Assert.AreEqual(JTokenType.Integer, result.Type);
        Assert.IsTrue(JToken.DeepEquals(result, value));
    }

    [TestMethod]
    [SkipTestInitialize]
    public void InputSelect_WhenPathLeadsToObject_ShouldReturnObject()
    {
        string key = "obj";
        JObject value = new()
        {
            { "key", "value" },
        };
        JObject input = new()
        {
            { key, value },
        };
        string path = key;

        _context = new Context(input);
        JToken? result = _context!.InputSelect(path);

        Assert.IsNotNull(result);
        Assert.AreEqual(JTokenType.Object, result.Type);
        Assert.IsTrue(JToken.DeepEquals(result, value));
    }

    [TestMethod]
    [SkipTestInitialize]
    public void InputSelect_WhenPathLeadsToArray_ShouldReturnArray()
    {
        string key = "arr";
        JArray value = ["value1", "value2"];
        JObject input = new()
        {
            { key, value },
        };
        string path = key + "[:]";

        _context = new Context(input);
        JToken? result = _context!.InputSelect(path);

        Assert.IsNotNull(result);
        Assert.AreEqual(JTokenType.Array, result.Type);
        Assert.IsTrue(JToken.DeepEquals(result, value));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void OutputAdd_WhenKeyNull_ShouldThrowArgumentNullException()
    {
        _context!.OutputAdd(null!, null!, 0);
    }

    [TestMethod]
    [ExpectedException(typeof(IndexOutOfRangeException))]
    public void OutputAdd_WhenIndexNegative_ShouldThrowIndexOutOfRangeException()
    {
        string key = "TestKey";

        _context!.OutputAdd(key, null!, -1);
    }

    [TestMethod]
    public void OutputAdd_ShouldAddValueToOutput()
    {
        string key = "TestKey";
        string value = "TestValue";
        int index = 2;

        _context!.OutputAdd(key, value, index);
        List<JObject> output = _context.OutputGet();

        Assert.AreEqual(3, output.Count);
        Assert.IsTrue(JToken.DeepEquals(output[0], s_emptyJsonObject));
        Assert.IsTrue(JToken.DeepEquals(output[1], s_emptyJsonObject));
        Assert.AreEqual(1, output[2].Count);
        Assert.AreEqual(value, output[2][key]);
    }

    [TestMethod]
    public void UpdateCursor_ShoudSaveCursor()
    {
        string cursor1Name = "Cursor1";
        string cursor1LastValue = "LastValue1";
        int cursor1LastIndex = 1;
        string cursor2Name = "Cursor2";
        string cursor2LastValue = "LastValue2";
        int cursor2LastIndex = 0;

        Assert.ThrowsException<CursorNotFoundException>(() => _context!.GetCursor());
        Assert.ThrowsException<CursorIndexNotFoundException>(() => _context!.GetCursorIndex());

        // 1 start
        _context!.UpdateCursor(cursor1Name, "TestValue1", 0);
        _context!.UpdateCursor(cursor1Name, cursor1LastValue, cursor1LastIndex);

        Assert.AreEqual(cursor1LastValue, _context.GetCursor());
        Assert.AreEqual(cursor1LastIndex, _context.GetCursorIndex());

        // 2 start
        _context!.UpdateCursor(cursor2Name, cursor2LastValue, cursor2LastIndex);

        Assert.AreEqual(cursor2LastValue, _context.GetCursor());
        Assert.AreEqual(cursor2LastIndex, _context.GetCursorIndex());

        // 2 end
        _context.RemoveCursor(cursor2Name);

        // 2 start again
        _context!.UpdateCursor(cursor2Name, cursor2LastValue, cursor2LastIndex);

        Assert.AreEqual(cursor2LastValue, _context.GetCursor());
        Assert.AreEqual(cursor2LastIndex, _context.GetCursorIndex());

        // 2 end
        _context.RemoveCursor(cursor2Name);

        Assert.AreEqual(cursor1LastValue, _context.GetCursor());
        Assert.AreEqual(cursor1LastIndex, _context.GetCursorIndex());

        // 1 end
        _context.RemoveCursor(cursor1Name);

        Assert.ThrowsException<CursorNotFoundException>(() => _context!.GetCursor());
        Assert.ThrowsException<CursorIndexNotFoundException>(() => _context!.GetCursorIndex());
    }

    [TestMethod]
    [ExpectedException(typeof(CursorNotFoundException))]
    public void GetCursor_WhenNoName_ShoudThrowCursorNotFoundException()
    {
        _context!.GetCursor();
    }

    [TestMethod]
    [ExpectedException(typeof(CursorNotFoundException))]
    public void GetCursor_WhenNameInvalid_ShoudThrowCursorNotFoundException()
    {
        _context!.GetCursor("InvalidName");
    }

    [TestMethod]
    [ExpectedException(typeof(CursorIndexNotFoundException))]
    public void GetCursorIndex_WhenNoName_ShoudThrowsCursorIndexNotFoundException()
    {
        _context!.GetCursorIndex();
    }

    [TestMethod]
    [ExpectedException(typeof(CursorIndexNotFoundException))]
    public void GetCursorIndex_WhenNameInvalid_ShoudThrowsCursorIndexNotFoundException()
    {
        _context!.GetCursorIndex("InvalidName");
    }
}
