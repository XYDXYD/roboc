using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using UnityEngine;

namespace Login
{
	public class WidgetNewsletterResendEmailController : ISplashLoginDialogController
	{
		private WidgetNewsletterResendEmailView _view;

		private bool wasEmailAlreadySet;

		[Inject]
		private SplashLoginHierarchyChangedObserver hierarchyChangedObserver
		{
			get;
			set;
		}

		public void Close()
		{
			hierarchyChangedObserver.RemoveAction((Action)RebuildSignalChain);
			_view.DestroySelf();
			_view = null;
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			if (fieldToGet == SplashLoginEntryField.Field_Identifier)
			{
				return _view.userNameEntry;
			}
			throw new ArgumentOutOfRangeException("fieldToGet", fieldToGet, null);
		}

		public void SetView(ISplashLoginDialogView splashLoginDialogView)
		{
			_view = (splashLoginDialogView as WidgetNewsletterResendEmailView);
			hierarchyChangedObserver.AddAction((Action)RebuildSignalChain);
		}

		internal void SetEmailAddress(string emailAddress)
		{
			_view.userNameEntry = emailAddress;
			wasEmailAlreadySet = true;
		}

		internal bool WasEmailAlreadySet()
		{
			return wasEmailAlreadySet;
		}

		internal void ShowEmailNotValidLabel()
		{
			UILabel emailNotValidUILabel = _view.emailNotValidUILabel;
			GameObject gameObject = emailNotValidUILabel.get_gameObject();
			if (!gameObject.get_activeSelf())
			{
				emailNotValidUILabel.set_text(StringTableBase<StringTable>.Instance.GetString("strIncentiveInvalidEmail"));
				gameObject.SetActive(true);
				TaskRunner.get_Instance().Run(HideInvalidEmaillabel());
			}
		}

		private void RebuildSignalChain()
		{
			_view.RebuildSignalChain();
		}

		private IEnumerator HideInvalidEmaillabel()
		{
			UILabel emailNotValidLabel = _view.emailNotValidUILabel;
			GameObject emailNotValidLabelGO = emailNotValidLabel.get_gameObject();
			if (emailNotValidLabelGO.get_activeInHierarchy())
			{
				yield return (object)new WaitForSecondsEnumerator(_view.intervalHideInvalidEmailUILabel);
				if (emailNotValidLabel != null && emailNotValidLabelGO != null)
				{
					emailNotValidLabelGO.SetActive(false);
				}
			}
		}
	}
}
