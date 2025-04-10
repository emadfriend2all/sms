using ShowMatic.Server.Domain.Catalog;

namespace ShowMatic.Server.Application.Catalog.Smss.Create;

public class SendSmsRequestValidator : CustomValidator<SendSmsCommand>
{
    public SendSmsRequestValidator(IStringLocalizer<SendSmsRequestValidator> localizer)
    {
        RuleFor(p => p.PhoneNumber).NotEmpty().WithMessage("Sorry the Phone number is required to send the sms");
        RuleFor(p => p.Message).NotEmpty().Length(0, 200).WithMessage("The message length is not correct the min is 0 and max is 200");
    }
}