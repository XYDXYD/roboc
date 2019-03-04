using Simulation;
using Svelto.Context;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using System;

namespace Mothership
{
	internal class BundleAwardController : IGUIDisplay, IWaitForFrameworkDestruction, IWaitForFrameworkInitialization, IComponent
	{
		public BundleAwardDialog Dialog;

		GuiScreens IGUIDisplay.screenType
		{
			get
			{
				return GuiScreens.BundleAwardDialog;
			}
		}

		TopBarStyle IGUIDisplay.topBarStyle
		{
			get
			{
				return TopBarStyle.TimerHidden;
			}
		}

		ShortCutMode IGUIDisplay.shortCutMode
		{
			get
			{
				return ShortCutMode.OnlyEsc;
			}
		}

		bool IGUIDisplay.isScreenBlurred
		{
			get
			{
				return false;
			}
		}

		bool IGUIDisplay.hasBackground
		{
			get
			{
				return false;
			}
		}

		public bool doesntHideOnSwitch => false;

		[Inject]
		internal IGameObjectFactory _gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership _guiInputController
		{
			private get;
			set;
		}

		public HudStyle battleHudStyle => HudStyle.Full;

		public event Action OnBundleAwardDialogShown = delegate
		{
		};

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			Dialog = null;
		}

		GUIShowResult IGUIDisplay.Show()
		{
			if (Dialog == null)
			{
				Dialog = _gameObjectFactory.Build("BundleAwardDialog").GetComponent<BundleAwardDialog>();
			}
			Dialog.Show();
			this.OnBundleAwardDialogShown();
			return GUIShowResult.Showed;
		}

		public bool Hide()
		{
			Dialog.Hide();
			return true;
		}

		public void EnableBackground(bool enable)
		{
		}

		public bool IsActive()
		{
			if (Dialog == null)
			{
				return false;
			}
			return Dialog.get_enabled();
		}

		void IWaitForFrameworkInitialization.OnFrameworkInitialized()
		{
		}

		public unsafe void SetDialog(BundleAwardDialog dialog)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Expected O, but got Unknown
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Expected O, but got Unknown
			Dialog = dialog;
			dialog.Submit.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		public void Bundles(string bundles)
		{
			if (Dialog == null)
			{
				Dialog = _gameObjectFactory.Build("BundleAwardDialog").GetComponent<BundleAwardDialog>();
			}
			Hide();
			Dialog.Bundles.set_text(bundles);
		}
	}
}
