using DSWeatherApp.Models;

namespace DSWeatherApp.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        BindingContext = new WeatherPageViewModel();
        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


        await ((WeatherPageViewModel)BindingContext).GetLocation();
    }
}