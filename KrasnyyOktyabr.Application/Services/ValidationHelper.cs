using System.ComponentModel.DataAnnotations;
using KrasnyyOktyabr.Application.Contracts.Configuration.Kafka;
using KrasnyyOktyabr.Application.Logging;

namespace KrasnyyOktyabr.Application.Services;

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

    public static TSettings[]? GetAndValidateKafkaClientSettings<TSettings>(IConfiguration configuration, string section, ILogger logger)
    {
        TSettings[]? settings = configuration
            .GetSection(section)
            .Get<TSettings[]>();

        if (settings == null)
        {
            return null;
        }

        try
        {
            foreach (TSettings setting in settings)
            {
                if (setting == null)
                {
                    continue;
                }

                ValidateObject(setting);
            }

            return settings;
        }
        catch (ValidationException ex)
        {
            logger.InvalidConfiguration(ex, MsSqlConsumerSettings.Position);
        }

        return null;
    }
}
