using FluentValidation;

namespace YTNotifier.Api.Contracts.Authentication;

public class LoginRequestValidators : AbstractValidator<LoginRequest>
{
    public LoginRequestValidators()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(8);
    }
}
