namespace YTNotifier.Api.Contracts.Users;

public class SetDeliveryTimeRequestValidator : AbstractValidator<UpdateDeliveryTimeRequest>
{
    public SetDeliveryTimeRequestValidator()
    {
        RuleFor(x => x.DeliveryDay)
            .ExclusiveBetween(0, 6);

        RuleFor(x => x.DeliveryHour)
            .ExclusiveBetween(0, 23);
    }
}
