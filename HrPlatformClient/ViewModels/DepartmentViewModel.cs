using HrPlatformClient.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HrPlatformClient.ViewModels
{
    public class DepartmentViewModel : INotifyPropertyChanged
    {
        private readonly DepartmentsService _departmentService;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<string> Departments => _departmentService.DepartmentNames;

        private string _newDepartmentName;
        public string NewDepartmentName
        {
            get => _newDepartmentName;
            set
            {
                _newDepartmentName = value;
                OnPropertyChanged();
            }
        }

        private bool _isAddingDepartment;
        public bool IsAddingDepartment
        {
            get => _isAddingDepartment;
            set
            {
                _isAddingDepartment = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AddDepartmentButtonText));
                OnPropertyChanged(nameof(AddDepartmentButtonColor));
            }
        }

        public string AddDepartmentButtonText => IsAddingDepartment ? "Скасувати" : "Додати";
        public Color AddDepartmentButtonColor => IsAddingDepartment ? Color.FromArgb("#E57373") : Colors.LimeGreen;

        public ICommand ToggleAddDepartmentMode { get; }

        public ICommand DeleteDepartment { get; }
        public ICommand CreateDepartment { get; }

        public DepartmentViewModel(DepartmentsService departmentsService)
        {
            _departmentService = departmentsService;

            DeleteDepartment = new Command<string>(OnDeleteDepartment);
            CreateDepartment = new Command(OnCreateDepartment);
            ToggleAddDepartmentMode = new Command(() =>
            {
                IsAddingDepartment = !IsAddingDepartment;
                NewDepartmentName = string.Empty;
            });

            NewDepartmentName = string.Empty;
        }

        private async void OnCreateDepartment()
        {
            if (string.IsNullOrWhiteSpace(NewDepartmentName))
                return;

            await _departmentService.CreateDepartmentAsync(NewDepartmentName);
            NewDepartmentName = string.Empty;
            IsAddingDepartment = false;
        }

        private async void OnDeleteDepartment(string name)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Підтвердження",
                $"Ви дійсно хочете видалити відділ {name}?",
                "Так", "Ні");

            if (!confirm)
                return;

            _departmentService.Remove(name);
        }

        public async Task InitAsync()
        {
            await _departmentService.InitAsync();
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
