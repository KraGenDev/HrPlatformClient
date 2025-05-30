using CommunityToolkit.Maui;
using HrPlatformClient.Services;
using Microsoft.Extensions.Logging;

namespace HrPlatformClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton<HttpRequestsController>();
            builder.Services.AddSingleton<PositionsService>();
            builder.Services.AddSingleton<DepartmentsService>();
            builder.Services.AddTransient<EditEmployeePage>();

            return builder.Build();
        }
    }
}
