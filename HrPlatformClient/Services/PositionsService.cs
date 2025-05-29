using HrPlatformClient.DTO;
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

        public void Remove(string positionName)
        {
            if (string.IsNullOrEmpty(positionName))
                return;

            var pos = _positionDTOs.FirstOrDefault(p => p.Name.Equals(positionName, StringComparison.OrdinalIgnoreCase));
            if (pos != null)
            {
                _positionDTOs.Remove(pos);
                PositionNames.Remove(positionName);
                OnPropertyChanged(nameof(PositionNames));
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
