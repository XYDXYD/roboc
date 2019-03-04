using Svelto.IoC;
using System;

namespace Login
{
	internal class DialogLoginConfirmQuitController : ISplashLoginDialogController
	{
		private DialogLoginConfirmQuitView _view;

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
			throw new ArgumentOutOfRangeException("fieldToGet", fieldToGet, null);
		}

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as DialogLoginConfirmQuitView);
			hierarchyChangedObserver.AddAction((Action)RebuildSignalChain);
		}

		private void RebuildSignalChain()
		{
			_view.RebuildSignalChain();
		}
	}
}
