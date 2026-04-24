namespace Navi.Application.DTOs
{
    public class SystemSettingsDto
    {
        public bool IsAutoSaveEnabled { get; set; }
        public string DefaultTheme { get; set; }
        public int RefreshIntervalMs { get; set; }
    }
}
