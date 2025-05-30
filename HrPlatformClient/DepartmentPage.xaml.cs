using CommunityToolkit.Maui.Views;
using HrPlatformClient.Services;
using HrPlatformClient.ViewModels;
using System.Windows.Input;

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

    private async void OnUpdateClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is string currentName)
        {
            var popup = new UpdateDepartmentPopup(currentName);
            var result = await this.ShowPopupAsync(popup);

            if (result is string newName && !string.IsNullOrWhiteSpace(newName))
            {
                // Тут встав свій код для оновлення Position
                // Наприклад, ViewModel.UpdatePositionCommand.Execute((currentName, newName));
                await _viewModel.UpdateDepartmentAsync(currentName, newName);
            }
        }
    }
}
