using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Login
{
	internal class SplashLoginTooHighCCUView : MonoBehaviour, ISplashLoginDialogView, IChainListener
	{
		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UILabel bodyText;

		[SerializeField]
		private UILabel queueText;

		private SplashLoginErrorDialogController _controller;

		public SplashLoginTooHighCCUView()
			: this()
		{
		}

		public void SetQueuePosition(string queuePosition)
		{
			queueText.set_text(queuePosition);
		}

		void ISplashLoginDialogView.InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as SplashLoginErrorDialogController);
		}

		public void DestroySelf()
		{
			this.get_gameObject().get_transform().set_parent(null);
			Object.Destroy(this.get_gameObject());
		}

		public void SetTitleText(string title_)
		{
			title.set_text(title_);
		}

		public void SetBodyText(string bodyText_)
		{
			bodyText.set_text(bodyText_);
		}

		public void Listen(object message)
		{
		}
	}
}
