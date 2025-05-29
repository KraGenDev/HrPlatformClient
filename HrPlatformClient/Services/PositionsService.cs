using HrPlatformClient.DTO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace HrPlatformClient.Services
{
    public class PositionsService : INotifyPropertyChanged
    {
        private readonly HttpRequestsController _http;
        public ObservableCollection<string> Positions { get; } = [];

        private List<PositionDTO> positionDTOs;

        public event Action PositionsLoaded;
        public event PropertyChangedEventHandler? PropertyChanged;


        public PositionsService(HttpRequestsController http)
        {
            _http = http;
            positionDTOs = new List<PositionDTO>();
            LoadPositionsAsync().ConfigureAwait(false);
        }

        private async Task LoadPositionsAsync()
        {
            positionDTOs = await _http.GetAsync<List<PositionDTO>>("/api/positions/getAll");

            Positions.Clear();
            foreach (var pos in positionDTOs)
            {
                Positions.Add(pos.Name);
            }

            PositionsLoaded?.Invoke();

            OnPropertyChanged(nameof(Positions));
        }

        public int GetPositionIdByName(string positionName)
        {
            var position = positionDTOs.FirstOrDefault(p => p.Name.Equals(positionName, StringComparison.OrdinalIgnoreCase));
            return position?.Id ?? -1; // -1 indicates not found
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
