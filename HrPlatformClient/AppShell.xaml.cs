namespace HrPlatformClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("editEmployeePage", typeof(EditEmployeePage));

        }
    }
}
