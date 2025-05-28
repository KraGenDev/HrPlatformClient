using HrPlatformClient.DTO;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace HrPlatformClient.ViewModels
{
    public class EditEmployeeViewModel : INotifyPropertyChanged
    {
        private readonly HttpRequestsController _http;

        private List<PositionDTO> positionDTOs;
        private List<DepartmentDTO> departmentDTOs;

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

        public ObservableCollection<string> Positions { get; } = [];

        public event Action PositionsLoaded;
        public event Action DepartmentsLoaded;


        public EditEmployeeViewModel(Employee employee, HttpRequestsController http)
        {
            _http = http;
            Employee = employee;

            SaveCommand = new Command(async () => await OnSave());
            CancelCommand = new Command(async () => await OnCancel());

            _ = LoadPositionsAsync();
            _ = LoadDepartments();
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
            employeeDto.Position = positionDTOs.First(item => item.Name == Employee.Position).Id;

            await Application.Current.MainPage.DisplayAlert("Заголовок", employeeDto.Position.ToString() + " + " + employeeDto.Department.ToString(), "OK");


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
