using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Attributes;

public class AvatarFileAttribute : ValidationAttribute
{
    private const int MaxSizeInBytes = 5 * 1024 * 1024; // 5MB

    private static readonly string[] AllowedMimeTypes =
    {
        "image/jpeg", "image/png", "image/gif", "image/webp"
    };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            // Проверка размера
            if (file.Length > MaxSizeInBytes)
            {
                return new ValidationResult("Avatar size must not exceed 5MB");
            }

            // Проверка MIME типа
            if (!AllowedMimeTypes.Contains(file.ContentType))
            {
                return new ValidationResult("Only JPEG, PNG, GIF, and WEBP images are allowed");
            }
        }

        return ValidationResult.Success;
    }
}