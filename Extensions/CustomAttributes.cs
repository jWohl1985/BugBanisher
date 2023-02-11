using System.ComponentModel.DataAnnotations;

namespace BugBanisher.Extensions;

public class MaxFileSizeAttribute : ValidationAttribute
{
    private readonly int _maxFileSize;

    public MaxFileSizeAttribute(int maxFileSize)
    {
        _maxFileSize = maxFileSize;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        IFormFile? file = value as IFormFile;

        if (file is not null && file.Length > _maxFileSize)
            return new ValidationResult($"Maximum allowed file size is {_maxFileSize} bytes.");

        return ValidationResult.Success!;
    }
}

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtensions;

    public AllowedExtensionsAttribute(string[] allowedExtensions)
    {
        _allowedExtensions = allowedExtensions;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        IFormFile? file = value as IFormFile;

        if (file is null || _allowedExtensions.Contains(Path.GetExtension(file.FileName).ToLower()))
            return ValidationResult.Success!;

        return new ValidationResult($"The file extension type is not allowed.");
    }
}
