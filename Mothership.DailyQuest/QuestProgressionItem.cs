using UnityEngine;

namespace Mothership.DailyQuest
{
	public class QuestProgressionItem : MonoBehaviour
	{
		[SerializeField]
		private UIWidget _mainWidget;

		[SerializeField]
		private Transform _inProgressAnchor;

		[SerializeField]
		private Transform _completedAnchor;

		[SerializeField]
		private GameObject _questOnObject;

		[SerializeField]
		private GameObject _questOffObject;

		[SerializeField]
		private UILabel _title;

		[SerializeField]
		private UITexture _image;

		[SerializeField]
		private UISlider _progressSlider;

		[SerializeField]
		private UILabel _progressLabel;

		[SerializeField]
		private GameObject _completedTickObject;

		[SerializeField]
		private GameObject _completedRewardsObject;

		[SerializeField]
		private UILabel _earnedExperienceAmount;

		[SerializeField]
		private UILabel _earnedRobitsAmount;

		public QuestProgressionItem()
			: this()
		{
		}

		public void UpdateData(Quest quest, Texture2D sprite, bool hasPremium)
		{
			if (quest == null)
			{
				_mainWidget.SetAnchor(_inProgressAnchor);
				ResetAnchor(_mainWidget);
				_questOnObject.SetActive(false);
				_questOffObject.SetActive(true);
				return;
			}
			bool flag = quest.progressCount >= quest.targetCount;
			Transform anchor = (!flag) ? _inProgressAnchor : _completedAnchor;
			_mainWidget.SetAnchor(anchor);
			ResetAnchor(_mainWidget);
			_questOnObject.SetActive(true);
			_questOffObject.SetActive(false);
			_title.set_text(StringTableBase<StringTable>.Instance.GetString(quest.questNameStrKey));
			_image.set_mainTexture(sprite);
			_progressSlider.set_value((float)quest.progressCount / (float)quest.targetCount);
			_progressLabel.get_gameObject().SetActive(!flag);
			_progressLabel.set_text($"{quest.progressCount} / {quest.targetCount}");
			_completedTickObject.SetActive(flag);
			_completedRewardsObject.SetActive(flag);
			_earnedExperienceAmount.set_text("+" + ((!hasPremium) ? quest.xp : quest.premiumXP));
			_earnedRobitsAmount.set_text("+" + ((!hasPremium) ? quest.robits : quest.premiumRobits));
		}

		private void ResetAnchor(UIWidget widget)
		{
			widget.leftAnchor.absolute = 0;
			widget.rightAnchor.absolute = 0;
			widget.topAnchor.absolute = 0;
			widget.bottomAnchor.absolute = 0;
		}
	}
}
