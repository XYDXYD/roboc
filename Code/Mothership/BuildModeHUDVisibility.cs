using Fabric;
using Svelto.Command;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class BuildModeHUDVisibility : IHandleEditingInput, IInitialize, IWaitForFrameworkDestruction, IInputComponent, IComponent
	{
		private bool _hudHiddenByPlayer;

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership inputController
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_hudHiddenByPlayer = false;
			inputController.OnScreenStateChange += OnScreenChange;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			inputController.OnScreenStateChange -= OnScreenChange;
		}

		private void OnScreenChange()
		{
			_hudHiddenByPlayer = false;
			commandFactory.Build<ToggleBuildModeHUDVisibilityCommand>().Inject(dependancy: true).Execute();
		}

		public void HandleEditingInput(InputEditingData data)
		{
			if (data[EditingInputAxis.HIDEHUD_BUILDMODE] == 1f && (inputController.GetActiveScreen() == GuiScreens.BuildMode || inputController.GetActiveScreen() == GuiScreens.Garage))
			{
				EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.UIButtonSelect));
				data[EditingInputAxis.HIDEHUD_BUILDMODE] = 0f;
				_hudHiddenByPlayer = !_hudHiddenByPlayer;
				commandFactory.Build<ToggleBuildModeHUDVisibilityCommand>().Inject(!_hudHiddenByPlayer).Execute();
			}
		}
	}
}
