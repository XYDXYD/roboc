using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Login
{
	internal class PromptToLinkWithSteamDialogView : MonoBehaviour, ISplashLoginDialogView, IChainListener
	{
		[SerializeField]
		private UIInput textEntryField;

		private SignalChain _signal;

		private PromptToLinkWithSteamDialogController _controller;

		public PromptToLinkWithSteamDialogView()
			: this()
		{
		}

		void ISplashLoginDialogView.InjectController(ISplashLoginDialogController controller)
		{
			_controller = (controller as PromptToLinkWithSteamDialogController);
		}

		public void Start()
		{
			RebuildSignalChain();
		}

		public void RebuildSignalChain()
		{
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Expected O, but got Unknown
			GameObject gameObject = this.get_gameObject();
			while (gameObject.get_transform().get_parent() != null)
			{
				gameObject = gameObject.get_transform().get_parent().get_gameObject();
				if (gameObject.GetComponent<IChainRoot>() != null)
				{
					break;
				}
			}
			_signal = new SignalChain(gameObject.get_transform());
		}

		public void DestroySelf()
		{
			Object.Destroy(this.get_gameObject());
		}

		public void Listen(object message)
		{
		}
	}
}
