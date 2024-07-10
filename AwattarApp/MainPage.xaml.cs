using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace AwattarApp
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnFetchButtonClicked(object sender, EventArgs e)
        {
            resultLabel.Text = "Fetching data...";
            var json = await FetchMarketDataAsync("https://api.awattar.at/v1/marketdata");
            resultLabel.Text = json;
        }

        private async Task<string> FetchMarketDataAsync(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetStringAsync(url);
                    var formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(response), Formatting.Indented);
                    return formattedJson;
                }
            }
            catch (Exception ex)
            {
                return $"Error fetching data: {ex.Message}";
            }
        }
    }
}
