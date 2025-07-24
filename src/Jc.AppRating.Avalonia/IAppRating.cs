namespace Jc.AppRating.Avalonia;

public interface IAppRating
{
    event EventHandler<AppRatingError>? OnError;
    
    Task<bool> RequestInAppRatingAsync();
    Task<bool> RequestInStoreRatingAsync(string appId);
}