#nullable enable

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using KrasnyyOktyabr.ApplicationNet48.Filters;
using KrasnyyOktyabr.ApplicationNet48.Logging;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using static KrasnyyOktyabr.ApplicationNet48.Controllers.ControllersHelper;

namespace KrasnyyOktyabr.ApplicationNet48.Controllers;

[EnabledInSettings(settingsType: typeof(IOptions<TerminalSettings>), propertyName: "Value.Enabled")]
public class TerminalController(ILogger<TerminalController> logger, IOptions<TerminalSettings> settings) : ApiController
{
    private readonly TerminalSettings _settings = settings.Value;

    [HttpPost]
    public async Task<IHttpActionResult> Execute(HttpRequestMessage request)
    {
        try
        {
            string command = GetRequiredQueryParameter(request, "command");

            logger.LogInformation("Executing command '{Command}' ...", command);

            Encoding encoding = _settings.EncodingCodePage is not null ? Encoding.GetEncoding(_settings.EncodingCodePage.Value) : Encoding.Default;

            ProcessStartInfo processStartInfo = new()
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                StandardOutputEncoding = encoding,
                RedirectStandardError = true,
                StandardErrorEncoding = encoding,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new() { StartInfo = processStartInfo })
            {
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();

                process.WaitForExit();

                logger.LogInformation("Command '{Command}' executed successfully", command);

                return base.ResponseMessage(new HttpResponseMessage() { Content = new StringContent(string.IsNullOrWhiteSpace(output) ? error : output) });
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogOperationCancelled();

            return BadRequest();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
