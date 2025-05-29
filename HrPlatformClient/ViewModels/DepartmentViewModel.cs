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

        public ICommand DeleteDepartment { get; }
        public ICommand CreateDepartment { get; }

        public DepartmentViewModel(DepartmentsService departmentsService)
        {
            _departmentService = departmentsService;

            DeleteDepartment = new Command<string>(OnDeleteDepartment);
            CreateDepartment = new Command(OnCreateDepartment);
        }

        private async void OnCreateDepartment(object obj)
        {
            await _departmentService.CreateDepartmentAsync(NewDepartmentName);
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
