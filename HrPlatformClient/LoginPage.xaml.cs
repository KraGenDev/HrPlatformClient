using HrPlatformClient.DTO;
using Newtonsoft.Json;

namespace HrPlatformClient;

public partial class LoginPage : ContentPage
{
    private readonly HttpRequestsController _http;

    public LoginPage(HttpRequestsController http)
    {
        InitializeComponent();
        _http = http;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string baseAddress = BaseAddressEntry.Text?.Trim() ?? "";
        string username = UsernameEntry.Text?.Trim() ?? "";
        string password = PasswordEntry.Text ?? "";

        if (string.IsNullOrEmpty(baseAddress) ||
            string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Validation", "Please fill in all fields", "OK");
            return;
        }

        _http.SetBaseAdress(baseAddress);

        try
        {
            var userObj = new
            {
                username = username,
                password = password
            };

            var response = await _http.PostAsync("/auth/token", userObj);
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", $"Request failed: {response.StatusCode}\n{json}", "OK");
                return;
            }

            var data = JsonConvert.DeserializeObject<TokenDto>(json);

            if (!string.IsNullOrWhiteSpace(data?.AccessToken))
            {
                _http.SetBearerToken(data.AccessToken);

                await Shell.Current.GoToAsync("//EmployeesPage");
            }
            else
            {
                await DisplayAlert("Error", "Failed to parse access token.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exception", ex.Message, "OK");
        }
    }
}