using Svelto.IoC;
using Svelto.Tasks;
using System.Collections;
using UnityEngine;

namespace Login
{
	internal class IntroVideoController : ISplashLoginDialogController
	{
		private IntroVideoView _view;

		[Inject]
		private IntroAnimationsSequenceEventObservable introVideoEventObservable
		{
			get;
			set;
		}

		public void SetView(ISplashLoginDialogView view)
		{
			_view = (view as IntroVideoView);
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
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.StopVideo)
				{
					_view.StopVideo();
				}
				if (splashLoginGUIMessage.Message == SplashLoginGUIMessageType.PlayVideo)
				{
					_view.PlayVideo();
					TaskRunner.get_Instance().Run(PollForVideoEndAfterPlaying());
				}
			}
		}

		private IEnumerator PollForVideoEndAfterPlaying()
		{
			while (_view.IsPlaying())
			{
				yield return null;
			}
			_view.StopVideo();
			IntroAnimationsSequenceEventCode eventCode = IntroAnimationsSequenceEventCode.VideoHasFinishedPlaying;
			introVideoEventObservable.Dispatch(ref eventCode);
		}

		public void Close()
		{
			_view.StopVideo();
		}

		public object GetEntry(SplashLoginEntryField fieldToGet)
		{
			return null;
		}
	}
}
