namespace HrPlatformClient;

public partial class EmployeesPage : ContentPage
{
	public EmployeesPage(HttpRequestsController http)
	{
		InitializeComponent();
        BindingContext = new EmployeesViewModel(http);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (BindingContext is EmployeesViewModel vm)
        {
            await vm.LoadEmployeesAsync();
        }
    }


    private void OnAddEmployeeClicked() { }
} 