using CommunityToolkit.Maui.Views;
using HrPlatformClient.Services;
using HrPlatformClient.ViewModels;

namespace HrPlatformClient;

public partial class PositionPage : ContentPage
{
    private readonly PositionsViewModel _viewModel;

    public PositionPage(PositionsService positionsService)
    {
        InitializeComponent();
        _viewModel = new PositionsViewModel(positionsService);
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
            var popup = new UpdatePositionPopup(currentName);
            var result = await this.ShowPopupAsync(popup);

            if (result is string newName && !string.IsNullOrWhiteSpace(newName))
            {
                // Тут встав свій код для оновлення Position
                // Наприклад, ViewModel.UpdatePositionCommand.Execute((currentName, newName));
                await _viewModel.UpdatePositionAsync(currentName, newName);
            }
        }
    }

}
