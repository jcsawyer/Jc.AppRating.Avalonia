namespace Jc.AppRating.Avalonia;

public static class AppRating
{
    private static Lazy<IAppRating> Implementation = new(() => new InternalAppRating(),
        LazyThreadSafetyMode.PublicationOnly);

    public static IAppRating Current => Implementation.Value;
}