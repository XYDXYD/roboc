using Svelto.IoC;
using System;

namespace Login
{
	public class WidgetEmailSentController : ISplashLoginDialogController
	{
		private WidgetEmailSentView _sentView;

		[Inject]
		private SplashLoginHierarchyChangedObserver hierarchyChangedObserver
		{
			get;
			set;
		}

		public void Close()
		{
			hierarchyChangedObserver.RemoveAction((Action)RebuildSignalChain);
			_sentView.DestroySelf();
			_sentView = null;
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			throw new NotImplementedException();
		}

		public void SetView(ISplashLoginDialogView splashLoginDialogView)
		{
			_sentView = (splashLoginDialogView as WidgetEmailSentView);
			hierarchyChangedObserver.AddAction((Action)RebuildSignalChain);
		}

		private void RebuildSignalChain()
		{
			_sentView.RebuildSignalChain();
		}
	}
}
