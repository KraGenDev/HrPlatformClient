using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using HrPlatformClient.DTO;
using HrPlatformClient;

public class EmployeesViewModel : INotifyPropertyChanged
{
    private readonly HttpRequestsController _http;

    private string _findRequestWords;

    public ObservableCollection<Employee> Employees { get; } = [];

    private bool _isSearchPanelVisible;
    public bool IsSearchPanelVisible
    {
        get => _isSearchPanelVisible;
        set
        {
            _isSearchPanelVisible = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SearchToggleButtonText));
            OnPropertyChanged(nameof(SearchToggleButtonColor));
        }
    }

    private bool _searchUsed = false;
    public string SearchToggleButtonText => IsSearchPanelVisible ? "Скасувати" : "Пошук";
    public Color SearchToggleButtonColor => IsSearchPanelVisible ? Color.FromArgb("#374151") : Color.FromArgb("#5F80C8");

    public string FindRequestWords
    {
        get => _findRequestWords;
        set
        {
            _findRequestWords = value;
            OnPropertyChanged();
        }
    }
    public ICommand EditEmployeeCommand { get; }
    public ICommand CreateEmployeeCommand { get; }
    public ICommand DeleteEmployeeCommand { get; }
    public ICommand FindEmployeeCommand { get; }
    public ICommand ToggleAddMode { get; }


    public event PropertyChangedEventHandler PropertyChanged;


    public EmployeesViewModel(HttpRequestsController http)
    {
        _http = http;

        EditEmployeeCommand = new Command<Employee>(OnEditEmployee);
        DeleteEmployeeCommand = new Command<Employee>(OnDeleteEmployee);
        CreateEmployeeCommand = new Command(OnCreateEmployee);
        FindEmployeeCommand = new Command(async () => await OnFindEmployee());

        ToggleAddMode = new Command(async () =>
        {
            IsSearchPanelVisible = !IsSearchPanelVisible;
            FindRequestWords = string.Empty;

            if (!IsSearchPanelVisible && _searchUsed)
            {
                _searchUsed = false;
                await LoadEmployeesAsync();
            }
        });


        FindRequestWords = string.Empty;
    }

    private async Task OnFindEmployee()
    {
        if (string.IsNullOrWhiteSpace(FindRequestWords))
        {
            await Application.Current.MainPage.DisplayAlert("Помилка", "Будь ласка, введіть пошуковий запит", "OK");
            return;
        }

        try
        {
            var employees = await _http.GetAsync<List<Employee>>($"api/employees/search?keyword={Uri.EscapeDataString(FindRequestWords)}");
            Employees.Clear();
            if (employees != null)
            {
                foreach (Employee emp in employees)
                {
                    Employees.Add(emp);
                }
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
        }

        _searchUsed = true;
    }

    private async void OnCreateEmployee()
    {
        await Shell.Current.GoToAsync($"createEmployeePage");
    }

    public async Task LoadEmployeesAsync()
    {
        try
        {
            var employees = await _http.GetAsync<List<Employee>>("api/employees/getAll");

            Employees.Clear();
            if (employees != null)
            {
                foreach (Employee emp in employees)
                {
                    Employees.Add(emp);
                }
            }
        }
        catch (Exception ex)
        {
            // Логіка обробки помилки (лог, повідомлення тощо)
        }
    }

    private async void OnEditEmployee(Employee employee)
    {
        if (employee == null)
            return;

        await Shell.Current.GoToAsync($"editEmployeePage?id={employee.Id}");
    }

    private async void OnDeleteEmployee(Employee employee)
    {
        if (employee == null)
            return;

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Підтвердження",
            $"Ви дійсно хочете видалити працівника {employee.FirstName} {employee.LastName}?",
            "Так", "Ні");

        if (!confirm)
            return;

        try
        {
            bool success = await _http.DeleteAsync($"api/employees/delete/{employee.Id}");
            if (success)
            {
                Employees.Remove(employee);
                await Application.Current.MainPage.DisplayAlert("Видалено", "Працівника успішно видалено", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Помилка", "Не вдалося видалити працівника", "OK");
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Помилка", ex.Message, "OK");
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
