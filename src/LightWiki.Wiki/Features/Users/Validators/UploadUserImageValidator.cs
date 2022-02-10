using FluentValidation;
using LightWiki.Features.Users.Requests;

namespace LightWiki.Features.Users.Validators;

public class UploadUserImageValidator : AbstractValidator<UploadUserImage>
{
    public UploadUserImageValidator()
    {
        RuleFor(r => r.ContentType)
            .Must(ct => UploadUserImage.AcceptedFileMimeTypes.Contains(ct));
    }
}