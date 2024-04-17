using System.ComponentModel.DataAnnotations;

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
}
