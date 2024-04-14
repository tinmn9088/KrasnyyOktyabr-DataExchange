using Newtonsoft.Json.Linq;

namespace KrasnyyOktyabr.JsonTransform.Tests;

[TestClass]
public class JsonHelperTests
{
    [TestMethod]

    public void RemoveEmptyProperties_InputEmpty_ShouldDoNothing()
    {
        JObject empty = new();

        JsonHelper.RemoveEmptyProperties(empty);

        Assert.IsTrue(JToken.DeepEquals(new JObject(), empty));
    }

    [TestMethod]

    public void RemoveEmptyProperties_InputWithEmptyEntries_ShouldRemoveEmptyEntries()
    {
        JObject input = new()
        {
            { "Key1", "Value1" },
            { "Key2", "  " },
            { "Key3", "Value3" },
            { "Key4", null },
            { "Key5", 0 },
            { "Key6", new JArray() },
            {
                "Key7",
                new JArray()
                {
                    "Value4",
                    new JObject()
                    {
                        {
                            "Key8",
                            null
                        }
                    }
                }
            },
            {
                "Key9",
                new JObject()
                {
                    {
                        "Key10",
                        null
                    }
                }
            }
        };
        JObject expected = new()
        {
            { "Key1", "Value1" },
            { "Key3", "Value3" },
            { "Key5", 0 },
            { "Key6", new JArray() },
            {
                "Key7",
                new JArray()
                {
                    "Value4",
                    new JObject()
                }
            },
            {
                "Key9",
                new JObject()
            }
        };

        JsonHelper.RemoveEmptyProperties(input);

        Assert.IsTrue(JToken.DeepEquals(expected, input));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Unflatten_InputNull_ShouldThrowArgumentNullException()
    {
        JsonHelper.Unflatten(null!);
    }

    [TestMethod]
    public void Unflatten_InputEmpty_ShouldReturnEmpty()
    {
        JObject empty = new();

        JObject result = JsonHelper.Unflatten(empty);

        Assert.IsTrue(JToken.DeepEquals(empty, result));
    }

    [TestMethod]
    public void Unflatten_InputFlatWithoutNestedObjects_ShouldReturnEqual()
    {
        JObject flatJsonWithoutNestedObjects = new()
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
        };

        JObject result = JsonHelper.Unflatten(flatJsonWithoutNestedObjects);

        Assert.IsTrue(JToken.DeepEquals(flatJsonWithoutNestedObjects, result));
    }

