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

        public ICommand DeletePosition { get; }
        public ICommand CreatePosition { get; }

        public PositionsViewModel(HttpRequestsController http,PositionsService positionsService)
        {
            _http = http;
            _positionsService = positionsService;

            DeletePosition = new Command<string>(OnDeletePosition);
            CreatePosition = new Command(OnCreatePosition);
        }

        private void OnCreatePosition(object obj)
        {
            throw new NotImplementedException();
        }

        private async void OnDeletePosition(string obj)
        {
            var id = _positionsService.GetPositionIdByName(obj);

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Підтвердження",
                $"Ви дійсно хочете видалити посаду {obj}?",
                "Так", "Ні");

            if (!confirm)
                return;

            try
            {
                bool success = await _http.DeleteAsync($"api/positions/delete/{id}");
                if (success)
                {
                    _positionsService.Remove(obj);
                    await Application.Current.MainPage.DisplayAlert("Видалено", "Посаду успішно видалено", "OK");
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Помилка", "Не вдалося видалити Посаду", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
            }
        }

        public async Task InitAsync()
        {
            await _positionsService.InitAsync();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
