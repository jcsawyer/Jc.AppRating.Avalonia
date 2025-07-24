using Android.Content;
using Android.Content.PM;
using Android.OS;
using Java.Lang;
using Microsoft.Maui.ApplicationModel;
using Xamarin.Google.Android.Play.Core.Review;
using Xamarin.Google.Android.Play.Core.Review.Testing;
using Exception = System.Exception;
using Task = Android.Gms.Tasks.Task;

namespace Jc.AppRating.Avalonia.Android;

public class AppRatingAndroid : Java.Lang.Object, IAppRating, global::Android.Gms.Tasks.IOnCompleteListener
{
    public event EventHandler<AppRatingError>? OnError;

    private bool _hasLaunchedReviewFlow;
    private TaskCompletionSource<bool>? _tcs;
    private IReviewManager? _reviewManager;
    private Task? _reviewFlowTask;

    public async Task<bool> RequestInAppRatingAsync()
    {
        _tcs?.TrySetCanceled();
        _tcs = new TaskCompletionSource<bool>();
        Task? request = null;

        try
        {
            _reviewManager = ReviewManagerFactory.Create(Application.Context);
            _hasLaunchedReviewFlow = false;
            request = _reviewManager.RequestReviewFlow();
            request.AddOnCompleteListener(this);

            return await _tcs.Task;
        }
        catch
        {
            return false;
        }
        finally
        {
            _reviewManager?.Dispose();
            request?.Dispose();
        }
    }

    public Task<bool> RequestInStoreRatingAsync(string appId)
    {
        _tcs?.TrySetCanceled();
        _tcs = new TaskCompletionSource<bool>();

        if (string.IsNullOrWhiteSpace(appId))
        {
            OnError?.Invoke(this, new AppRatingError("Play Store appId cannot be null or empty."));
            _tcs.TrySetResult(false);
            return _tcs.Task;
        }

        var url = $"market://details?id={appId}";

        try
        {
            var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(url));
            intent.AddFlags(ActivityFlags.NoHistory);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
                intent.AddFlags(ActivityFlags.NewDocument);
            else
                intent.AddFlags(ActivityFlags.ClearWhenTaskReset);
            intent.AddFlags(ActivityFlags.ClearTop);
            intent.AddFlags(ActivityFlags.NewTask | ActivityFlags.ResetTaskIfNeeded);

            Platform.AppContext.StartActivity(intent);

            _tcs.SetResult(true);
        }
        catch (PackageManager.NameNotFoundException)
        {
            OnError?.Invoke(this, new AppRatingError("Google Play Store is not installed on this device."));
            _tcs.SetResult(false);
        }
        catch (ActivityNotFoundException)
        {
            var playStoreUrl = $"https://play.google.com/store/apps/details?id={appId}";
            var intent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(playStoreUrl));
            intent.AddFlags(ActivityFlags.NewTask);
            Platform.AppContext.StartActivity(intent);

            _tcs.SetResult(true);
        }

        return _tcs.Task;
    }

    public void OnComplete(Task task)
    {
        if (!task.IsSuccessful || _hasLaunchedReviewFlow)
        {
            _tcs?.TrySetResult(_hasLaunchedReviewFlow);
            _reviewFlowTask?.Dispose();

            return;
        }

        try
        {
            var reviewInfo = (ReviewInfo)task.GetResult(Class.FromType(typeof(ReviewInfo)));
            _hasLaunchedReviewFlow = true;
            _reviewFlowTask = _reviewManager?.LaunchReviewFlow(Platform.CurrentActivity, reviewInfo);
            _reviewFlowTask?.AddOnCompleteListener(this);
        }
        catch (Exception e)
        {
            OnError?.Invoke(this, new AppRatingError(e.Message));
            _tcs?.TrySetResult(false);
        }
    }
}