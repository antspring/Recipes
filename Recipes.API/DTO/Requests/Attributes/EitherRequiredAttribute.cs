using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class EitherRequiredAttribute : ValidationAttribute
{
    private readonly string[] _otherProperties;

    public EitherRequiredAttribute(params string[] otherProperties)
    {
        _otherProperties = otherProperties;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var currentValue = value as string;

        if (!string.IsNullOrWhiteSpace(currentValue))
        {
            return ValidationResult.Success;
        }

        foreach (var propertyName in _otherProperties)
        {
            var property = validationContext.ObjectType.GetProperty(propertyName);
            if (property != null)
            {
                var otherValue = property.GetValue(validationContext.ObjectInstance) as string;
                if (!string.IsNullOrWhiteSpace(otherValue))
                {
                    return ValidationResult.Success;
                }
            }
        }

        return new ValidationResult(ErrorMessage ?? "Either this field or one of the related fields must be provided");
    }
}