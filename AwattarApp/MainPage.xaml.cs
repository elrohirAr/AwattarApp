using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microcharts;
using SkiaSharp;
using System;

namespace AwattarApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private  async void OnFetchButtonClicked(object sender, EventArgs e)
        {
            var json = await FetchMarketDataAsync("https://api.awattar.at/v1/marketdata");
            //string json = "{\r\n  \"object\": \"list\",\r\n  \"data\": [\r\n    {\r\n      \"start_timestamp\": 1720620000000,\r\n      \"end_timestamp\": 1720623600000,\r\n      \"marketprice\": 56.1,\r\n      \"unit\": \"Eur/MWh\"\r\n    },\r\n    {\r\n      \"start_timestamp\": 1720623600000,\r\n      \"end_timestamp\": 1720627200000,\r\n      \"marketprice\": 73.93,\r\n      \"unit\": \"Eur/MWh\"\r\n    },\r\n    {\r\n      \"start_timestamp\": 1720627200000,\r\n      \"end_timestamp\": 1720630800000,\r\n      \"marketprice\": 62.9,\r\n      \"unit\": \"Eur/MWh\"\r\n    }]}";
            var marketDataResponse = JsonConvert.DeserializeObject<MarketDataResponse>(json);
            DisplayChart(marketDataResponse!.Data);
        }

        private async Task<string> FetchMarketDataAsync(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    return await httpClient.GetStringAsync(url);
                }
            }
            catch (Exception ex)
            {
                return $"Error fetching data: {ex.Message}";
            }
        }

        private void DisplayChart(List<MarketData> data)
        {
            var entries = new List<ChartEntry>();

            foreach (var item in data)
            {
                var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(item.StartTimestamp).LocalDateTime;
                entries.Add(new ChartEntry((float)item.MarketPrice)
                {
                    Label = dateTime.ToString("HH"), // Datum und Zeit als Label für die X-Achse
                    ValueLabel = (item.MarketPrice / 10.0).ToString("F2"), // Preis als Wert für die Y-Achse
                    Color = SKColor.Parse("#00BFFF")
                });
            }

            var chart = new LineChart
            {
                Entries = entries,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Vertical,
                LineMode = LineMode.Spline,
                PointMode = PointMode.Circle,
                LineSize = 8,
                PointSize = 18,
                ValueLabelTextSize = 20,
            };
            chartView.Chart = chart;
            MarketData? highestPriceEntry = null;
            MarketData? lowestPriceEntry = null;

            if (data.Count > 0)
            {
                highestPriceEntry = data.OrderByDescending(d => d.MarketPrice).First();
                lowestPriceEntry = data.OrderBy(d => d.MarketPrice).First();
            }

            foreach (var item in data)
            {
                var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(item.StartTimestamp).LocalDateTime;
                entries.Add(new ChartEntry((float)item.MarketPrice)
                {
                    Label = dateTime.ToString("g"), // Datum und Zeit als Label für die X-Achse
                    ValueLabel = item.MarketPrice.ToString("F2"), // Preis als Wert für die Y-Achse
                    Color = SKColor.Parse("#00BFFF")
                });
            }
            // Update Labels for highest and lowest prices
            if (highestPriceEntry != null)
            {
                var highestDateTime = DateTimeOffset.FromUnixTimeMilliseconds(highestPriceEntry.StartTimestamp).LocalDateTime;
                highestPriceLabel.Text = $"Highest Price: {(highestPriceEntry.MarketPrice / 10.0).ToString("F2")} Cent/kWh at {highestDateTime:g}";
            }

            if (lowestPriceEntry != null)
            {
                var lowestDateTime = DateTimeOffset.FromUnixTimeMilliseconds(lowestPriceEntry.StartTimestamp).LocalDateTime;
                lowestPriceLabel.Text = $"Lowest Price: {lowestPriceEntry.MarketPrice / 10.0:F2} Cent/kWh at {lowestDateTime:g}";
            }
        }

    }
}
