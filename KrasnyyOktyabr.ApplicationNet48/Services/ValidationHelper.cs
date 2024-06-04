using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KrasnyyOktyabr.ApplicationNet48.Services;

public static class ValidationHelper
{
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
    public static TSettings[]? GetAndValidateKafkaClientSettings<TSettings>(IConfiguration configuration, string section, ILogger logger)
    {
        TSettings[]? settings = configuration
            .GetSection(section)
            .Get<TSettings[]>();

        if (settings is null)
        {
            return null;
        }

        try
        {
            foreach (TSettings setting in settings)
            {
                if (setting is null)
                {
                    continue;
                }

                ValidateObject(setting);
            }

            return settings;
        }
        catch (ValidationException ex)
        {
            logger.LogError(ex, "Invalid configuration at '{Position}'",  section);
        }

        return null;
    }
#nullable disable
}
