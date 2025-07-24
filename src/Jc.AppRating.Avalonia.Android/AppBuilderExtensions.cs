using Avalonia;

namespace Jc.AppRating.Avalonia.Android;

public static class AppBuilderExtensions
{
    public static AppBuilder UseAppRating(this AppBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        (AppRating.Current as InternalAppRating)!.Implementation = new AppRatingAndroid();

        return builder;
    }
}