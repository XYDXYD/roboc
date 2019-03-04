using Svelto.IoC;
using System;

namespace Login
{
	internal class PromptToLinkWithSteamDialogController : ISplashLoginDialogController
	{
		public enum PromptToLinkWithSteamButtonChoice
		{
			None,
			ButtonOK,
			ButtonCancel
		}

		private PromptToLinkWithSteamDialogView _view;

		[Inject]
		private SplashLoginHierarchyChangedObserver hierarchyChangedObserver
		{
			get;
			set;
		}

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as PromptToLinkWithSteamDialogView);
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

		public void HandleMessage(object message)
		{
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			return null;
		}
	}
}
