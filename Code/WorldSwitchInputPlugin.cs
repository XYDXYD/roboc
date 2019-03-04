using InputMask;
using Mothership;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

internal sealed class WorldSwitchInputPlugin : IInputPlugin, IComponent
{
	[Inject]
	internal IInputActionMask inputActionMask
	{
		private get;
		set;
	}

	private event Action<bool> OnWorldSwitchInput = delegate
	{
	};

	private event Action OnWorldSwitchInputTestModeShortcut = delegate
	{
	};

	private event Action OnWorldSwitchInputPracticeModeShortcut = delegate
	{
	};

	private event Action OnWorldSwitchBuildModeShortcut = delegate
	{
	};

	private event Action OnWorldSwitchQuickPlayModeShortcut = delegate
	{
	};

	private event Action OnWorldSwitchMainMenuShortcut = delegate
	{
	};

	private event Action OnWorldSwitchBrawlMatchShortcut = delegate
	{
	};

	public void RegisterComponent(IInputComponent component)
	{
		if (component is IHandleWorldSwitchInput)
		{
			IHandleWorldSwitchInput obj = component as IHandleWorldSwitchInput;
			OnWorldSwitchInput += obj.HandleWorldSwitchInput;
		}
		if (component is IHandleWorldSwitchInputPracticeModeShortcut)
		{
			IHandleWorldSwitchInputPracticeModeShortcut obj2 = component as IHandleWorldSwitchInputPracticeModeShortcut;
			OnWorldSwitchInputPracticeModeShortcut += obj2.HandleWorldSwitchInputPracticeModeShortcut;
		}
		if (component is IHandleWorldSwitchInputTestModeShortcut)
		{
			IHandleWorldSwitchInputTestModeShortcut obj3 = component as IHandleWorldSwitchInputTestModeShortcut;
			OnWorldSwitchInputTestModeShortcut += obj3.HandleWorldSwitchInputTestModeShortcut;
		}
		if (component is IHandleWorldSwitchInputQuickPlayModeShortcut)
		{
			IHandleWorldSwitchInputQuickPlayModeShortcut obj4 = component as IHandleWorldSwitchInputQuickPlayModeShortcut;
			OnWorldSwitchQuickPlayModeShortcut += obj4.HandleWorldSwitchInputQuickPlayModeShortcut;
		}
		if (component is IHandleWorldSwitchInputBuildModeShortcut)
		{
			IHandleWorldSwitchInputBuildModeShortcut obj5 = component as IHandleWorldSwitchInputBuildModeShortcut;
			OnWorldSwitchBuildModeShortcut += obj5.HandleWorldSwitchInputBuildModeShortcut;
		}
		if (component is IHandleWorldSwitchInputMainMenuModeShortcut)
		{
			IHandleWorldSwitchInputMainMenuModeShortcut obj6 = component as IHandleWorldSwitchInputMainMenuModeShortcut;
			OnWorldSwitchMainMenuShortcut += obj6.HandleWorldSwitchInputMainMenuModeShortcut;
		}
		if (component is IHandleWorldSwitchInputBrawlShortcut)
		{
			IHandleWorldSwitchInputBrawlShortcut obj7 = component as IHandleWorldSwitchInputBrawlShortcut;
			OnWorldSwitchBrawlMatchShortcut += obj7.HandleWorldSwitchInputBrawlShortcut;
		}
	}

	public void UnregisterComponent(IInputComponent component)
	{
		if (component is IHandleWorldSwitchInput)
		{
			IHandleWorldSwitchInput obj = component as IHandleWorldSwitchInput;
			OnWorldSwitchInput -= obj.HandleWorldSwitchInput;
		}
		if (component is IHandleWorldSwitchInputPracticeModeShortcut)
		{
			IHandleWorldSwitchInputPracticeModeShortcut obj2 = component as IHandleWorldSwitchInputPracticeModeShortcut;
			OnWorldSwitchInputPracticeModeShortcut -= obj2.HandleWorldSwitchInputPracticeModeShortcut;
		}
		if (component is IHandleWorldSwitchInputTestModeShortcut)
		{
			IHandleWorldSwitchInputTestModeShortcut obj3 = component as IHandleWorldSwitchInputTestModeShortcut;
			OnWorldSwitchInputTestModeShortcut -= obj3.HandleWorldSwitchInputTestModeShortcut;
		}
		if (component is IHandleWorldSwitchInputQuickPlayModeShortcut)
		{
			IHandleWorldSwitchInputQuickPlayModeShortcut obj4 = component as IHandleWorldSwitchInputQuickPlayModeShortcut;
			OnWorldSwitchQuickPlayModeShortcut -= obj4.HandleWorldSwitchInputQuickPlayModeShortcut;
		}
		if (component is IHandleWorldSwitchInputBuildModeShortcut)
		{
			IHandleWorldSwitchInputBuildModeShortcut obj5 = component as IHandleWorldSwitchInputBuildModeShortcut;
			OnWorldSwitchBuildModeShortcut -= obj5.HandleWorldSwitchInputBuildModeShortcut;
		}
		if (component is IHandleWorldSwitchInputMainMenuModeShortcut)
		{
			IHandleWorldSwitchInputMainMenuModeShortcut obj6 = component as IHandleWorldSwitchInputMainMenuModeShortcut;
			OnWorldSwitchMainMenuShortcut -= obj6.HandleWorldSwitchInputMainMenuModeShortcut;
		}
		if (component is IHandleWorldSwitchInputBrawlShortcut)
		{
			IHandleWorldSwitchInputBrawlShortcut obj7 = component as IHandleWorldSwitchInputBrawlShortcut;
			OnWorldSwitchBrawlMatchShortcut -= obj7.HandleWorldSwitchInputBrawlShortcut;
		}
	}

	public void Execute()
	{
		if (InputRemapper.Instance.GetButtonDown("Drive robot"))
		{
			this.OnWorldSwitchInput(obj: true);
		}
		else if (InputRemapper.Instance.GetButtonUp("Drive robot"))
		{
			this.OnWorldSwitchInput(obj: false);
		}
		if (InputRemapper.Instance.GetButtonDown("Single Player"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.WorldSwitchingInputAxis, 0))
			{
				this.OnWorldSwitchInputPracticeModeShortcut();
			}
		}
		else if (InputRemapper.Instance.GetButtonDown("Test Robot"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.WorldSwitchingInputAxis, 1))
			{
				this.OnWorldSwitchInputTestModeShortcut();
			}
		}
		else if (InputRemapper.Instance.GetButtonDown("Quick Play"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.WorldSwitchingInputAxis, 4))
			{
				this.OnWorldSwitchQuickPlayModeShortcut();
			}
		}
		else if (InputRemapper.Instance.GetButtonDown("Edit robot"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.WorldSwitchingInputAxis, 2))
			{
				this.OnWorldSwitchBuildModeShortcut();
			}
		}
		else if (InputRemapper.Instance.GetButtonDown("Select robot"))
		{
			if (inputActionMask.InputIsAvailable(UserInputCategory.WorldSwitchingInputAxis, 3))
			{
				this.OnWorldSwitchMainMenuShortcut();
			}
		}
		else if (InputRemapper.Instance.GetButtonDown("Brawl") && inputActionMask.InputIsAvailable(UserInputCategory.WorldSwitchingInputAxis, 4))
		{
			this.OnWorldSwitchBrawlMatchShortcut();
		}
	}
}
