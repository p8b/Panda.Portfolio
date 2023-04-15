using Microsoft.AspNetCore.Components;

using MudBlazor;
using MudBlazor.Services;

using Panda.RubikCube;
using Panda.RubikCube.Entities;
using Panda.RubikCube.Enums;
using Panda.RubikCube.Services;

namespace Panda.Portfolio.WebUI.Pages.RubikCubeGamePage
{
	public partial class RubikCubeGame : IAsyncDisposable
	{
		private string moveHoverWrapClass { get; set; }
		private bool ShowCoordinates { get; set; }
		private int CubeSize { get; set; } = 3;
		private RubiksCube Cube { get; set; }

		[Inject] private IBreakpointService BreakpointListener { get; set; } = default!;
		private readonly IRubiksCubeSolver _solver = new ReverseMoveHistorySolver();
		private Guid _subscriptionId;
		private Width CubeDisplaySize = Width.False;

		public RubikCubeGame()
		{
			Cube = new(_solver, new(CubeSize));
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			var currentBreakPoint = await BreakpointListener.GetBreakpoint();
			SetCubeDisplaySize(currentBreakPoint);
			if (firstRender)
			{
				var subscriptionResult = await BreakpointListener.Subscribe((_) =>
				{
					InvokeAsync(StateHasChanged);
				}, new ResizeOptions
				{
					ReportRate = 10,
					NotifyOnBreakpointOnly = false,
				});

				_subscriptionId = subscriptionResult.SubscriptionId;
				StateHasChanged();
			}
			await base.OnAfterRenderAsync(firstRender);
		}

		private void ToggleCoordinates()
		{
			ShowCoordinates = !ShowCoordinates;
			StateHasChanged();
		}

		private void Clear()
		{
			Cube = new RubiksCube(_solver, new(CubeSize));
			StateHasChanged();
		}

		public void SetCubeSize(int size)
		{
			CubeSize = size;
			Cube = new RubiksCube(_solver, new(CubeSize));
			StateHasChanged();
		}

		private void SetCubeDisplaySize(Breakpoint currentBreakPoint)
		{
			var containerWidth = AppState.Settings.ContainerMaxWidth switch
			{
				MaxWidth.ExtraSmall => Width.xs,
				MaxWidth.Small => Width.sm,
				MaxWidth.Medium => Width.md,
				MaxWidth.Large => Width.lg,
				_ => Width.xl,
			};
			var breakpointWidth = currentBreakPoint switch
			{
				Breakpoint.Xs => Width.xs,
				Breakpoint.Sm => Width.sm,
				Breakpoint.Md => Width.md,
				Breakpoint.Lg => Width.lg,
				_ => Width.xl,
			};
			var result = breakpointWidth > containerWidth ? containerWidth : breakpointWidth;
			Console.WriteLine(currentBreakPoint);
			if (result != CubeDisplaySize)
			{
				CubeDisplaySize = result;
				StateHasChanged();
			}
		}

		private void Move(FaceSide side, Rotation rotation)
		{
			var move = new CubeMoveRequest(side, rotation);
			Cube.Execute(move);
		}

		private async Task MixUp()
		{
			await Cube.MixUp(30, async () =>
			{
				await InvokeAsync(StateHasChanged);
				await Task.Delay(100);
			});
		}

		private async Task Solve()
		{
			await Cube.Solve(async () =>
			{
				await InvokeAsync(StateHasChanged);
				await Task.Delay(200);
			});
			StateHasChanged();
		}

		private MarkupString GetHistory()
		{
			var result = new List<string>();
			var result100Char = "";
			foreach (var item in Cube.MoveHistory)
			{
				var itemResult = (item.GetShortString() + " ");
				if (result100Char.Length + itemResult.Length < 100)
				{
					result100Char += itemResult;
				}
				else
				{
					result.Add(result100Char);
					result100Char = "<br><br>" + itemResult;
				}
			}
			result.Add(result100Char);

			return (MarkupString)string.Concat(result);
		}

		private void MouseoverClockwise(FaceSide faceSide)
		{
			moveHoverWrapClass = $"{faceSide.ToString().ToLower()}-hover clockwise-hover ";
			StateHasChanged();
		}

		private void MouseoverCounterclockwise(FaceSide faceSide)
		{
			moveHoverWrapClass = $"{faceSide.ToString().ToLower()}-hover counter-clockwise-hover ";
			StateHasChanged();
		}

		private void MouseOut()
		{
			moveHoverWrapClass = string.Empty;
			StateHasChanged();
		}

		public async ValueTask DisposeAsync() => await BreakpointListener.Unsubscribe(_subscriptionId);
	}
}