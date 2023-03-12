using Microsoft.AspNetCore.Components;

using Panda.BlazorCore.Components;
using Panda.MudBlazorLib.Model.Interfaces;
using Panda.Portfolio.WebUI.App;

namespace Panda.Portfolio.WebUI.Components
{
    public class AppCoreComponent : CoreComponent
    {
        [CascadingParameter] public IApp<AppSettings> AppState { get; set; } = default!;
    }
}