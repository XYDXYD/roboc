using Simulation;
using Svelto.ES.Legacy;
using Svelto.IoC;
using System;

namespace Mothership
{
	internal sealed class RobotShopDisplay : IGUIDisplay, IComponent
	{
		[Inject]
		internal IRobotShopController robotShop
		{
			private get;
			set;
		}

		public GuiScreens screenType => GuiScreens.RobotShop;

		public TopBarStyle topBarStyle => TopBarStyle.FullScreenInterface;

		public ShortCutMode shortCutMode => ShortCutMode.OnlyGUINoSwitching;

		public bool isScreenBlurred => false;

		public bool hasBackground => true;

		public bool doesntHideOnSwitch => false;

		public HudStyle battleHudStyle => HudStyle.Full;

		public event Action OnShowRobotShop = delegate
		{
		};

		public void EnableBackground(bool enable)
		{
			robotShop.EnableBackground(enable);
		}

		public GUIShowResult Show()
		{
			GUIShowResult gUIShowResult = robotShop.Show();
			if (gUIShowResult == GUIShowResult.Showed)
			{
				this.OnShowRobotShop();
			}
			return gUIShowResult;
		}

		public bool Hide()
		{
			return robotShop.Hide();
		}

		public bool IsActive()
		{
			if (robotShop == null)
			{
				return false;
			}
			return robotShop.IsActive();
		}
	}
}
