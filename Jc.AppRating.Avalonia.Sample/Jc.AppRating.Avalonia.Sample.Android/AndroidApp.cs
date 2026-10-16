using System;
using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;
using Jc.AppRating.Avalonia.Android;
using ReactiveUI.Avalonia;

namespace Jc.AppRating.Avalonia.Sample.Android;

[Application]
public class AndroidApp : AvaloniaAndroidApplication<App>
{
    public AndroidApp(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .UseAppRating()
            .WithInterFont()
            .UseReactiveUI(_ => { });
    }
}
