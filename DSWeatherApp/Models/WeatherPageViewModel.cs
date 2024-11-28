using DSWeatherApp.Pages;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace DSWeatherApp.Models
{
    public class WeatherPageViewModel : INotifyPropertyChanged
    {

        private readonly string _apiKey = "5dd97a2dfb0b92a3fc8562f67935bea0";
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://api.openweathermap.org";

        public event PropertyChangedEventHandler? PropertyChanged;

        private CurrentWeatherResponse? _currentWeather;

        private ObservableCollection<HourlyWeather> _hourlyForecast = new();

        public ObservableCollection<DailyWeather> DailyForecast { get; } = new();

        public ObservableCollection<City> Cities { get; } = new();

        public ICommand NavigateToDetailsCommand { get; }

        private string? _searchQuery;

        public CurrentWeatherResponse? CurrentWeather
        {
            get => _currentWeather;
            set => SetProperty(ref _currentWeather, value);
        }

        public ObservableCollection<HourlyWeather> HourlyForecast
        {
            get => _hourlyForecast;
            set => SetProperty(ref _hourlyForecast, value);
        }
        public ICommand ClearCitiesCommand => new Command(() =>
        {
            Cities.Clear();
        });



        private City? _selectedCity;
        public City? SelectedCity
        {
            get => _selectedCity;
            set
            {
                if (SetProperty(ref _selectedCity, value))
                {
                    _ = Task.Run(async () =>
                    {
                        if (_selectedCity != null)
                        {
                            await SearchWeather(_selectedCity.Lat, _selectedCity.Lon);
                        }
                    });
                }
            }
        }
        public string? SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    _ = Task.Run(async () =>
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            await SearchCitiesAsync(value);
                        }
                        else
                        {
                            Cities.Clear();
                        }
                    });
                }
            }
        }

        public WeatherPageViewModel()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseApiUrl)
            };

            NavigateToDetailsCommand = new Command(async () =>
            {
                if (CurrentWeather != null)
                {
                    var detailsPage = new CurrentWeatherDetailsPage
                    {
                        BindingContext = new { CurrentWeather = CurrentWeather }
                    };
                    await Shell.Current.Navigation.PushAsync(detailsPage);
                }
            });
        }

        private async Task SearchCitiesAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                Cities.Clear();
                return;
            }

            try
            {
                var response = await _httpClient.GetStringAsync(
                    $"/geo/1.0/direct?q={query}&limit=5&appid={_apiKey}");

                var cities = JsonConvert.DeserializeObject<List<City>>(response);

                Cities.Clear();
                if (cities?.Any() == true)
                {
                    cities.ForEach(city => Cities.Add(city));
                }
            }
            catch (HttpRequestException ex)
            {

                Debug.WriteLine($"City search error: {ex.Message}");
            }
        }

        public async Task SearchWeather(double latitude, double longitude)
        {
            if (latitude == 0 && longitude == 0)
                return;

            try
            {
                var currentWeatherTask = _httpClient.GetStringAsync(
                    $"/data/2.5/weather?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric");

                var hourlyWeatherTask = _httpClient.GetStringAsync(
                    $"/data/2.5/forecast?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric");

                var dailyForecastTask = _httpClient.GetStringAsync(
                $"/data/2.5/forecast?lat={latitude}&lon={longitude}&appid={_apiKey}&units=metric");


                await Task.WhenAll(currentWeatherTask, hourlyWeatherTask, dailyForecastTask);
                
                #region Current Weather
                CurrentWeather = JsonConvert.DeserializeObject<CurrentWeatherResponse>(
                    await currentWeatherTask);

                if (CurrentWeather != null)
                {
                    CurrentWeather.CityName = SelectedCity.Name;
                    OnPropertyChanged(nameof(CurrentWeather));
                }

                #endregion

                #region Hourly Forecast
                var hourlyWeather = JsonConvert.DeserializeObject<HourlyWeatherResponse>(
                await hourlyWeatherTask);

                HourlyForecast.Clear();
                hourlyWeather?.List?
                    .Take(8)
                    .ToList()
                    .ForEach(item =>
                    {
                        if (item?.Weather?.FirstOrDefault() != null)
                        {
                            HourlyForecast.Add(new HourlyWeather
                            {
                                Hour = item.DtTxt?.Substring(11, 5) ?? string.Empty,
                                Icon = item.Weather != null && item.Weather.Any()
                                    ? $"https://openweathermap.org/img/wn/{item.Weather[0].icon}.png"
                                    : string.Empty,
                                Temperature = item.Main != null
                                    ? $"{Math.Round(item.Main.Temp)}°C"
                                    : string.Empty
                            });
                        }
                    });
                #endregion

                #region Daily Forecast
                var forecastData = JsonConvert.DeserializeObject<HourlyWeatherResponse>(
                await dailyForecastTask);

                DailyForecast.Clear();

                if (forecastData?.List != null)
                {
                    var dailyGrouped = forecastData.List
                        .GroupBy(item => item.DtTxt?.Substring(0, 10))
                        .Select(group => new DailyWeather
                        {
                            Date = DateTime.Parse(group.Key).ToString("dd/MM"),
                            Temperature = $"{Math.Round(group.Average(item => item.Main.Temp))}°C",
                            Icon = $"https://openweathermap.org/img/wn/{group.First().Weather.First().icon}.png"
                        })
                        .ToList();

                    foreach (var day in dailyGrouped)
                    {
                        DailyForecast.Add(day);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Weather data error: {ex.Message}");
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

       

    }
}
