using Microsoft.Extensions.Logging;
using Student_Attendance_Management_System.View.login;
using Student_Attendance_Management_System.View.pages;
using Student_Attendance_Management_System.ViewModel;
using ZXing.Net.Maui.Controls;

namespace Student_Attendance_Management_System
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();

            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<RegisterViewModel>();

            //Pages Services
            //QrScan DI
            builder.Services.AddTransient<QRScanPage>();
            builder.Services.AddTransient<QRViewModel>();
            //Record DI
            builder.Services.AddTransient<RecordPage>();
            builder.Services.AddTransient<RecordViewModel>();
            //DetailsPage DI
            builder.Services.AddTransient<DetailsPage>();
            builder.Services.AddTransient<DetailsStudentViewModel>();
            //EditPage DI
            builder.Services.AddTransient<EditPage>();
            builder.Services.AddTransient<EditViewModel>();
            //Profile DI
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<UpdateTeacherViewModel>();
            //Home DI
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<HomeViewModel>();
#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
