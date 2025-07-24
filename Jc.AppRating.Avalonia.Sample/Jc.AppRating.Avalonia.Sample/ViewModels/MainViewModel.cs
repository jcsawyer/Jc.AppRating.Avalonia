using System;
using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;

namespace Jc.AppRating.Avalonia.Sample.ViewModels;

public class MainViewModel : ViewModelBase
{
    public ICommand InAppRatingCommand { get; }
    public ICommand InStoreRatingCommand { get; }

    public MainViewModel()
    {
        InAppRatingCommand = ReactiveCommand.CreateFromTask(InAppRating);
        InStoreRatingCommand = ReactiveCommand.CreateFromTask(InStoreRating);
    }

    private async Task InAppRating()
    {
        await AppRating.Current.RequestInAppRatingAsync();
    }

    private async Task InStoreRating()
    {
        if (OperatingSystem.IsAndroid())
        {
            await AppRating.Current.RequestInStoreRatingAsync("com.taptapremote.android");
        }
        else if (OperatingSystem.IsIOS())
        {
            await AppRating.Current.RequestInStoreRatingAsync("com.taptapremote.ios");
        }
    }
}