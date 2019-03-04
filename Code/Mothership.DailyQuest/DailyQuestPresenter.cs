using Services.Analytics;
using Svelto.Context;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;

namespace Mothership.DailyQuest
{
	internal class DailyQuestPresenter : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private DailyQuestView _view;

		[Inject]
		internal DailyQuestController dailyQuestController
		{
			private get;
			set;
		}

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal PremiumMembership premiumMembership
		{
			private get;
			set;
		}

		[Inject]
		internal IAnalyticsRequestFactory analyticsRequestFactory
		{
			private get;
			set;
		}

		public void OnFrameworkInitialized()
		{
			premiumMembership.onSubscriptionActivated += HandlePremiumMembershipActivated;
		}

		public void OnFrameworkDestroyed()
		{
			premiumMembership.onSubscriptionActivated -= HandlePremiumMembershipActivated;
		}

		public bool Show(bool enable)
		{
			_view.Show(enable);
			if (enable)
			{
				TaskRunner.get_Instance().Run(ShowPlayerQuests());
			}
			return true;
		}

		public bool IsActive()
		{
			return _view.get_gameObject().get_activeSelf();
		}

		public unsafe void SetView(DailyQuestView view)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			_view = view;
			_view.AddBackButtonEvent(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void HandlePlayerClicked(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				ProcessButtonClick(buttonType);
			}
			else if (message is string)
			{
				string questID = (string)message;
				TaskRunner.get_Instance().Run(ReplacePlayerQuests(questID));
			}
		}

		public Texture2D GetQuestImage(string questID)
		{
			return _view.GetTexture(questID);
		}

		private IEnumerator ShowPlayerQuests()
		{
			if (dailyQuestController.playerQuestData == null)
			{
				yield return dailyQuestController.LoadData();
			}
			_view.LayoutQuests(dailyQuestController.playerQuestData, premiumMembership.hasSubscription);
		}

		private IEnumerator ReplacePlayerQuests(string questID)
		{
			yield return dailyQuestController.ReplaceQuest(questID);
			yield return dailyQuestController.LoadData();
			yield return ShowPlayerQuests();
		}

		private void HandlePremiumMembershipActivated(TimeSpan premiumTimeSpan)
		{
			if (premiumTimeSpan.TotalSeconds > 0.0)
			{
				_view.LayoutQuests(dailyQuestController.playerQuestData, premiumMembership.hasSubscription);
			}
		}

		private void ProcessButtonClick(ButtonType buttonType)
		{
			switch (buttonType)
			{
			case ButtonType.Close:
				guiInputController.CloseCurrentScreen();
				break;
			case ButtonType.PurchasePremium:
				PurchaseFunnelHelper.SendEvent(analyticsRequestFactory, "StoreEntered", "DailyQuests", startsNewChain: true);
				guiInputController.ShowScreen(GuiScreens.RealMoneyStoreScreen);
				break;
			}
		}
	}
}
