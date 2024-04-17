﻿using System.ComponentModel.DataAnnotations;

namespace KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;

public class ConsumerMsSqlSettings : AbstractConsumerSettings
{
    public static string Position => "Kafka:Clients:Consumers:MsSql";

    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string TableJsonPropertyName { get; init; }
}
