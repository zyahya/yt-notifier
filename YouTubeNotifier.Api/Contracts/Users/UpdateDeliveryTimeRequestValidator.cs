namespace YouTubeNotifier.Api.Contracts.Users;

public class SetDeliveryTimeRequestValidator : AbstractValidator<UpdateDeliveryTimeRequest>
{
    public SetDeliveryTimeRequestValidator()
    {
        RuleFor(x => x.PreferredDeliveryDay)
            .IsInEnum()
            .WithMessage("{PreferredDeliveryDay} must be a valid day of the week. e.g. Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday");
    }
}
