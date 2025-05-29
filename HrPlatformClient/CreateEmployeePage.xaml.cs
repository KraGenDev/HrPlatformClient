using HrPlatformClient.DTO;
using HrPlatformClient.Services;
using HrPlatformClient.ViewModels;

namespace HrPlatformClient;

public partial class CreateEmployeePage : ContentPage
{
    private readonly HttpRequestsController _http;
    private readonly PositionsService _positionsService;
    private readonly DepartmentsService _departmentsService;

    public CreateEmployeePage(HttpRequestsController http, PositionsService positionsService, DepartmentsService departmentsService)
    {
        InitializeComponent();
        _http = http;
        _positionsService = positionsService;
        _departmentsService = departmentsService;

        _ = InitAsync();
    }

    private async Task InitAsync()
    {
        await _positionsService.InitAsync();
        await _departmentsService.InitAsync();

        var employee = new Employee
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            Phone = string.Empty,
            Position = _positionsService.PositionNames.FirstOrDefault() ?? string.Empty,
            Department = _departmentsService.DepartmentNames.FirstOrDefault() ?? string.Empty
        };

        var viewModel = new CreateEmployeeViewModel(employee, _http, _positionsService, _departmentsService);
        BindingContext = viewModel;

        PositionPicker.SelectedItem = employee.Position;
        DepartmentPicker.SelectedItem = employee.Department;
    }
}