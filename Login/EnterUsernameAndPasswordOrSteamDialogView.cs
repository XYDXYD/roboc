using Svelto.UI.Comms.SignalChain;
using System;
using UnityEngine;

namespace Login
{
	internal class EnterUsernameAndPasswordOrSteamDialogView : MonoBehaviour, ISplashLoginDialogView
	{
		[SerializeField]
		private UIInput usernameEntryField;

		[SerializeField]
		private UIInput passwordEntryField;

		[SerializeField]
		private UIToggle rememberMeUIToggle;

		[SerializeField]
		private GameObject SteamButtonContainer;

		private SignalChain _signal;

		private EnterUsernameAndPasswordOrSteamDialogController _controller;

		public string UserNameEntry => usernameEntryField.get_value();

		public string PasswordEntry => passwordEntryField.get_value();

		public bool RememberMeValue => rememberMeUIToggle.get_value();

		public EnterUsernameAndPasswordOrSteamDialogView()
			: this()
		{
		}

		void ISplashLoginDialogView.InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as EnterUsernameAndPasswordOrSteamDialogController);
		}

		public unsafe void Start()
		{
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Expected O, but got Unknown
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Expected O, but got Unknown
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			RebuildSignalChain();
			usernameEntryField.set_value(StringUtil.Decrypt(PlayerPrefs.GetString("username")));
			passwordEntryField.set_value(StringUtil.Decrypt(PlayerPrefs.GetString("password")));
			rememberMeUIToggle.set_value(Convert.ToBoolean(PlayerPrefs.GetInt("rememberme", 0)));
			usernameEntryField.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			passwordEntryField.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
		}

		public void RebuildSignalChain()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			GameObject gameObject = this.get_gameObject();
			while (gameObject.get_transform().get_parent() != null)
			{
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
				if (gameObject.GetComponent<IChainRoot>() != null)
				{
					break;
				}
			}
			_signal = new SignalChain(gameObject.get_transform());
		}

		public void DestroySelf()
		{
			this.get_gameObject().get_transform().set_parent(null);
			Object.Destroy(this.get_gameObject());
		}

		private void HandleOnUserNameFieldChangeEvent()
		{
			_signal.DeepBroadcast(typeof(SplashLoginGUIMessage), (object)new SplashLoginGUIMessage(SplashLoginGUIMessageType.EnterUsernameTextEntryFieldChanged));
		}

		private void HandleOnPasswordFieldChangeEvent()
		{
			_signal.DeepBroadcast(typeof(SplashLoginGUIMessage), (object)new SplashLoginGUIMessage(SplashLoginGUIMessageType.EnterPasswordTextEntryFieldChanged));
		}
	}
}
