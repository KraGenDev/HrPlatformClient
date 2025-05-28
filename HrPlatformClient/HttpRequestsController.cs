using System.Text;
using Newtonsoft.Json;

namespace HrPlatformClient
{
    public class HttpRequestsController
    {
        private readonly HttpClient _httpClient = new HttpClient();


        public HttpRequestsController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://63d4-176-111-185-180.ngrok-free.app/"); 
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            try 
            {
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }

                return default;
            }
            catch (Exception ex)
            {
                // TODO: логування
                return default;
            }
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, content);

                return response;
            }
            catch (Exception ex)
            {
                // TODO: логування або повторна передача помилки
                throw;
            }
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(url, content);
                if (!response.IsSuccessStatusCode)
                    return default;

                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(resultJson);
            }
            catch
            {
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string url)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(url);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        public void SetBaseAdress(string adress)
        {
            _httpClient.BaseAddress = new Uri(adress);
        }
    }
}
