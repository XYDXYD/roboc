using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Login
{
	public class WidgetUserValidationResendEmailView : MonoBehaviour, ISplashLoginDialogView
	{
		[SerializeField]
		private UIInput usernameEntryField;

		[SerializeField]
		private UILabel emailNotValid;

		[Range(1f, 5f)]
		[SerializeField]
		private float intervalHideInvalidEmailLabel = 3f;

		private UILabel _emailNotValidLabel;

		private WidgetUserValidationResendEmailController _controller;

		internal UILabel emailNotValidUILabel => emailNotValid;

		internal float intervalHideInvalidEmailUILabel => intervalHideInvalidEmailLabel;

		internal string userNameEntry
		{
			get
			{
				return usernameEntryField.get_value();
			}
			set
			{
				usernameEntryField.set_value(value);
			}
		}

		public WidgetUserValidationResendEmailView()
			: this()
		{
		}

		public void InjectController(ISplashLoginDialogController splashLoginDialogController)
		{
			_controller = (WidgetUserValidationResendEmailController)splashLoginDialogController;
		}

		public void DestroySelf()
		{
			Object.Destroy(this.get_gameObject());
		}

		internal void RebuildSignalChain()
		{
			GameObject gameObject = this.get_gameObject();
			while (gameObject.get_transform().get_parent() != null)
			{
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
				if (gameObject.GetComponent<IChainRoot>() != null)
				{
					break;
				}
			}
		}

		private void Start()
		{
			_emailNotValidLabel = emailNotValid.GetComponent<UILabel>();
		}
	}
}
