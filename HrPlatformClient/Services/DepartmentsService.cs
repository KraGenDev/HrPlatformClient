using HrPlatformClient.DTO;
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
