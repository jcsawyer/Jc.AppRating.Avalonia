using StoreKit;

namespace Jc.AppRating.Avalonia.iOS;

internal sealed class AppRatingiOS : IAppRating
{
    public event EventHandler<AppRatingError>? OnError;

    public Task<bool> RequestInAppRatingAsync()
    {
        if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
        {
            OnError?.Invoke(this, new AppRatingError("In-App Rating is not supported on this iOS version."));
            return Task.FromResult(false);
        }

        if (!UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
        {
            SKStoreReviewController.RequestReview();
            return Task.FromResult(true);
        }
        
        if (UIApplication.SharedApplication?.ConnectedScenes?.ToArray().FirstOrDefault(scene =>
                scene.ActivationState == UISceneActivationState.ForegroundActive) is UIWindowScene windowScene)
        {
            // TODO - Uncomment when iOS 18 API is stable
            // if (UIDevice.CurrentDevice.CheckSystemVersion(18, 0))
            // {
            //     AppStore.RequestReview(windowScene);
            // }
            
            SKStoreReviewController.RequestReview(windowScene);
            return Task.FromResult(true);
        }
        
        return Task.FromResult(false);
    }

    public Task<bool> RequestInStoreRatingAsync(string appId)
    {
        var tcs = new TaskCompletionSource<bool>();

        if (string.IsNullOrWhiteSpace(appId))
        {
            OnError?.Invoke(this, new AppRatingError("App Store appId cannot be null or empty."));
            tcs.SetResult(false);
            return tcs.Task;
        }

        var url = new NSUrl($"itms-apps://apps.apple.com/app/id{appId}?action=write-review");

        try
        {
            UIApplication.SharedApplication.OpenUrl(url, new UIApplicationOpenUrlOptions(), success =>
            {
                tcs.SetResult(success);
            });
        }
        catch (Exception)
        {
            OnError?.Invoke(this, new AppRatingError("Unable to launch App Store."));
            tcs.SetResult(false);
        }

        return tcs.Task;
    }
}