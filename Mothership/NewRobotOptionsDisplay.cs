using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;

namespace Mothership
{
	internal class NewRobotOptionsDisplay : IGUIDisplay, IComponent
	{
		[Inject]
		private NewRobotOptionsPresenter presenter
		{
			get;
			set;
		}

		public GuiScreens screenType => GuiScreens.NewRobotOptionsScreen;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public GUIShowResult Show()
		{
			presenter.Show(enable: true);
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			presenter.Show(enable: false);
			return true;
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			return presenter.IsActive();
		}
	}
}
