using Svelto.Context;
using Svelto.IoC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership.DailyQuest
{
	internal class QuestProgressionPresenter : IWaitForFrameworkInitialization, IWaitForFrameworkDestruction
	{
		private QuestProgressionView _view;

		private readonly Dictionary<string, Texture2D> _sprites = new Dictionary<string, Texture2D>();

		private readonly List<Quest> _seenQuests = new List<Quest>();

		[Inject]
		internal IGUIInputControllerMothership guiInputController
		{
			private get;
			set;
		}

		[Inject]
		internal DailyQuestController dailyQuestController
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

		public void OnFrameworkInitialized()
		{
			premiumMembership.onSubscriptionActivated += HandlePremiumSubscriptionActivated;
		}

		public void OnFrameworkDestroyed()
		{
			premiumMembership.onSubscriptionActivated -= HandlePremiumSubscriptionActivated;
		}

		public void SetView(QuestProgressionView view)
		{
			_view = view;
		}

		public void SetUpSpriteReferences(string[] questNames, Texture2D[] questImages)
		{
			for (int i = 0; i < questNames.Length; i++)
			{
				_sprites.Add(questNames[i], questImages[i]);
			}
		}

		public void Show()
		{
			UpdateSeenQuests(dailyQuestController.playerQuestData.playerQuests);
			_view.UpdateQuestLayout(dailyQuestController.playerQuestData, _seenQuests, _sprites, premiumMembership.hasSubscription);
			_view.Show();
		}

		public void Hide()
		{
			_view.Hide();
		}

		public bool IsActive()
		{
			return _view.IsActive();
		}

		public void HandleMessage(object message)
		{
			if (message is ButtonType)
			{
				ButtonType buttonType = (ButtonType)message;
				ProcessButtonClick(buttonType);
			}
		}

		public bool HasQuestProgression()
		{
			return dailyQuestController.playerQuestData.completedQuests.Count > 0;
		}

		private void ProcessButtonClick(ButtonType buttonType)
		{
			if (buttonType == ButtonType.Close)
			{
				guiInputController.CloseCurrentScreen();
				return;
			}
			throw new Exception("No logic implemented for button type: " + buttonType);
		}

		private void HandlePremiumSubscriptionActivated(TimeSpan premiumDuration)
		{
			if (premiumDuration.TotalSeconds > 0.0 && dailyQuestController.playerQuestData != null)
			{
				UpdateSeenQuests(dailyQuestController.playerQuestData.playerQuests);
				_view.UpdateQuestLayout(dailyQuestController.playerQuestData, _seenQuests, _sprites, premiumMembership.hasSubscription);
			}
		}

		private void UpdateSeenQuests(List<Quest> quests)
		{
			_seenQuests.Clear();
			for (int i = 0; i < quests.Count; i++)
			{
				Quest quest = quests[i];
				if (quest.seen)
				{
					_seenQuests.Add(quest);
				}
			}
		}
	}
}
