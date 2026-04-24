using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Navi.Application.DTOs;
using Navi.Application.Interfaces;

namespace Navi_UI_WPF.ViewModels
{
    public class SettingsViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));

            // EXPLICIT initialization of commands for .NET 4.7.2
            LoadCommand = new AsyncRelayCommand(LoadSettingsAsync);
            SaveCommand = new AsyncRelayCommand(SaveSettingsAsync);

            // Initial load
            _ = LoadSettingsAsync();
        }

        // ─── Properties ──────────────────────────────────────────────────

        private bool _isAutoSaveEnabled;
        public bool IsAutoSaveEnabled
        {
            get => _isAutoSaveEnabled;
            set => SetProperty(ref _isAutoSaveEnabled, value);
        }

        private string _defaultTheme;
        public string DefaultTheme
        {
            get => _defaultTheme;
            set => SetProperty(ref _defaultTheme, value);
        }

        private int _refreshIntervalMs;
        public int RefreshIntervalMs
        {
            get => _refreshIntervalMs;
            set => SetProperty(ref _refreshIntervalMs, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        // ─── Commands ────────────────────────────────────────────────────

        public IAsyncRelayCommand LoadCommand { get; }
        public IAsyncRelayCommand SaveCommand { get; }

        // ─── Logic ───────────────────────────────────────────────────────

        private async Task LoadSettingsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var settings = await _settingsService.GetSettingsAsync();

                if (settings != null)
                {
                    IsAutoSaveEnabled = settings.IsAutoSaveEnabled;
                    DefaultTheme      = settings.DefaultTheme;
                    RefreshIntervalMs = settings.RefreshIntervalMs;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveSettingsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                var dto = new SystemSettingsDto
                {
                    IsAutoSaveEnabled = IsAutoSaveEnabled,
                    DefaultTheme      = DefaultTheme,
                    RefreshIntervalMs = RefreshIntervalMs
                };

                await _settingsService.UpdateSettingsAsync(dto);
                MessageBox.Show("Settings saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
