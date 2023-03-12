namespace Panda.Portfolio.WebUI.Pages
{
    public partial class Index
    {
        private bool displayMessageRenderStarted;
        public string DisplayMessage { get; set; } = string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!AppState.Settings.IsLoading && !displayMessageRenderStarted)
            {
                displayMessageRenderStarted = true;
                StateHasChanged();
                const string msg = "Hi, My name is Majid, a Full Stack developer.";
                await Task.Run(async () =>
                {
                    var counter = -5;
                    for (; ; )
                    {
                        char? character = null;
                        try
                        {
                            character = msg[counter];
                        }
                        catch { }
                        if (character is not null)
                        {
                            DisplayMessage = DisplayMessage.Replace("_", "") + character + "_";
                        }
                        else if (counter >= msg.Length)
                        {
                            DisplayMessage = msg;
                            await InvokeAsync(StateHasChanged);
                            return Task.CompletedTask;
                        }
                        else
                        {
                            DisplayMessage = DisplayMessage.Replace("_", "") + "_";
                        }
                        counter++;
                        await InvokeAsync(StateHasChanged);
                        await Task.Delay(125);
                    }
                });
            }
        }
    }
}