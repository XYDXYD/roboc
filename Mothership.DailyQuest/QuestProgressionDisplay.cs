using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;

namespace Mothership.DailyQuest
{
	internal class QuestProgressionDisplay : IGUIDisplay, IComponent
	{
		[Inject]
		internal QuestProgressionPresenter presenter
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.QuestProgressionScreen;

		public TopBarStyle topBarStyle => TopBarStyle.OffScreen;

		public ShortCutMode shortCutMode => ShortCutMode.NoKeyboardInputAllowed;

		public bool isScreenBlurred => true;

		public bool hasBackground => false;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.HideAll;

		public GUIShowResult Show()
		{
			presenter.Show();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			presenter.Hide();
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
