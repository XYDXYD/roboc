using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;

namespace Mothership.DailyQuest
{
	internal class DailyQuestDisplay : IGUIDisplay, IComponent
	{
		[Inject]
		private DailyQuestPresenter presenter
		{
			get;
			set;
		}

		public GuiScreens screenType => GuiScreens.DailyQuestScreen;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyGUINoSwitching;

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
			return presenter.Show(enable: false);
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
