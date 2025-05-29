using HrPlatformClient.DTO;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HrPlatformClient.Services
{
    public class DepartmentsService : INotifyPropertyChanged
    {
        private readonly HttpRequestsController _http;
        private List<DepartmentDTO> _departmentsDTOs = [];

        public ObservableCollection<string> DepartmentNames { get; } = [];

        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isInitialized = false;

        public DepartmentsService(HttpRequestsController http)
        {
            _http = http;
        }

        public async Task InitAsync()
        {
            if (_isInitialized)
                return;

            _isInitialized = true;
            await LoadDepartmentsAsync();
        }

        public async Task ReloadAsync()
        {
            await LoadDepartmentsAsync();
        }

        public async Task CreateDepartmentAsync(string departmentName)
        {
            if (string.IsNullOrWhiteSpace(departmentName))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Назва відділу не може бути порожньою", "OK");
                return;
            }
            if (DepartmentNames.Contains(departmentName, StringComparer.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Відділ з такою назвою вже існує", "OK");
                return;
            }

            var department = new
            {
                name = departmentName
            };

            try
            {
                var response = await _http.PostAsync("api/departments/new", department);
                var json = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    DepartmentNames.Add(departmentName);

                    var data = JsonConvert.DeserializeObject<DepartmentDTO>(json);
                    if (data != null)
                    {
                        _departmentsDTOs.Add(data);
                    }
                    else
                    {
                        throw new Exception("Не вдалося отримати дані про новий відділ");
                    }

                    OnPropertyChanged(nameof(DepartmentNames));
                    Application.Current.MainPage.DisplayAlert("Успіх", "Відділ успішно створено", "OK");
                }
                else
                {
                    Application.Current.MainPage.DisplayAlert("Помилка", "Не вдалося створити відділ", "OK");
                }
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        public async void Remove(string departmentName)
        {
            var id = GetDepartmentIdByName(departmentName);

            try
            {
                bool success = await _http.DeleteAsync($"api/departments/dellete/{id}");
                if (success)
                {
                    var pos = _departmentsDTOs.FirstOrDefault(p => p.Name.Equals(departmentName, StringComparison.OrdinalIgnoreCase));
                    if (pos != null)
                    {
                        _departmentsDTOs.Remove(pos);
                        DepartmentNames.Remove(departmentName);
                        OnPropertyChanged(nameof(DepartmentNames));
                    }
                    await Application.Current.MainPage.DisplayAlert("Видалено", "Відділ успішно видалено", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Не вдалося видалити Відділ", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }


        private async Task LoadDepartmentsAsync()
        {
            _departmentsDTOs.Clear();
            _departmentsDTOs = await _http.GetAsync<List<DepartmentDTO>>("/api/departments/getAll");

            DepartmentNames.Clear();
            foreach (var pos in _departmentsDTOs)
            {
                DepartmentNames.Add(pos.Name);
            }

            OnPropertyChanged(nameof(DepartmentNames));
        }

        public int GetDepartmentIdByName(string name)
        {
            var pos = _departmentsDTOs.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            return pos?.Id ?? -1;
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
