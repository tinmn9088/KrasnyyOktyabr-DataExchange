namespace KrasnyyOktyabr.JsonTransform.Expressions.Tests;

[TestClass]
public class CastExpressionsHelperTests
{
    [TestMethod]
    public async Task ExtractTaskResultAsync_ShouldReturnTaskResult()
    {
        int taskResult = 66;
        Task task = Task.FromResult(taskResult);

        object? actual = await CastExpressionsHelper.ExtractTaskResultAsync(task);

        Assert.AreEqual(taskResult, actual);
    }

    [TestMethod]
    public async Task ExtractTaskResultAsync_WhenVoidTask_ShouldReturnTaskResult()
    {
        object? actual = await CastExpressionsHelper.ExtractTaskResultAsync(Task.CompletedTask);

        Assert.IsNotNull(actual);
    }
}
