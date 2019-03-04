using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using UnityEngine;

namespace Login
{
	internal class FadeInOutImageController : ISplashLoginDialogController
	{
		private FadeInOutImageView _view;

		[Inject]
		private IntroAnimationsSequenceEventObservable introVideoEventObservable
		{
			get;
			set;
		}

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as FadeInOutImageView);
		}

		public void Initialise()
		{
			Application.set_targetFrameRate(60);
		}

		public void HandleMessage(object message)
		{
			if (message is SplashLoginGUIMessage)
			{
				SplashLoginGUIMessage splashLoginGUIMessage = message as SplashLoginGUIMessage;
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.HideImage)
				{
					_view.Hide();
				}
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.ShowImage)
				{
					_view.Show();
					TaskRunner.get_Instance().Run(PollForFadeOutAfterShow());
				}
			}
		}

		private IEnumerator PollForFadeOutAfterShow()
		{
			while (_view.IsFading())
			{
				yield return null;
			}
			_view.Hide();
			IntroAnimationsSequenceEventCode eventCode = IntroAnimationsSequenceEventCode.ImageHasFinishedShowing;
			introVideoEventObservable.Dispatch(ref eventCode);
		}

		public void Close()
		{
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			return null;
		}
	}
}
