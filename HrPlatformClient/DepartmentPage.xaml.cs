using HrPlatformClient.Services;
using HrPlatformClient.ViewModels;

namespace HrPlatformClient;

public partial class DepartmentPage : ContentPage
{
    private readonly DepartmentViewModel _viewModel;

    public DepartmentPage(DepartmentsService departmentService)
    {
        InitializeComponent();
        _viewModel = new DepartmentViewModel(departmentService);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitAsync();
    }
}
