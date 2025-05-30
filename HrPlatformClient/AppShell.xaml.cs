namespace HrPlatformClient
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Реєстрація додаткових сторінок
            Routing.RegisterRoute("LoginPage", typeof(LoginPage));
            Routing.RegisterRoute("editEmployeePage", typeof(EditEmployeePage));
            Routing.RegisterRoute("createEmployeePage", typeof(CreateEmployeePage));

            // Перехід до логіну при старті
            GoToLogin();
        }

        public void GoToLogin()
        {
            MainShellItem.IsVisible = false;
            LoginShellItem.IsVisible = true;
            CurrentItem = LoginShellItem;
        }

        public void GoToMain()
        {
            LoginShellItem.IsVisible = false;
            MainShellItem.IsVisible = true;
            CurrentItem = MainShellItem;
        }
    }
}
