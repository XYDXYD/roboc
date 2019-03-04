using System;
using UnityEngine;

namespace Login
{
	internal class AccountUnverifiedDialog : MonoBehaviour
	{
		public UIPanel panel;

		private Action _onClose;

		public AccountUnverifiedDialog()
			: this()
		{
		}

		public void Show(Action onClose)
		{
			_onClose = onClose;
			panel.get_gameObject().SetActive(true);
			this.get_gameObject().SetActive(true);
		}

		public void OnResendEmailClick()
		{
			Application.OpenURL(string.Format("http://robocraftgame.com/recover-password.php?lang={0}", StringTableBase<StringTable>.Instance.GetString("strLocale")));
			Hide();
		}

		public void OnCancelClick()
		{
			Hide();
		}

		private void Hide()
		{
			panel.get_gameObject().SetActive(false);
			this.get_gameObject().SetActive(false);
			_onClose();
		}
	}
}
