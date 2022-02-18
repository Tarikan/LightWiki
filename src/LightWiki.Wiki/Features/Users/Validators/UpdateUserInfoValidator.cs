using System.Data;
using System.Globalization;
using System.Linq;
using FluentValidation;
using LightWiki.Features.Users.Requests;

namespace LightWiki.Features.Users.Validators;

public class UpdateUserInfoValidator : AbstractValidator<UpdateUserInfo>
{
    public UpdateUserInfoValidator()
    {
        RuleFor(r => r.Bio)
            .MaximumLength(256);

        RuleFor(r => r.Location)
            .MaximumLength(32);

        RuleFor(r => r.ContactEmail)
            .Cascade(CascadeMode.Stop)
            .MaximumLength(254)
            .EmailAddress();

        RuleFor(r => r.CountryCode)
            .Must(code =>
            {
                return CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                    .Any(c =>
                    {
                        var info = new RegionInfo(c.LCID);
                        return info.TwoLetterISORegionName == code ||
                               info.ThreeLetterISORegionName == code;
                    });
            });
    }
}