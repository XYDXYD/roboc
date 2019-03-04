using Svelto.Tasks;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Login
{
	internal class FadeInOutImageView : MonoBehaviour, IChainListener, ISplashLoginDialogView
	{
		public UIPanel panel;

		public float showDuration;

		public float additionalFadeDuration;

		private bool _fading;

		private FadeInOutImageController _controller;

		public FadeInOutImageView()
			: this()
		{
		}

		public void InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as FadeInOutImageController);
		}

		public void Initialise()
		{
			this.get_gameObject().SetActive(true);
			panel.get_gameObject().SetActive(false);
			GameObject gameObject = this.get_gameObject();
			while (gameObject.get_transform().get_parent() != null)
			{
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
			}
		}

		public void Show()
		{
			Console.Log("Show image triggered");
			this.get_gameObject().SetActive(true);
			panel.get_gameObject().SetActive(true);
			_fading = true;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)Fade);
		}

		public void Hide()
		{
			Console.Log("Hide image triggered");
			this.get_gameObject().SetActive(false);
		}

		public bool IsFading()
		{
			return _fading;
		}

		private IEnumerator Fade()
		{
			yield return FadeIn();
			yield return (object)new WaitForSeconds(showDuration);
			yield return FadeOut();
			_fading = false;
		}

		private IEnumerator FadeOut()
		{
			while (panel.get_alpha() > 0f)
			{
				UIPanel obj = panel;
				obj.set_alpha(obj.get_alpha() - Time.get_deltaTime() / additionalFadeDuration);
				yield return null;
			}
		}

		private IEnumerator FadeIn()
		{
			while (panel.get_alpha() < 1f)
			{
				UIPanel obj = panel;
				obj.set_alpha(obj.get_alpha() + Time.get_deltaTime() / additionalFadeDuration);
				yield return null;
			}
		}

		void IChainListener.Listen(object message)
		{
			_controller.HandleMessage(message);
		}

		public void DestroySelf()
		{
			Hide();
		}
	}
}
