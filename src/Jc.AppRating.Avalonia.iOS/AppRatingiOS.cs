using StoreKit;

namespace Jc.AppRating.Avalonia.iOS;

internal sealed class AppRatingiOS : IAppRating
{
    public event EventHandler<AppRatingError>? OnError;

    public Task<bool> RequestInAppRatingAsync()
    {
        var tcs = new TaskCompletionSource<bool>();
        if (!UIDevice.CurrentDevice.CheckSystemVersion(10, 3))
        {
            OnError?.Invoke(this, new AppRatingError("In-App Rating is not supported on this iOS version."));
            tcs.SetResult(false);
            return tcs.Task;
        }

        if (!UIDevice.CurrentDevice.CheckSystemVersion(14, 0))
        {
            SKStoreReviewController.RequestReview();
            tcs.SetResult(true);
        }
        else
        {
            if (UIApplication.SharedApplication?.ConnectedScenes?.ToArray().FirstOrDefault(scene =>
                    scene.ActivationState == UISceneActivationState.ForegroundActive) is UIWindowScene windowScene)
            {
                SKStoreReviewController.RequestReview(windowScene);
                tcs.SetResult(true);
            }
        }

        return tcs.Task;
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
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UIApplication.SharedApplication.OpenUrlAsync(url, new UIApplicationOpenUrlOptions());
            }
            else
            {
                UIApplication.SharedApplication.OpenUrl(url);
            }

            tcs.SetResult(true);
        }
        catch (Exception)
        {
            OnError?.Invoke(this, new AppRatingError("Unable to launch App Store."));
            tcs.SetResult(false);
        }

        return tcs.Task;
    }
}