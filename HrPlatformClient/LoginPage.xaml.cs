using HrPlatformClient.DTO;
using Newtonsoft.Json;

namespace HrPlatformClient;

public partial class LoginPage : ContentPage
{
    private readonly HttpRequestsController _http;

    private bool _isBusy = false;

    public LoginPage(HttpRequestsController http)
    {
        InitializeComponent();
        _http = http;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (_isBusy) return;
        _isBusy = true;

        try
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

            _http.SetBaseAddress(baseAddress);

            var userObj = new
            {
                username = username,
                password = password
            };

            var response = await _http.PostAsync("/auth/token", userObj);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Error", $"Request failed: {response.StatusCode}", "OK");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
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
        finally
        {
            _isBusy = false;
        }
    }

}