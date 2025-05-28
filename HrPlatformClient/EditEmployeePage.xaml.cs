using HrPlatformClient.DTO;
using HrPlatformClient.ViewModels;

namespace HrPlatformClient;

public partial class EditEmployeePage : ContentPage, IQueryAttributable
{
    private readonly HttpRequestsController _http;

    public EditEmployeePage(HttpRequestsController http)
    {
        InitializeComponent();
        _http = http;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj.ToString(), out int id))
        {
            var employee = await _http.GetAsync<Employee>($"api/employees/{id}");
            var editEmployeeViewModel = new EditEmployeeViewModel(employee, _http);
            BindingContext = editEmployeeViewModel;

            editEmployeeViewModel.PositionsLoaded += () => PositionPicker.SelectedItem = employee.Position;
            editEmployeeViewModel.DepartmentsLoaded += () => DepartmentPicker.SelectedItem = employee.Department;

        }
    }
}

