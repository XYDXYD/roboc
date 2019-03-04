using Svelto.IoC;
using UnityEngine;
using Utility;

namespace Login
{
	internal class SplashLoginErrorDialogController : ISplashLoginDialogController
	{
		private SplashLoginErrorDialogConfiguration _configuration;

		private SplashLoginErrorDialogView _view;

		[Inject]
		private SplashLoginHierarchyChangedObservable hierarchyChangedObserveable
		{
			get;
			set;
		}

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as SplashLoginErrorDialogView);
		}

		public void SetConfiguration(SplashLoginErrorDialogConfiguration configuration)
		{
			_configuration = configuration;
			_view.SetTitleText(configuration.header);
			_view.SetBodyText(configuration.body);
			if (configuration.numButtons == 1)
			{
				_view.SetOneButtonLayout(configuration.buttonTypes[0]);
			}
			else
			{
				_view.SetTwoButtonLayout(configuration.buttonTypes[0], configuration.buttonTypes[1]);
			}
		}

		public void HandleButtonClick(SplashLoginGUIMessageType msgType)
		{
			bool flag = false;
			for (int i = 0; i < _configuration.buttonTypes.Length; i++)
			{
				if (_view.GetMessageTypeForButtonType(_configuration.buttonTypes[i]) == msgType)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return;
			}
			if (_configuration.OnActionCallback != null)
			{
				_configuration.OnActionCallback(msgType);
				Close();
				return;
			}
			if (msgType == SplashLoginGUIMessageType.ErrorButtonQuitPressed)
			{
				Console.Log("Application will quit now");
				Application.Quit();
			}
			if (msgType == SplashLoginGUIMessageType.ErrorButtonRetryPressed)
			{
			}
			Close();
		}

		public void Close()
		{
			_view.DestroySelf();
			_view = null;
			hierarchyChangedObserveable.Dispatch();
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			return null;
		}
	}
}
