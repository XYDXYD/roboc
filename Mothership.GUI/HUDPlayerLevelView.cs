using System.Text;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDPlayerLevelView : MonoBehaviour
	{
		public enum StyleVersion
		{
			EditMode,
			EditCustomGame
		}

		[SerializeField]
		private Animation animLevelUpBar;

		[SerializeField]
		private Animation animLevelUpMsg;

		[SerializeField]
		private Animation animXPEarned;

		[SerializeField]
		private UILabel labelLevel;

		[SerializeField]
		private UILabel labelXPEarned;

		[SerializeField]
		private UISprite spriteLevel;

		[SerializeField]
		private UIWidget _container;

		[SerializeField]
		private UIWidget _editStyleWidget;

		[SerializeField]
		private UIWidget _editCustomGameStyleWidget;

		private readonly StringBuilder _stringBuilder;

		public HUDPlayerLevelView()
			: this()
		{
			_stringBuilder = new StringBuilder(16);
		}

		internal void ShowXPIncrement(int playerXP)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.Append("+");
			_stringBuilder.Append(playerXP);
			labelXPEarned.set_text(_stringBuilder.ToString());
			animXPEarned.Play();
		}

		internal void SetPlayerLevel(int playerLevel)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.Append("LEVEL ");
			_stringBuilder.Append(playerLevel);
			labelLevel.set_text(_stringBuilder.ToString());
		}

		internal void ShowLevelUpAnimations()
		{
			ShowLevelUpBarAnim();
			animLevelUpMsg.Play();
		}

		internal void ShowLevelUpBarAnim()
		{
			animLevelUpBar.Play();
		}

		internal void SetPlayerProgress(float playerProgressCurrent)
		{
			spriteLevel.set_fillAmount(playerProgressCurrent);
		}

		internal void HideUI()
		{
			RewindAnims();
			this.get_gameObject().SetActive(false);
		}

		internal void ShowUI(StyleVersion style)
		{
			this.get_gameObject().SetActive(true);
			SetStyle(style);
			RewindAnims();
		}

		private void SetStyle(StyleVersion style)
		{
			switch (style)
			{
			case StyleVersion.EditMode:
				UIAnchorUtility.CopyAnchors(_editStyleWidget, _container, 12);
				break;
			case StyleVersion.EditCustomGame:
				UIAnchorUtility.CopyAnchors(_editCustomGameStyleWidget, _container, 12);
				break;
			}
		}

		private void Awake()
		{
			this.get_gameObject().SetActive(false);
		}

		private void Start()
		{
			labelXPEarned.set_text(string.Empty);
		}

		private void RewindAnims()
		{
			RealRewind(animLevelUpMsg);
			RealRewind(animXPEarned);
		}

		private static void RealRewind(Animation animation)
		{
			animation.Rewind();
			animation.Play();
			animation.Sample();
			animation.Stop();
		}
	}
}
