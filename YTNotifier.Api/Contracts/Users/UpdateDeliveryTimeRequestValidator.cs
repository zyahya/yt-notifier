namespace YTNotifier.Api.Contracts.Users;

public class SetDeliveryTimeRequestValidator : AbstractValidator<UpdateDeliveryTimeRequest>
{
    public SetDeliveryTimeRequestValidator()
    {
        RuleFor(x => x.PreferredDeliveryDay)
            .ExclusiveBetween(0, 6);

        RuleFor(x => x.PreferredDeliveryHour)
            .ExclusiveBetween(0, 23);
    }
}
