using DSWeatherApp.Models;

namespace DSWeatherApp.Pages;

public partial class CurrentPage : ContentPage
{
    
    public CurrentPage()
	{
		InitializeComponent();
        BindingContext = new WeatherPageViewModel();
    }
}