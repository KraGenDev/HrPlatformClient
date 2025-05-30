using HrPlatformClient.DTO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace HrPlatformClient.Services
{
    public class PositionsService : INotifyPropertyChanged
    {
        private readonly HttpRequestsController _http;
        private List<PositionDTO> _positionDTOs = [];

        public ObservableCollection<string> PositionNames { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isInitialized = false;

        public PositionsService(HttpRequestsController http)
        {
            _http = http;
        }

        public async Task InitAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            await LoadPositionsAsync();
        }

        public async Task ReloadAsync()
        {
            await LoadPositionsAsync();
        }

        public async Task CreatePositionAsynk(string positionName)
        {
            if (string.IsNullOrWhiteSpace(positionName))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Назва посади не може бути порожньою", "OK");
                return;
            }
            if (PositionNames.Contains(positionName, StringComparer.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Посада з такою назвою вже існує", "OK");
                return;
            }

            var position = new
            {
                name = positionName
            };

            try
            {
                var response = await _http.PostAsync("api/positions/new", position);
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    PositionNames.Add(positionName);

                    var data = JsonConvert.DeserializeObject<PositionDTO>(json);
                    if (data != null)
                    {
                        _positionDTOs.Add(data);
                    }
                    else
                    {
                        throw new Exception("Не вдалося отримати дані про нову посаду");
                    }

                    OnPropertyChanged(nameof(PositionNames));
                    Application.Current.MainPage.DisplayAlert("Успіх", "Посаду успішно створено", "OK");
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Помилка", "Не вдалося створити посаду", "OK");
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        public async void Remove(string positionName)
        {
            var id = GetPositionIdByName(positionName);

            try
            {
                bool success = await _http.DeleteAsync($"api/positions/delete/{id}");
                if (success)
                {
                    var pos = _positionDTOs.FirstOrDefault(p => p.Name.Equals(positionName, StringComparison.OrdinalIgnoreCase));
                    if (pos != null)
                    {
                        _positionDTOs.Remove(pos);
                        PositionNames.Remove(positionName);
                        OnPropertyChanged(nameof(PositionNames));
                    }
                    await Application.Current.MainPage.DisplayAlert("Видалено", "Посаду успішно видалено", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Не вдалося видалити посаду", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        public async Task<bool> UpdatePositionAsync(string name, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Назва посади не може бути порожньою", "OK");
                return false;
            }

            var id = GetPositionIdByName(name);

            if (id == -1)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Посада не знайдена", "OK");
                return false;
            }

            if (PositionNames.Contains(newName, StringComparer.OrdinalIgnoreCase) && !newName.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Посада з такою назвою вже існує", "OK");
                return false;
            }

            try
            {
                var updateObj = new { name = newName };

                var updatedPosition = await _http.PutAsync<object, PositionDTO>($"api/positions/update/{id}", updateObj);

                if (updatedPosition != null)
                {
                    var pos = _positionDTOs.FirstOrDefault(p => p.Id == id);
                    if (pos != null)
                    {
                        pos.Name = updatedPosition.Name;

                        int index = PositionNames.IndexOf(name);
                        if (index >= 0)
                        {
                            PositionNames[index] = updatedPosition.Name;
                        }
                        OnPropertyChanged(nameof(PositionNames));
                    }

                    await Application.Current.MainPage.DisplayAlert("Успіх", "Посаду успішно оновлено", "OK");
                    return true;
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Не вдалося оновити посаду", "OK");
                    return false;
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
                return false;
            }
        }

        private async Task LoadPositionsAsync()
        {
            _positionDTOs.Clear();
            _positionDTOs = await _http.GetAsync<List<PositionDTO>>("/api/positions/getAll");

            PositionNames.Clear();
            foreach (var pos in _positionDTOs)
            {
                PositionNames.Add(pos.Name);
            }

            OnPropertyChanged(nameof(PositionNames));
        }

        public int GetPositionIdByName(string name)
        {
            var pos = _positionDTOs.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return pos?.Id ?? -1;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
