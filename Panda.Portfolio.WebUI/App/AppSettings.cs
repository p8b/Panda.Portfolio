using MudBlazor;

using Panda.MudBlazorLib.Model.Interfaces;

namespace Panda.Portfolio.WebUI.App
{
    public class AppSettings : IAppSettings
    {
        public bool IsDarkTheme { get; set; } = true;
        public MaxWidth ContainerMaxWidth { get; set; } = MaxWidth.Medium;
        public bool NavDrawerOpen { get; set; } = false;
        public bool IsLoading { get; set; } = true;
    }
}