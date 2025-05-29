namespace HrPlatformClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("editEmployeePage", typeof(EditEmployeePage));
            Routing.RegisterRoute("createEmployeePage", typeof(CreateEmployeePage));

            Device.BeginInvokeOnMainThread(async () =>
            {
                await GoToAsync("//LoginPage");
            });

        }
    }
}