    [TestMethod]
    public void Unflatten_InputFlatWithNestedObjects_ShouldUnflatten()
    {
        JObject flatJsonWithNestedObjects = new()
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
            { "NestedObject3.Key1", "Value3" },
            { "NestedObject3.Key2", "Value4" },
            { "Key4", "Value5" },
        };
        JObject expected = new()
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
            {
                "NestedObject3",
                new JObject()
                {
                    { "Key1", "Value3" },
                    { "Key2", "Value4" },
                }
            },
            { "Key4", "Value5" },
        };

        JObject actual = JsonHelper.Unflatten(flatJsonWithNestedObjects);

        Assert.IsTrue(JToken.DeepEquals(expected, actual));
    }

    [TestMethod]
    public void Unflatten_InputFlatWithArray_ShouldUnflatten()
    {
        JObject flatJsonWithArray = new()
        {
            { "Key1", "Value1" },
            { "Key2[0]", "One" },
            { "Key2[1]", 2 },
            { "Key2[2]", "Three" },
            { "Key2[3].Key1", "Value2" },
            { "Key2[3].Key2", "Value3" },
        };
        JObject expected = new()
        {
            { "Key1", "Value1" },
            {
                "Key2",
                new JArray()
                {
                    "One",
                    2,
                    "Three",
                    new JObject()
                    {
                        { "Key1", "Value2" },
                        { "Key2", "Value3" },
                    }
                }
            },
        };

        JObject actual = JsonHelper.Unflatten(flatJsonWithArray);

        Assert.IsTrue(JToken.DeepEquals(expected, actual));
    }

    [TestMethod]
    public void Unflatten_InputFlatWithMultilevelNestedObjects_ShouldUnflatten()
    {
        JObject flatJsonWithNestedObjects = new()
        {
            { "NestedObject1.Key1", "Value1" },
            { "NestedObject1.NestedObject2.Key1", "Value2" },
            { "NestedObject1.NestedObject2.Key2", "Value3" },
            { "NestedObject1.Key3", "Value4" },
            { "Key2", "Value5" },
        };
        JObject expected = new()
        {
            {
                "NestedObject1",
                new JObject()
                {
                    { "Key1", "Value1" },
                    {
                        "NestedObject2",
                        new JObject()
                        {
                            { "Key1", "Value2" },
                            { "Key2", "Value3" },
                        }
                    },
                    { "Key3", "Value4" },
                }
            },
            { "Key2", "Value5" },
        };

        JObject actual = JsonHelper.Unflatten(flatJsonWithNestedObjects);

        Assert.IsTrue(JToken.DeepEquals(expected, actual));
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Flatten_InputNull_ShouldThrowArgumentNullException()
    {
        JsonHelper.Flatten(null!);
    }

    [TestMethod]
    public void Flatten_InputWithoutNestedObjects_ShouldReturnEqual()
    {
        JObject jsonWithoutNestedObjects = new()
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" },
        };

        JObject actual = JsonHelper.Flatten(jsonWithoutNestedObjects);

        Assert.IsTrue(JToken.DeepEquals(jsonWithoutNestedObjects, actual));
    }

    [TestMethod]
    public void Flatten_InputWithNestedObjects_ShouldFlatten()
    {
        JObject jsonWithNestedObjects = new()
        {
            { "Key1", "Value1" },
            {
                "Key2",
                new JObject()
                {
                    { "InnerKey1", "Value2" },
                    { "InnerKey2", "Value3" },
                }
            },
            { "Key3", "Value4" },
        };
        JObject expected = new()
        {
            { "Key1", "Value1" },
            { "Key2.InnerKey1", "Value2" },
            { "Key2.InnerKey2", "Value3" },
            { "Key3", "Value4" },
        };

        JObject actual = JsonHelper.Flatten(jsonWithNestedObjects);

        Assert.IsTrue(JToken.DeepEquals(expected, actual));
    }

    [TestMethod]
    public void Flatten_InputWithMultilevelNestedObjects_ShouldFlatten()
    {
        JObject jsonWithNestedObjects = new()
        {
            { "Key1", "Value1" },
            {
                "Key2",
                new JObject()
                {
                    { "InnerKey1", "Value2" },
                    {
                        "InnerKey2",
                        new JObject()
                        {
                            { "InnerKey1", "Value3" },
                        }
                    },
                }
            },
            { "Key3", "Value4" },
        };
        JObject expected = new()
        {
            { "Key1", "Value1" },
            { "Key2.InnerKey1", "Value2" },
            { "Key2.InnerKey2.InnerKey1", "Value3" },
            { "Key3", "Value4" },
        };

        JObject actual = JsonHelper.Flatten(jsonWithNestedObjects);

        Assert.IsTrue(JToken.DeepEquals(expected, actual));
    }

    [TestMethod]
    public void Flatten_InputWithArray_ShouldFlatten()
    {
        JObject jsonWithArray = new()
        {
            { "Key1", "Value1" },
            {
                "Key2",
                new JArray()
                {
                    "One",
                    2,
                    "Three",
                    new JObject()
                    {
                        { "Key1", "Value2" },
                        { "Key2", "Value3" },
                    }
                }
            },
        };
        JObject expected = new()
        {
            { "Key1", "Value1" },
            { "Key2[0]", "One" },
            { "Key2[1]", 2 },
            { "Key2[2]", "Three" },
            { "Key2[3].Key1", "Value2" },
            { "Key2[3].Key2", "Value3" },
        };

        JObject actual = JsonHelper.Flatten(jsonWithArray);

        Assert.IsTrue(JToken.DeepEquals(expected, actual));
    }

    [TestMethod]
    public void FlattenArrayTest_InputArray_ShouldFlatten()
    {
        JArray array = ["One", 2, "Three"];
        JObject expected = new()
        {
            { "[0]", "One" },
            { "[1]", 2 },
            { "[2]", "Three" },
        };

        JObject actual = JsonHelper.FlattenArray(array);

        Assert.IsTrue(JToken.DeepEquals(expected, actual));
    }
}
