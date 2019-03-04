using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System.Collections.Generic;
using UnityEngine;

namespace Mothership.DailyQuest
{
	public class QuestProgressionView : MonoBehaviour, IInitialize, IChainListener, IChainRoot
	{
		[SerializeField]
		private QuestProgressionItem[] _questItems;

		[SerializeField]
		private string[] _questNames;

		[SerializeField]
		private Texture2D[] _questImages;

		[SerializeField]
		private UILabel _totalExperienceAmount;

		[SerializeField]
		private UILabel _totalRobitsAmount;

		private const string EARNINGS_FORMAT = "N0";

		[Inject]
		internal QuestProgressionPresenter presenter
		{
			private get;
			set;
		}

		public QuestProgressionView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
			presenter.SetUpSpriteReferences(_questNames, _questImages);
		}

		public void Listen(object message)
		{
			presenter.HandleMessage(message);
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public bool IsActive()
		{
			return this.get_gameObject().get_activeInHierarchy();
		}

		public void UpdateQuestLayout(PlayerDailyQuestsData playerDailyQuestsData, List<Quest> seenQuests, Dictionary<string, Texture2D> sprites, bool hasPremium)
		{
			UpdateQuestSlots(playerDailyQuestsData, seenQuests, sprites, hasPremium);
			UpdateEarningsLabels(playerDailyQuestsData, hasPremium);
		}

		private void UpdateQuestSlots(PlayerDailyQuestsData playerDailyQuestsData, List<Quest> seenQuests, Dictionary<string, Texture2D> sprites, bool hasPremium)
		{
			for (int i = 0; i < _questItems.Length; i++)
			{
				QuestProgressionItem questProgressionItem = _questItems[i];
				if (i < playerDailyQuestsData.completedQuests.Count)
				{
					Quest quest = playerDailyQuestsData.completedQuests[i];
					questProgressionItem.UpdateData(quest, sprites[quest.questID], hasPremium);
					continue;
				}
				int num = i - playerDailyQuestsData.completedQuests.Count;
				if (num < seenQuests.Count)
				{
					Quest quest2 = seenQuests[num];
					questProgressionItem.UpdateData(quest2, sprites[quest2.questID], hasPremium);
				}
				else
				{
					questProgressionItem.UpdateData(null, null, hasPremium: false);
				}
			}
		}

		private void UpdateEarningsLabels(PlayerDailyQuestsData playerDailyQuestsData, bool hasPremium)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < playerDailyQuestsData.completedQuests.Count; i++)
			{
				Quest quest = playerDailyQuestsData.completedQuests[i];
				num += ((!hasPremium) ? quest.xp : quest.premiumXP);
				num2 += ((!hasPremium) ? quest.robits : quest.premiumRobits);
			}
			_totalExperienceAmount.set_text(num.ToString("N0"));
			_totalRobitsAmount.set_text(num2.ToString("N0"));
		}
	}
}
