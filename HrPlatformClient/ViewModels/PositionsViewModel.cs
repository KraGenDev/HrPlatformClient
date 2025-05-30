using HrPlatformClient.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HrPlatformClient.ViewModels
{
    public class PositionsViewModel : INotifyPropertyChanged
    {
        private readonly PositionsService _positionsService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> Positions => _positionsService.PositionNames;

        public ICommand DeletePosition { get; }
        public ICommand CreatePosition { get; }
        public ICommand ToggleAddMode { get; }


        public Color AddButtonColor => IsAddingPosition ? Color.FromArgb("#E57373") : Colors.LimeGreen;

        private bool _isAddingPosition;
        public bool IsAddingPosition
        {
            get => _isAddingPosition;
            set
            {
                _isAddingPosition = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddButtonText));
                OnPropertyChanged(nameof(AddButtonColor));
            }
        }


        public string AddButtonText => IsAddingPosition ? "Скасувати" : "Додати";

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

        public PositionsViewModel(PositionsService positionsService)
        {
            _positionsService = positionsService;

            DeletePosition = new Command<string>(OnDeletePosition);
            CreatePosition = new Command(OnCreatePosition);
            ToggleAddMode = new Command(() =>
            {
                IsAddingPosition = !IsAddingPosition;
                NewPositionName = string.Empty;
            });

            NewPositionName = string.Empty;
        }

        private async void OnCreatePosition()
        {
            if (string.IsNullOrWhiteSpace(NewPositionName))
                return;

            await _positionsService.CreatePositionAsynk(NewPositionName);
            NewPositionName = string.Empty;
            IsAddingPosition = false;
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

        public async Task UpdatePositionAsync(string oldName, string newName)
        {
            try
            {
                await _positionsService.UpdatePositionAsync(oldName, newName);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Помилка 1", ex.Message, "OK");
            }
        }


        public async Task InitAsync() => await _positionsService.InitAsync();

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
