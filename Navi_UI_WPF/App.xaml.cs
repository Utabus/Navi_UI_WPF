using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Navi.Application.Interfaces;
using Navi.Application.Services;
using Navi.Core.Interfaces;
using Navi.Infrastructure.Repositories;
using Navi_UI_WPF.Services;
using Navi_UI_WPF.ViewModels;
using Navi_UI_WPF.Views;

namespace Navi_UI_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private System.IServiceProvider _serviceProvider;

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

            ConfigureServices();
 
            // Hiển thị màn hình Login từ DI
            var loginWindow = _serviceProvider.GetRequiredService<LoginView>();
            loginWindow.Show();
        }

        private void ConfigureServices()
        {
            var services = new ServiceCollection();

            // ─── HTTP Client ───────────────────────────────────────────
            services.AddSingleton<System.Net.Http.HttpClient>(sp => new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri(Navi.Core.Constants.ApiEndpoints.BaseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            });

            // ─── Infrastucture / Repositories ───────────────────────────
            services.AddTransient<INaviHistoryRepository, NaviHistoryRepository>();
            services.AddTransient<IProductAssemblyRepository, ProductAssemblyRepository>();
            services.AddTransient<IManufaRepository, ManufaRepository>();

            // ─── Application Services ───────────────────────────────────
            services.AddSingleton<SessionManager>(_ => SessionManager.Instance);
            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<IDeviceService, DeviceService>();
            services.AddTransient<INaviHistoryService, NaviHistoryService>();
            services.AddTransient<INaviItemService, NaviItemService>();
            services.AddTransient<INaviProductMasterItemService, NaviProductMasterItemService>();
            services.AddTransient<ProductAssemblyService>();
            services.AddTransient<ManufaService>();
            services.AddTransient<ISettingsService, SettingsService>();

            // ─── ViewModels ─────────────────────────────────────────────
            services.AddTransient<LoginViewModel>();
            services.AddTransient<HomeViewModel>();
            services.AddTransient<ProductAssemblyViewModel>();
            services.AddTransient<NaviProductViewModel>();
            services.AddTransient<NaviItemViewModel>();
            services.AddTransient<NaviProductItemViewModel>();
            services.AddTransient<NaviHistoryViewModel>();
            services.AddTransient<ForceGaugeViewModel>();
            services.AddTransient<InsertBallViewModel>();
            services.AddTransient<InspectionViewModel>();
            services.AddTransient<PackingViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<NgReasonViewModel>();
            services.AddTransient<MainViewModel>();

            // ─── Views ──────────────────────────────────────────────────
            services.AddTransient<LoginView>();
            services.AddTransient<MainWindow>();
            services.AddTransient<HomeView>();
            services.AddTransient<ProductAssemblyView>();
            services.AddTransient<NaviProductView>();
            services.AddTransient<NaviItemView>();
            services.AddTransient<NaviProductItemView>();
            services.AddTransient<NaviHistoryView>();
            services.AddTransient<ForceGaugeView>();
            services.AddTransient<InsertBallView>();
            services.AddTransient<InspectionView>();
            services.AddTransient<PackingView>();
            services.AddTransient<NaviProductFormWindow>();
            services.AddTransient<NgReasonDialog>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Serilog.Log.Information("Application Exiting...");
            Serilog.Log.CloseAndFlush();
            base.OnExit(e);
        }
    }
}
