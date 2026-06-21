using FluentValidation;

namespace YTNotifier.Api.Contracts.Authentication;

public class RegisterRequestValidators : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidators()
    {
        RuleFor(x => x.FirstName)
            .Length(2, 50);

        RuleFor(x => x.LastName)
            .Length(2, 50);

        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(8);
    }
}
