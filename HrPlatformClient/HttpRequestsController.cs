using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace HrPlatformClient
{
    public class HttpRequestsController
    {
        private readonly HttpClient _httpClient;
        private Uri? _baseAddress;

        public HttpRequestsController()
        {
            _httpClient = new HttpClient();
        }

        public void SetBaseAddress(string address)
        {
            if (Uri.TryCreate(address, UriKind.Absolute, out var uri))
            {
                _baseAddress = uri;
            }
            else
            {
                throw new ArgumentException("Invalid base address");
            }
        }

        private Uri GetFullUri(string relativeUrl)
        {
            if (_baseAddress == null)
                throw new InvalidOperationException("Base address not set. Call SetBaseAddress() first.");

            return new Uri(_baseAddress, relativeUrl);
        }

        public async Task<T?> GetAsync<T>(string relativeUrl)
        {
            try
            {
                var response = await _httpClient.GetAsync(GetFullUri(relativeUrl));
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<T>(json);
                }

                return default;
            }
            catch (Exception ex)
            {
                // TODO: Логування помилки ex
                return default;
            }
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string relativeUrl, T body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await _httpClient.PostAsync(GetFullUri(relativeUrl), content);
            }
            catch (Exception ex)
            {
                // TODO: Логування помилки ex
                throw;
            }
        }

        public async Task<TResponse?> PutAsync<TRequest, TResponse>(string relativeUrl, TRequest body)
        {
            try
            {
                var json = JsonConvert.SerializeObject(body);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync(GetFullUri(relativeUrl), content);
                if (!response.IsSuccessStatusCode)
                    return default;

                var resultJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(resultJson);
            }
            catch (Exception ex)
            {
                // TODO: Логування помилки ex
                return default;
            }
        }

        public async Task<bool> DeleteAsync(string relativeUrl)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(GetFullUri(relativeUrl));
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // TODO: Логування помилки ex
                return false;
            }
        }

        public void SetBearerToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}
