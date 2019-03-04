using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using Svelto.UI.Comms.SignalChain;
using System.Collections;
using UnityEngine;

namespace Login
{
	public class WidgetNewsletterView : MonoBehaviour, ISplashLoginDialogView
	{
		[SerializeField]
		private UIInput usernameEntryField;

		[SerializeField]
		private UILabel emailNotValid;

		[Range(1f, 5f)]
		[SerializeField]
		private float intervalHideInvalidEmailLabel = 3f;

		private UILabel _emailNotValidLabel;

		private WidgetNewsletterController _widgetNewsletterController;

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

		public WidgetNewsletterView()
			: this()
		{
		}

		public void InjectController(ISplashLoginDialogController splashLoginDialogController)
		{
			_widgetNewsletterController = (WidgetNewsletterController)splashLoginDialogController;
		}

		public void DestroySelf()
		{
			Object.Destroy(this.get_gameObject());
		}

		internal void ShowEmailNotValidLabel()
		{
			_emailNotValidLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strIncentiveInvalidEmail"));
			emailNotValid.get_gameObject().SetActive(true);
			TaskRunner.get_Instance().Run(HideInvalidEmaillabel());
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

		private IEnumerator HideInvalidEmaillabel()
		{
			if (emailNotValid != null && emailNotValid.get_gameObject().get_activeInHierarchy())
			{
				yield return (object)new WaitForSecondsEnumerator(intervalHideInvalidEmailLabel);
				if (emailNotValid != null)
				{
					emailNotValid.get_gameObject().SetActive(false);
				}
			}
		}
	}
}
