using FluentValidation;

namespace BaseTemplate.Application.Settings.Commands.UpdateBrandingSettings;

public sealed class UpdateBrandingSettingsCommandValidator : AbstractValidator<UpdateBrandingSettingsCommand>
{
    private const string HexColorPattern = "^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$";

    public UpdateBrandingSettingsCommandValidator()
    {
        RuleFor(x => x.AppName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.LogoUrl)
            .MaximumLength(2048);

        RuleFor(x => x.PrimaryColor)
            .NotEmpty()
            .Matches(HexColorPattern)
            .WithMessage("Primary color must be a valid hex color, e.g. #4F46E5.");

        RuleFor(x => x.SecondaryColor)
            .NotEmpty()
            .Matches(HexColorPattern)
            .WithMessage("Secondary color must be a valid hex color, e.g. #F59E0B.");
    }
}
