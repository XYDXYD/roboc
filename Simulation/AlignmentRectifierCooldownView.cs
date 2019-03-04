using Fabric;
using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class AlignmentRectifierCooldownView : MonoBehaviour
	{
		[SerializeField]
		private UILabel flipLabel;

		private Animation _animation;

		private string _cooldownErrorSound = "KUB_DEMO_fabric_GUI_TeleCoolDownExpand";

		private string _cooldownEndSound = "KUB_DEMO_fabric_GUI_TeleCoolDownContract";

		private HudStyle _hudStyleStyle;

		[Inject]
		internal AlignmentRectifierEngine alignmentRectifierManager
		{
			private get;
			set;
		}

		public AlignmentRectifierCooldownView()
			: this()
		{
		}

		private void Start()
		{
			_animation = this.GetComponentInChildren<Animation>();
			alignmentRectifierManager.RegisterView(this);
		}

		private void OnDestroy()
		{
			alignmentRectifierManager.UnregisterView(this);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void SetStyle(HudStyle style)
		{
			_hudStyleStyle = style;
			switch (style)
			{
			case HudStyle.HideAll:
				Hide();
				break;
			case HudStyle.HideAllButChat:
				Hide();
				break;
			case HudStyle.Full:
				Show();
				break;
			}
		}

		public void Show()
		{
			if (_hudStyleStyle != HudStyle.HideAllButChat)
			{
				this.get_gameObject().SetActive(true);
				string text = alignmentRectifierManager.FetchKeyCodeForAction("strKeyPulseAR");
				if (text != null)
				{
					string @string = StringTableBase<StringTable>.Instance.GetString("strFlipRobotLabel");
					int length = @string.IndexOf('[');
					int num = @string.IndexOf(']');
					string str = @string.Substring(0, length);
					string str2 = @string.Substring(num + 1);
					flipLabel.set_text(str + text + str2);
				}
				else
				{
					flipLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strFlipRobotMouseOrJoystickLabel"));
				}
			}
		}

		public void PlayErrorCooldownActiveSound(string cooldownErrorSound, string cooldownEndSound)
		{
			_cooldownErrorSound = cooldownErrorSound;
			_cooldownEndSound = cooldownEndSound;
			EventManager.get_Instance().PostEvent(_cooldownErrorSound, 0);
		}
	}
}
