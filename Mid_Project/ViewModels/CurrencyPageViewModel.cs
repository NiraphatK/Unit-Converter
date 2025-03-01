using CommunityToolkit.Mvvm.ComponentModel;
using Mid_Project.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mid_Project.ViewModels
{
    public partial class CurrencyPageViewModel : ObservableObject
    {
        // ObservableCollection สำหรับเก็บรายการสกุลเงิน
        public ObservableCollection<Currency> Currencies { get; set; }

        // Property สำหรับเก็บสกุลเงินที่ผู้ใช้เลือก (From)
        [ObservableProperty]
        public Currency selectedFromCurrency;

        // Property สำหรับเก็บสกุลเงินที่ผู้ใช้เลือก (To)
        [ObservableProperty]
        public Currency selectedToCurrency;

        private readonly ConversionService _conversionService;

        private DateTime lastUpdated;
        public DateTime LastUpdated
        {
            get => lastUpdated;
            set => SetProperty(ref lastUpdated, value);  // Ensure this is updated properly
        }

        public CurrencyPageViewModel()
        {
            _conversionService = new ConversionService();
            Currencies = new ObservableCollection<Currency>();
        }

        // ฟังก์ชันดึงข้อมูลสกุลเงินจาก API
        [ObservableProperty]
        private bool isLoading = false;

        public async Task FetchCurrencyDataAsync()
        {
            IsLoading = true;  // Show the ActivityIndicator

            using (var client = new HttpClient())
            {
                string apiKey = "your_api_key_here";
                string apiUrl = $"https://v6.exchangerate-api.com/v6/{apiKey}/latest/USD";
                try
                {
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var jsonResponse = await response.Content.ReadAsStringAsync();
                        var apiData = JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);

                        Currencies.Clear();
                        foreach (var rate in apiData.ConversionRates)
                        {
                            Currencies.Add(new Currency()
                            {
                                CurrencyName = rate.Key,
                                ConversionRate = rate.Value
                            });
                        }

                        // Set the last updated time when the data is successfully fetched
                        LastUpdated = DateTimeOffset.FromUnixTimeSeconds(apiData.TimeLastUpdateUnix).DateTime;

                    }
                    else
                    {
                        Console.WriteLine("Error fetching data from API");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                finally
                {
                    IsLoading = false;  // Hide the ActivityIndicator
                }
            }
        }

        // ฟังก์ชันตั้งค่า default สำหรับสกุลเงิน
        private void SetDefaultCurrencies()
        {
            if (Currencies.Count > 0)
            {
                // Set default currency values (First and Second currencies)
                SelectedFromCurrency = Currencies.FirstOrDefault();
                SelectedToCurrency = Currencies.ElementAtOrDefault(1);  // Can be changed as needed
            }
        }

        // ฟังก์ชันสำหรับแปลงสกุลเงิน
        public double ConvertCurrency(double inputValue)
        {
            if (selectedFromCurrency != null && selectedToCurrency != null)
            {
                double fromRate = selectedFromCurrency.ConversionRate;
                double toRate = selectedToCurrency.ConversionRate;

                // คำนวณการแปลงจากสกุลเงิน (from -> to)
                return inputValue * (toRate / fromRate);
            }
            return 0;
        }

        // ฟังก์ชันสำหรับบันทึกประวัติการแปลง
        public void SaveConversionHistory(double inputValue, string fromCurrency, string toCurrency, double result)
        {
            var history = new ConversionHistory
            {
                InputValue = inputValue,
                FromUnit = fromCurrency,
                ToUnit = toCurrency,
                ResultValue = Math.Round(result, 2),
                Timestamp = DateTime.Now
            };

            _conversionService.SaveHistoryToFile(history);
        }
    }

    // ค่าที่จะได้รับจาก API
    public class ApiResponse
    {
        [JsonProperty("conversion_rates")]
        public Dictionary<string, double> ConversionRates { get; set; }

        [JsonProperty("time_last_update_unix")]
        public long TimeLastUpdateUnix { get; set; }
    }
}