using Svelto.IoC;
using System;

namespace Login
{
	internal class EnterUsernameAndPasswordDialogController : ISplashLoginDialogController
	{
		private EnterUsernameAndPasswordDialogView _view;

		[Inject]
		private SplashLoginHierarchyChangedObserver hierarchyChangedObserver
		{
			get;
			set;
		}

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as EnterUsernameAndPasswordDialogView);
			hierarchyChangedObserver.AddAction((Action)OnHierarchyChanged);
		}

		private void OnHierarchyChanged()
		{
			_view.RebuildSignalChain();
		}

		public void Close()
		{
			hierarchyChangedObserver.RemoveAction((Action)OnHierarchyChanged);
			_view.DestroySelf();
			_view = null;
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			switch (fieldToGet)
			{
			case SplashLoginEntryField.Field_Identifier:
				return _view.UserNameEntry;
			case SplashLoginEntryField.Field_Password:
				return _view.PasswordEntry;
			case SplashLoginEntryField.Field_RememberMeCheckBox:
				return _view.RememberMeValue;
			default:
				return null;
			}
		}
	}
}
