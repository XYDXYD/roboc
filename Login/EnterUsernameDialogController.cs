using Svelto.IoC;
using System;

namespace Login
{
	internal class EnterUsernameDialogController : ISplashLoginDialogController
	{
		private UsernameValidDuringEntryStatus _entryValidStatus;

		private EnterUsernameDialogView _view;

		[Inject]
		private SplashLoginHierarchyChangedObserver hierarchyChangedObserver
		{
			get;
			set;
		}

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as EnterUsernameDialogView);
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

		public void SetTextEntryIsValid(UsernameValidDuringEntryStatus status, string message)
		{
			_entryValidStatus = status;
			_view.ShowTextEntryInvalidMessage(status, message);
		}

		public void SetTextEntryIsValid(UsernameValidDuringEntryStatus status)
		{
			_entryValidStatus = status;
			switch (_entryValidStatus)
			{
			default:
				_view.HideTextEntryInvalidMessage();
				break;
			case UsernameValidDuringEntryStatus.EntryTooLong:
			case UsernameValidDuringEntryStatus.EntryTooShort:
			case UsernameValidDuringEntryStatus.EntryContainsProfanity:
			case UsernameValidDuringEntryStatus.NameAlreadyUsed:
			case UsernameValidDuringEntryStatus.EntryIsInvalidSomeOtherReasons:
				_view.ShowTextEntryInvalidMessage(_entryValidStatus);
				break;
			}
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			if (fieldToGet == SplashLoginEntryField.Field_Identifier)
			{
				return _view.IdentifierEntry;
			}
			return null;
		}
	}
}
