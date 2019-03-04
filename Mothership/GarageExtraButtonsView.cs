using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using UnityEngine;

namespace Mothership
{
	internal sealed class GarageExtraButtonsView : MonoBehaviour, IInitialize, IChainListener
	{
		[SerializeField]
		private GameObject _achievementsButton;

		[SerializeField]
		private GameObject _wikiButton;

		[SerializeField]
		private GameObject _feedbackButton;

		[SerializeField]
		private GameObject _customerServiceButton;

		[Inject]
		internal GarageExtraButtonsPresenter garage
		{
			private get;
			set;
		}

		public GarageExtraButtonsView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			garage.view = this;
		}

		public void Show()
		{
			_achievementsButton.SetActive(true);
			_wikiButton.SetActive(true);
			_feedbackButton.SetActive(true);
			_customerServiceButton.SetActive(true);
		}

		public void Listen(object message)
		{
			if (message is ButtonType)
			{
				switch ((ButtonType)message)
				{
				case ButtonType.RobotShopShowFilter:
				case ButtonType.RobotShopHideFilter:
					break;
				case ButtonType.AchievementsScreen:
					garage.OnAchievementsButtonClicked();
					break;
				case ButtonType.WikiScreen:
					garage.OnWikiButtonClicked();
					break;
				case ButtonType.FeedbackPage:
					garage.OnFeedbackButtonClicked();
					break;
				case ButtonType.CustomerServicePage:
					garage.OnCustomerServiceButtonClicked();
					break;
				}
			}
		}
	}
}
