using HrPlatformClient.DTO;
using HrPlatformClient.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HrPlatformClient.ViewModels
{
    public class EditEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly HttpRequestsController _http;
        private readonly PositionsService _positionsService;
        private readonly DepartmentsService _departmentsService;


        public Employee Employee { get; }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private string _selectedDepartment;
        public string SelectedDepartment
        {
            get => _selectedDepartment;
            set
            {
                if (_selectedDepartment != value)
                {
                    _selectedDepartment = value;
                    Employee.Department = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _selectedPosition;
        public string SelectedPosition
        {
            get => _selectedPosition;
            set
            {
                if (_selectedPosition != value)
                {
                    _selectedPosition = value;
                    Employee.Position = value;
                    OnPropertyChanged();
                }
            }
        }


        public ObservableCollection<string> Departments => _departmentsService.DepartmentNames;
        public ObservableCollection<string> Positions => _positionsService.PositionNames;

        public event PropertyChangedEventHandler PropertyChanged;


        public EditEmployeeViewModel(Employee employee, HttpRequestsController http, PositionsService positionsService,DepartmentsService departmentsService)
        {
            _http = http;
            Employee = employee;
            _positionsService = positionsService;
            _departmentsService = departmentsService;

            SaveCommand = new Command(async () => await OnSave());
            CancelCommand = new Command(async () => await OnCancel());
        }


        private async Task OnSave()
        {

            var employeeDto = new EmployeeDTO(Employee);
            employeeDto.Department = _departmentsService.GetDepartmentIdByName(Employee.Department);
            employeeDto.Position = _positionsService.GetPositionIdByName(Employee.Position);

            await _http.PutAsync<EmployeeDTO, EmployeeDTO>($"api/employees/update/{Employee.Id}", employeeDto);
            await Shell.Current.GoToAsync("..");
        }


        private async Task OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}