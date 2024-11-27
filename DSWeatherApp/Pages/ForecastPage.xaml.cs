using DSWeatherApp.Models;

namespace DSWeatherApp.Pages;

public partial class ForecastPage : ContentPage
{
	public ForecastPage()
	{
		InitializeComponent();
        BindingContext = new WeatherPageViewModel();
    }
}