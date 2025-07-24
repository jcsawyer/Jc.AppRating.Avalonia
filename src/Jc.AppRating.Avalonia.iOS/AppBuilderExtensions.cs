using Avalonia;

namespace Jc.AppRating.Avalonia.iOS;

public static class AppBuilderExtensions
{
    public static AppBuilder UseAppRating(this AppBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        (AppRating.Current as InternalAppRating)!.Implementation = new AppRatingiOS();

        return builder;
    }
}