using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace KrasnyyOktyabr.ApplicationNet48.Models;
public class V77ApplicationResolverRequest
{
    [Required]
    [JsonProperty("infobasePath")]
    public string InfobasePath { get; set; }

    [Required]
    [JsonProperty("username")]
    public string Username { get; set; }

    [Required]
    [JsonProperty("password")]
    public string Password { get; set; }

    [Required]
    [JsonProperty("ertName")]
    public string ErtName { get; set; }

    [Required]
    [JsonProperty("resultName")]
    public string ResultName { get; set; }

#nullable enable
    [JsonProperty("formParams")]
    public Dictionary<string, string>? FormParams { get; set; }
}
