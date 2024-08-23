using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using KrasnyyOktyabr.ApplicationNet48.Models.Configuration.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Common.Helpers;

public static class ValidationHelper
{
    /// <exception cref="ValidationException"></exception>
    public static void ValidateObject(object objectToValidate)
    {
        List<ValidationResult> validationResults = [];

        if (!Validator.TryValidateObject(objectToValidate, new ValidationContext(objectToValidate), validationResults, validateAllProperties: true))
        {
            throw new ValidationException(
                validationResult: validationResults[0],
                validatingAttribute: null,
                value: objectToValidate);
        }
    }

#nullable enable

    /// <remarks>
    /// Validates only outer properties, nested objects are ignored.
    /// </remarks>
    public static TSettings[]? GetAndValidateKafkaClientSettings<TSettings>(IConfiguration configuration, string section, ILogger logger)
    {
        TSettings[]? clientsSettings = configuration
            .GetSection(section)
            .Get<TSettings[]>();

        if (clientsSettings is null || clientsSettings.Length == 0)
        {
            return null;
        }

        try
        {
            foreach (TSettings clientSettings in clientsSettings)
            {
                if (clientSettings is null)
                {
                    continue;
                }

                ValidateObject(clientSettings);
            }

            return clientsSettings;
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Invalid configuration at '{Position}'",  section);
        }

        return null;
    }

    /// <summary>
    /// Runs <see cref="GetAndValidateKafkaClientSettings"/> and then validates
    /// <see cref="V77ApplicationProducerSettings.ObjectFilters"/> or
    /// <see cref="V83ApplicationProducerSettings.ObjectFilters"/>.
    /// </summary>
    public static TSettings[]? GetAndValidateVApplicationKafkaProducerSettings<TSettings>(IConfiguration configuration, string section, ILogger logger) where TSettings : AbstractVApplicationProducerSettings
    {
        TSettings[]? clientsSettings = GetAndValidateKafkaClientSettings<TSettings>(configuration, section, logger);

        if (clientsSettings is null || clientsSettings.Length == 0)
        {
            return null;
        }

        try
        {
            foreach (TSettings clientSettings in clientsSettings)
            {
                // V77ApplicationProducerSettings
                if (clientsSettings is V77ApplicationProducerSettings v77ApplicationProducerSettings)
                {
                    foreach (V77ApplicationObjectFilter objectFilter in v77ApplicationProducerSettings.ObjectFilters)
                    {
                        ValidateObject(objectFilter);
                    }
                }

                // V83ApplicationProducerSettings
                if (clientsSettings is V83ApplicationProducerSettings v83ApplicationProducerSettings)
                {
                    foreach (V83ApplicationObjectFilter objectFilter in v83ApplicationProducerSettings.ObjectFilters)
                    {
                        ValidateObject(objectFilter);
                    }
                }
            }

            return clientsSettings;
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Invalid configuration at '{Position}'", section);
        }

        return null;
    }
}
