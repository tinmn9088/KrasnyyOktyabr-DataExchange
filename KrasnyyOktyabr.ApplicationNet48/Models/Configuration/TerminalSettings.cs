using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;

public class TerminalSettings
{
    public static string Position => "Terminal";

    [Required]
    public bool Enabled { get; set; }

    public int? EncodingCodePage { get; set; }
}
