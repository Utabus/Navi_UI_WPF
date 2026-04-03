using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Serilog;

namespace Navi_UI_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Cấu hình Serilog global
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.File("logs/navi-.txt", rollingInterval: Serilog.RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                .CreateLogger();

            Serilog.Log.Information("Application Starting Up...");

            // Bắt lỗi toàn cục (Global Exception Handling)
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                Serilog.Log.Fatal(exception, "AppDomain Unhandled Exception (Lỗi nghiêm trọng không xác định)");
                Serilog.Log.CloseAndFlush();
            };

            this.DispatcherUnhandledException += (s, args) =>
            {
                Serilog.Log.Fatal(args.Exception, "Dispatcher Unhandled Exception (Lỗi UI/Luồng chính)");
                args.Handled = true; // Ngăn ứng dụng crash và hiển thị thông báo
                MessageBox.Show("Đã xảy ra lỗi hệ thống. Chi tiết được lưu trong file log.", "Lỗi hệ thống", MessageBoxButton.OK, MessageBoxImage.Error);
            };

            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                Serilog.Log.Fatal(args.Exception, "Unobserved Task Exception (Lỗi trong Task chạy ngầm)");
                args.SetObserved();
            };

            // Hiển thị màn hình Login — MainWindow sẽ được mở bởi LoginView.xaml.cs
            // sau khi xác thực thành công.
            var loginWindow = new Navi_UI_WPF.Views.LoginView();
            loginWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Serilog.Log.Information("Application Exiting...");
            Serilog.Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
