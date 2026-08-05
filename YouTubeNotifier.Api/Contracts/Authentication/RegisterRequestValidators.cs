namespace YouTubeNotifier.Api.Contracts.Authentication;

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

        RuleFor(x => x.PreferredDeliveryDay)
            .IsInEnum()
            .WithMessage("{PreferredDeliveryDay} must be a valid day of the week. e.g. Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday");
    }
}
