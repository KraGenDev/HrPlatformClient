using HrPlatformClient.DTO;
using HrPlatformClient.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HrPlatformClient.ViewModels
{
    public class CreateEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly HttpRequestsController _http;

        private List<DepartmentDTO> departmentDTOs;
        private PositionsService _positionsService;

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


        public ObservableCollection<string> Departments { get; } = [];


        public event Action DepartmentsLoaded;


        public CreateEmployeeViewModel(Employee employee, HttpRequestsController http, PositionsService positionsService)
        {
            _http = http;
            Employee = employee;
            _positionsService = positionsService;

            SaveCommand = new Command(async () => await OnSave());
            CancelCommand = new Command(async () => await OnCancel());

            _ = LoadDepartments();
        }


        private async Task LoadDepartments()
        {
            departmentDTOs = await _http.GetAsync<List<DepartmentDTO>>("/api/departments/getAll");

            Departments.Clear();
            foreach (var dep in departmentDTOs)
            {
                Departments.Add(dep.Name);
            }

            DepartmentsLoaded?.Invoke();

            OnPropertyChanged(nameof(Departments));
        }

        private async Task OnSave()
        {

            var employeeDto = new EmployeeDTO(Employee);
            employeeDto.Department = departmentDTOs.First(item => item.Name == Employee.Department).Id;
            employeeDto.Position = _positionsService.GetPositionIdByName(Employee.Position);

            await _http.PutAsync<EmployeeDTO, EmployeeDTO>($"api/employees/update/{Employee.Id}", employeeDto);
            await Shell.Current.GoToAsync("..");
        }


        private async Task OnCancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
