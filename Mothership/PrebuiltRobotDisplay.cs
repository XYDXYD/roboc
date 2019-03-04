using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;

namespace Mothership
{
	internal class PrebuiltRobotDisplay : IGUIDisplay, IComponent
	{
		[Inject]
		private PrebuiltRobotPresenter presenter
		{
			get;
			set;
		}

		public GuiScreens screenType => GuiScreens.PrebuiltRobotScreen;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyEsc;

		public bool isScreenBlurred => false;

		public bool hasBackground => true;

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
