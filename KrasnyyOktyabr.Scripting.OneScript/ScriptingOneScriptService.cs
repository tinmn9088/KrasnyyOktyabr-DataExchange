using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using KrasnyyOktyabr.Scripting.Core;
using KrasnyyOktyabr.Scripting.Core.Models;
using MyService.BusinessLogic.ScriptedWorker;
using MyService.OneScriptBridge;
using ScriptEngine.HostedScript.Library;

namespace KrasnyyOktyabr.Scripting.OneScript;

public class ScriptingOneScriptService : IScriptingService
{
    public int ClearCachedExpressions()
    {
        throw new System.NotImplementedException();
    }

    public async ValueTask RunJsonTransformAsync(Stream inputStream, Stream outputStream, CancellationToken cancellationToken)
    {
        using var outputStreamWriter = new StreamWriter(outputStream);
        using var streamReader = new StreamReader(inputStream);
        
        var host = new DefaultAppHost((message, messageStatusEnum) =>
        {
            outputStreamWriter.WriteLine(message);
        });
        
        var engine = EngineProvider.CreateEngine(host);
        engine.Initialize();

        string code = await streamReader.ReadToEndAsync();
        
        var worker = Worker.Create(engine, code);

        var success = worker.ProccessWeather();

        if (success)
            await outputStreamWriter.WriteLineAsync("УСПЕХ!");
        else 
            await outputStreamWriter.WriteLineAsync("НЕ УСПЕХ :(");

        await outputStreamWriter.FlushAsync();
    }

    public ValueTask<List<JsonTransformMsSqlResult>> RunJsonTransformOnConsumedMessageMsSqlAsync(string instructionName, string message, string tablePropertyName,
        CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }

    public ValueTask<List<string>> RunJsonTransformOnConsumedMessageVApplicationAsync(string instructionName, string message,
        CancellationToken cancellationToken)
    {
        throw new System.NotImplementedException();
    }
}