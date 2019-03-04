using Simulation;
using Svelto.Command;
using Svelto.ES.Legacy;
using Svelto.IoC;

namespace Mothership
{
	internal class DummyTestModeScreenDisplay : IDummyTestModeScreenDisplay, IGUIDisplay, IComponent
	{
		[Inject]
		public WorldSwitching worldSwitching
		{
			private get;
			set;
		}

		[Inject]
		public ICommandFactory commandFactory
		{
			protected get;
			set;
		}

		[Inject]
		public IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		public EnterBattleChecker enterBattleChecker
		{
			protected get;
			set;
		}

		public GuiScreens screenType => GuiScreens.TestMode;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => false;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			return false;
		}

		public virtual GUIShowResult Show()
		{
			if (enterBattleChecker.IsMachineValidForBattle())
			{
				ExecuteSwitchCommand();
				return GUIShowResult.Showed;
			}
			return GUIShowResult.NotShowedSlim;
		}

		public bool Hide()
		{
			return true;
		}

		protected void ExecuteSwitchCommand()
		{
			SwitchWorldDependency dependency = new SwitchWorldDependency("TestRobot", isRanked_: false, isBrawl_: false, isCustomGame_: false, GameModeType.TestMode);
			commandFactory.Build<SwitchToTestPlanetCommand>().Inject(dependency).Execute();
		}

		private void ManuallyHideScreen()
		{
			guiInputController.CloseCurrentScreen();
		}
	}
}
