using HrPlatformClient.DTO;
using HrPlatformClient.Services;
using HrPlatformClient.ViewModels;

namespace HrPlatformClient;

public partial class EditEmployeePage : ContentPage, IQueryAttributable
{
    private readonly HttpRequestsController _http;
    private readonly PositionsService _positionsService;
    private readonly DepartmentsService _departmentsService;

    public EditEmployeePage(HttpRequestsController http, PositionsService positionsService, DepartmentsService departmentsService)
    {
        InitializeComponent();
        _http = http;
        _positionsService = positionsService;
        _departmentsService = departmentsService;
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var idObj) && int.TryParse(idObj.ToString(), out int id))
        {
            await _positionsService.InitAsync();
            await _departmentsService.InitAsync();

            var employee = await _http.GetAsync<Employee>($"api/employees/{id}");

            var editEmployeeViewModel = new EditEmployeeViewModel(employee, _http, _positionsService,_departmentsService);
            BindingContext = editEmployeeViewModel;

            PositionPicker.SelectedItem = employee.Position;
            DepartmentPicker.SelectedItem = employee.Department;
        }
    }
}

