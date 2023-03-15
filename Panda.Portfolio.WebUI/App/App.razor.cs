using System.Reflection.Metadata;

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.JSInterop;

using MudBlazor;

using Panda.BlazorCore.Services.Interfaces;
using Panda.MudBlazorLib.Model.Interfaces;
using Panda.MudBlazorLib.Themes;

namespace Panda.Portfolio.WebUI.App;

public partial class App : ComponentBase, IApp<AppSettings>
{
    private string _pageTitle = string.Empty;
    public string PageTitle
    {
        get
        {
            const string appName = "Portfolio | Panda";
            if (string.IsNullOrWhiteSpace(_pageTitle))
                return appName;
            else
                return $"{_pageTitle} | {appName}";
        }
        set
        {
            if (value == _pageTitle)
                return;

            _pageTitle = value;
            StateHasChanged();
        }
    }
    public AppSettings Settings { get; set; } = new AppSettings();
    public Theme Theme { get; set; } = new();

    [Parameter] public RenderFragment? ChildContent { private get; set; }

    [Inject] private ProtectedLocalStorage Storage { get; set; } = default!;
    [Inject] private IJSService JSService { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var result = await Storage.GetAsync<AppSettings>(nameof(AppSettings));

            if (result.Success && result.Value?.Equals(Settings) is false)
            {
                Settings = result.Value;
                Settings.IsLoading = true;
            }
            await JSService.PageLoadCompleteCallBack(this, nameof(FullPageLoaded));
            StateHasChanged();
        }
    }

    [JSInvokable]
    public async Task FullPageLoaded(string readyState)
    {
        if (Settings.IsLoading && readyState.Equals("complete", StringComparison.OrdinalIgnoreCase))
        {
            Settings.IsLoading = false;
            await Storage.SetAsync(nameof(AppSettings), Settings);
            StateHasChanged();
        }
        else if (!Settings.IsLoading && !readyState.Equals("complete", StringComparison.OrdinalIgnoreCase))
        {
            Settings.IsLoading = true;
            await Storage.SetAsync(nameof(AppSettings), Settings);
            StateHasChanged();
        }
    }

    public async Task OnContainerMaxWithChange(MaxWidth maxWidth)
    {
        if (Settings.ContainerMaxWidth != maxWidth)
        {
            Settings.ContainerMaxWidth = maxWidth;
            await Storage.SetAsync(nameof(AppSettings), Settings);
            StateHasChanged();
        }
    }

    public async Task ToggleTheme(bool isDarkTheme)
    {
        Settings.IsDarkTheme = isDarkTheme;
        await Storage.SetAsync(nameof(AppSettings), Settings);
    }

    public async Task OnSideBarToggle()
    {
        Settings.NavDrawerOpen = !Settings.NavDrawerOpen;
        await Storage.SetAsync(nameof(AppSettings), Settings);
    }

    public async Task SetNavDrawerOpen(bool value)
    {
        if (Settings.NavDrawerOpen != value)
        {
            Settings.NavDrawerOpen = value;
            await Storage.SetAsync(nameof(AppSettings), Settings);
        }
    }
}