using Simulation;
using Svelto.ECS;
using Svelto.ES.Legacy;

namespace Game.ECS.GUI.Implementors
{
	internal class GUIDisplayImplementor : IGUIDisplay, IComponent
	{
		private readonly DispatchOnChange<bool> _isShown;

		public GuiScreens screenType
		{
			get;
			private set;
		}

		public HudStyle battleHudStyle
		{
			get;
			private set;
		}

		public bool doesntHideOnSwitch
		{
			get;
			private set;
		}

		public bool hasBackground
		{
			get;
			private set;
		}

		public bool isScreenBlurred
		{
			get;
			private set;
		}

		public ShortCutMode shortCutMode
		{
			get;
			private set;
		}

		public TopBarStyle topBarStyle
		{
			get;
			private set;
		}

		public GUIDisplayImplementor(GuiScreens screenType, HudStyle battleHudStyle, bool doesntHideOnSwitch, bool hasBackground, bool isScreenBlurred, ShortCutMode shortCutMode, TopBarStyle topBarStyle, DispatchOnChange<bool> isShownDispatcher)
		{
			this.screenType = screenType;
			this.battleHudStyle = battleHudStyle;
			this.doesntHideOnSwitch = doesntHideOnSwitch;
			this.hasBackground = hasBackground;
			this.isScreenBlurred = isScreenBlurred;
			this.shortCutMode = shortCutMode;
			this.topBarStyle = topBarStyle;
			_isShown = isShownDispatcher;
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool Hide()
		{
			_isShown.set_value(false);
			return !IsActive();
		}

		public GUIShowResult Show()
		{
			_isShown.set_value(true);
			return GUIShowResult.Showed;
		}

		public bool IsActive()
		{
			return _isShown.get_value();
		}
	}
}
