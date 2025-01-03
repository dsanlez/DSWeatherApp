﻿using DSWeatherApp.Pages;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Input;


namespace DSWeatherApp.Models
{
    public class WeatherPageViewModel : INotifyPropertyChanged
    {

        private readonly string _apiKey = "APIKEY";
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://api.openweathermap.org";

        public event PropertyChangedEventHandler? PropertyChanged;

        private CurrentWeatherResponse? _currentWeather;

        private ObservableCollection<HourlyWeather> _hourlyForecast = new();

        public ObservableCollection<DailyWeather> DailyForecast { get; } = new();

        public ObservableCollection<City> Cities { get; } = new();

        public ICommand NavigateToDetailsCommand { get; }
        public ICommand NavigateToDailyForecastDetailsCommand { get; }
        public ICommand NavigateToHourlyDetailsCommand { get; }

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

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

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

            NavigateToHourlyDetailsCommand = new Command<HourlyWeather>(async (hourlyItem) =>
            {
                if (hourlyItem != null)
                {
                    var detailsPage = new HourlyForecastDetailsPage
                    {
                        BindingContext = hourlyItem
                    };
                    await Shell.Current.Navigation.PushAsync(detailsPage);
                }
            });

            NavigateToDailyForecastDetailsCommand = new Command<DailyWeather>(async (dailyForecastItem) =>
            {
                if (dailyForecastItem != null)
                {
                    var forecastDetailsPage = new ForecastDetailsPage
                    {
                        BindingContext = dailyForecastItem
                    };
                    await Shell.Current.Navigation.PushAsync(forecastDetailsPage);
                }
            });
        }

        /// <summary>
        /// Searches for cities based on the provided query and updates the Cities collection.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
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

        /// <summary>
        /// /// Retrieves current, hourly, and daily weather data for a given geographical location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public async Task SearchWeather(double latitude, double longitude)
        {
            if (latitude == 0 && longitude == 0)
                return;

            IsLoading = true;

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
                                    : string.Empty,
                                Main = item.Main,
                                Wind = item.Wind,
                                Rain = item.Rain,
                                CityName = SelectedCity.Name
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
                            Icon = $"https://openweathermap.org/img/wn/{group.First().Weather.First().icon}.png",
                            Main = group.First().Main,
                            Wind = group.First().Wind,
                            Rain = group.First().Rain,
                            CityName = SelectedCity.Name
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
            finally
            {
                IsLoading = false;
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

        /// <summary>
        /// Retrieves the current geographical location of the device and uses it to search for weather information.
        /// </summary>
        /// <returns></returns>
        public async Task GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    var latitude = location.Latitude;
                    var longitude = location.Longitude;

                    await SearchWeather(latitude, longitude);
                }
                else
                {
                    Debug.WriteLine("Location not found.");
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {

                Debug.WriteLine(fnsEx.Message);
            }
            catch (PermissionException pEx)
            {
                Debug.WriteLine(pEx.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
