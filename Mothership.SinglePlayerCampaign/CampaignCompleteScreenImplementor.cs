using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership.SinglePlayerCampaign
{
	internal class CampaignCompleteScreenImplementor : MonoBehaviour, ICampaignCompleteScreenComponent, IChainListener
	{
		[SerializeField]
		private Animation _animation;

		[SerializeField]
		private UILabel _campaignLabel;

		[SerializeField]
		private UILabel _difficultyLabel;

		[Inject]
		public IGUIInputController guiInputController
		{
			private get;
			set;
		}

		public GameObject screenGameObject => this.get_gameObject();

		public Animation screenAnimation => _animation;

		public UILabel campaignLabel => _campaignLabel;

		public UILabel difficultyLabel => _difficultyLabel;

		public CampaignCompleteScreenImplementor()
			: this()
		{
		}

		public void Listen(object message)
		{
			if (message is ButtonType && (ButtonType)message == ButtonType.Close)
			{
				guiInputController.CloseCurrentScreen();
			}
		}
	}
}
