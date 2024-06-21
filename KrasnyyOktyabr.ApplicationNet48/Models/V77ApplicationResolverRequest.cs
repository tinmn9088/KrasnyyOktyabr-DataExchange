using System.Collections.Generic;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models;
public class V77ApplicationResolverRequest
{
    [JsonProperty("infobasePath")]
    public string InfobasePath { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("ertName")]
    public string ErtName { get; set; }

#nullable enable
    [JsonProperty("formParams")]
    public Dictionary<string, string>? FormParams { get; set; }

    [JsonProperty("resultName")]
    public string? ResultName { get; set; }
}
