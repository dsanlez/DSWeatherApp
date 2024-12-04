using DSWeatherApp.Models;

namespace DSWeatherApp.Pages;

public partial class CurrentPage : ContentPage
{
    
    public CurrentPage()
	{
		InitializeComponent();
        BindingContext = new WeatherPageViewModel();

        // Subscribe to language change notifications
        MessagingCenter.Subscribe<SettingsPage>(this, "LanguageChanged", (sender) =>
        {
            RefreshPage();
        });
    }

    private void RefreshPage()
    {
        InitializeComponent();
    }

}