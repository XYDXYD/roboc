using Svelto.IoC;
using Svelto.UI.Comms.SignalChain;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Mothership.DailyQuest
{
	internal class DailyQuestView : MonoBehaviour, IInitialize, IChainListener, IChainRoot
	{
		[SerializeField]
		private QuestItem[] questSlots;

		[SerializeField]
		private string[] questNames;

		[SerializeField]
		private Texture2D[] questImages;

		[SerializeField]
		private float disabledAlpha = 0.5f;

		[SerializeField]
		private UIButton backButt;

		[SerializeField]
		private QuestItem anchorQuestSlot;

		[SerializeField]
		private GameObject purchasePremiumPanel;

		[SerializeField]
		private Transform premiumAnchor;

		[SerializeField]
		private Transform nonPremiumAnchor;

		private const string FORMAT = "N0";

		private const string EMPTY = "strEmpty";

		private const bool ENABLE_PREMIUM = false;

		private StringBuilder _questProgressSB = new StringBuilder();

		private Dictionary<string, Texture2D> _sprites = new Dictionary<string, Texture2D>();

		[Inject]
		private DailyQuestPresenter presenter
		{
			get;
			set;
		}

		public DailyQuestView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void Show(bool enable)
		{
			this.get_gameObject().SetActive(enable);
		}

		public void Listen(object message)
		{
			presenter.HandlePlayerClicked(message);
		}

		public void AddBackButtonEvent(Callback callback)
		{
			EventDelegate.Add(backButt.onClick, callback);
		}

		public void LayoutQuests(PlayerDailyQuestsData data, bool hasPremium)
		{
			hasPremium = false;
			for (int i = 0; i < questSlots.Length; i++)
			{
				if (data != null && data.playerQuests.Count > i)
				{
					Quest quest = data.playerQuests[i];
					questSlots[i].SetValues(StringTableBase<StringTable>.Instance.GetString(quest.questNameStrKey), GetTexture(quest.questID), StringTableBase<StringTable>.Instance.GetString(quest.questDescStrKey), GetProgressString(quest), (float)quest.progressCount / (float)quest.targetCount, quest.xp.ToString("N0", CultureInfo.InvariantCulture), quest.premiumXP.ToString("N0", CultureInfo.InvariantCulture), quest.robits.ToString("N0", CultureInfo.InvariantCulture), quest.premiumRobits.ToString("N0", CultureInfo.InvariantCulture), (!hasPremium) ? 1f : disabledAlpha, (!hasPremium) ? disabledAlpha : 1f, data.canRemoveQuest, quest.questID);
					questSlots[i].Show(enable: true);
				}
				else
				{
					questSlots[i].SetValues(StringTableBase<StringTable>.Instance.GetString("strEmpty"), null, null, null, 0f, null, null, null, null, 0f, 0f, canReroll: false, null);
					questSlots[i].Show(enable: false);
				}
			}
			purchasePremiumPanel.SetActive(false);
			Transform anchorTransform = (!purchasePremiumPanel.get_activeInHierarchy()) ? premiumAnchor : nonPremiumAnchor;
			anchorQuestSlot.UpdateMainWidgetAnchor(anchorTransform);
		}

		private void Awake()
		{
			for (int i = 0; i < questNames.Length; i++)
			{
				_sprites.Add(questNames[i], questImages[i]);
			}
		}

		private string GetProgressString(Quest quest)
		{
			_questProgressSB.Length = 0;
			_questProgressSB.Append(StringTableBase<StringTable>.Instance.GetString("strDailyQuestsProgress"));
			_questProgressSB.Replace("{PROGRESS}", quest.progressCount.ToString());
			_questProgressSB.Replace("{TARGET}", quest.targetCount.ToString());
			return _questProgressSB.ToString();
		}

		internal Texture2D GetTexture(string questID)
		{
			if (_sprites.ContainsKey(questID))
			{
				return _sprites[questID];
			}
			return null;
		}
	}
}
