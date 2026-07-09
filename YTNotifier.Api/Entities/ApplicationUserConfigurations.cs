using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YTNotifier.Api.Entities;

public class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder
            .HasMany(x => x.Channels)
            .WithMany(x => x.Users)
            .UsingEntity<Subscription>(
                r => r
                    .HasOne(x => x.Channel)
                    .WithMany(x => x.Subscriptions)
                    .HasForeignKey(x => x.ChannelId),
                l => l
                    .HasOne(x => x.User)
                    .WithMany(x => x.Subscriptions)
                    .HasForeignKey(x => x.UserId),
                j => j.HasKey(x => new { x.UserId, x.ChannelId })
            );
    }
}
