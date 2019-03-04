using UnityEngine;

namespace Mothership.DailyQuest
{
	internal class QuestItem : MonoBehaviour
	{
		[SerializeField]
		private UIWidget mainWidget;

		[SerializeField]
		private UILabel title;

		[SerializeField]
		private UITexture image;

		[SerializeField]
		private UILabel description;

		[SerializeField]
		private UILabel progressLabel;

		[SerializeField]
		private UISlider progressSlider;

		[SerializeField]
		private UILabel xp;

		[SerializeField]
		private UIWidget xpWidget;

		[SerializeField]
		private UILabel premiumXP;

		[SerializeField]
		private UIWidget premiumXPWidget;

		[SerializeField]
		private UILabel robits;

		[SerializeField]
		private UIWidget robitsWidget;

		[SerializeField]
		private UILabel premiumRobits;

		[SerializeField]
		private UIWidget premiumRobitsWidget;

		[SerializeField]
		private UIButtonBroadcasterString buttBroadcaster;

		[SerializeField]
		private UIButton reroll;

		[SerializeField]
		private GameObject enabledGO;

		[SerializeField]
		private GameObject disabledGO;

		public QuestItem()
			: this()
		{
		}

		public void SetValues(string titleStr, Texture2D sprite, string descriptionStr, string progressLabelStr, float progressValue, string xpStr, string premiumXPStr, string robitsStr, string premiumRobitsStr, float nonPremiumAlpha, float premiumAlpha, bool canReroll, string questID)
		{
			title.set_text(titleStr);
			image.set_mainTexture(sprite);
			description.set_text(descriptionStr);
			progressLabel.set_text(progressLabelStr);
			progressSlider.set_value(progressValue);
			xp.set_text(xpStr);
			xpWidget.set_alpha(nonPremiumAlpha);
			premiumXP.set_text(premiumXPStr);
			premiumXPWidget.set_alpha(premiumAlpha);
			robits.set_text(robitsStr);
			robitsWidget.set_alpha(nonPremiumAlpha);
			premiumRobits.set_text(premiumRobitsStr);
			premiumRobitsWidget.set_alpha(premiumAlpha);
			buttBroadcaster.sendValue = questID;
			reroll.set_isEnabled(canReroll);
		}

		public void Show(bool enable)
		{
			enabledGO.SetActive(enable);
			disabledGO.SetActive(!enable);
		}

		public void UpdateMainWidgetAnchor(Transform anchorTransform)
		{
			mainWidget.SetAnchor(anchorTransform);
		}
	}
}
