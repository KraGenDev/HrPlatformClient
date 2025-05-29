using HrPlatformClient.Services;
using HrPlatformClient.ViewModels;

namespace HrPlatformClient;

public partial class PositionPage : ContentPage
{
    private readonly PositionsViewModel _viewModel;

    public PositionPage(HttpRequestsController http, PositionsService positionsService)
    {
        InitializeComponent();
        _viewModel = new PositionsViewModel(http, positionsService);
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitAsync();
    }
}
