using HrPlatformClient.DTO;
using HrPlatformClient.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HrPlatformClient.ViewModels
{
    public class PositionsViewModel : INotifyPropertyChanged
    {
        private readonly HttpRequestsController _http;
        private readonly PositionsService _positionsService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> Positions => _positionsService.PositionNames;

        private string _newPositionName;
        public string NewPositionName
        {
            get => _newPositionName;
            set
            {
                _newPositionName = value;
                OnPropertyChanged();
            }
        }

        public ICommand DeletePosition { get; }
        public ICommand CreatePosition { get; }

        public PositionsViewModel(HttpRequestsController http,PositionsService positionsService)
        {
            _http = http;
            _positionsService = positionsService;

            DeletePosition = new Command<string>(OnDeletePosition);
            CreatePosition = new Command(OnCreatePosition);
        }

        private async void OnCreatePosition(object obj)
        {
            await _positionsService.CreatePositionAsynk(NewPositionName);
        }

        private async void OnDeletePosition(string name)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Підтвердження",
                $"Ви дійсно хочете видалити посаду {name}?",
                "Так", "Ні");

            if (!confirm)
                return;

            _positionsService.Remove(name);
        }

        public async Task InitAsync()
        {
            await _positionsService.InitAsync();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
