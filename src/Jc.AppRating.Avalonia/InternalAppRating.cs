namespace Jc.AppRating.Avalonia;

internal sealed class InternalAppRating : IAppRating
{
    internal IAppRating? Implementation { get; set; }

    public event EventHandler<AppRatingError>? OnError;

    public Task<bool> RequestInAppRatingAsync()
        => Implementation?.RequestInAppRatingAsync() ?? Task.FromResult(false);

    public Task<bool> RequestInStoreRatingAsync(string appId)
        => Implementation?.RequestInStoreRatingAsync(appId) ?? Task.FromResult(false);
}