using UnityEngine;

namespace Mothership.GUI.Party
{
	public class PartyButtonInteraction : MonoBehaviour
	{
		private struct InteractionColours
		{
			public UIButton originatingButton;

			public Color normal;

			public Color hover;

			public Color pressed;
		}

		private InteractionColours[] _buttonComponentInteractionColors;

		private bool _isButtonEnabled;

		public PartyButtonInteraction()
			: this()
		{
		}

		public void Activate()
		{
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			Component[] componentsInChildren = this.GetComponentsInChildren(typeof(UIButton), true);
			_buttonComponentInteractionColors = new InteractionColours[componentsInChildren.Length];
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				UIButton val = componentsInChildren[i] as UIButton;
				_buttonComponentInteractionColors[i].originatingButton = val;
				_buttonComponentInteractionColors[i].normal = val.get_defaultColor();
				_buttonComponentInteractionColors[i].hover = val.hover;
				_buttonComponentInteractionColors[i].pressed = val.pressed;
			}
		}

		public void DisableButton()
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_004d: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			_isButtonEnabled = false;
			for (int i = 0; i < _buttonComponentInteractionColors.Length; i++)
			{
				Color disabledColor = _buttonComponentInteractionColors[i].originatingButton.disabledColor;
				_buttonComponentInteractionColors[i].originatingButton.set_defaultColor(disabledColor);
				_buttonComponentInteractionColors[i].originatingButton.pressed = disabledColor;
				_buttonComponentInteractionColors[i].originatingButton.hover = disabledColor;
			}
		}

		public void EnableButton()
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			_isButtonEnabled = true;
			for (int i = 0; i < _buttonComponentInteractionColors.Length; i++)
			{
				_buttonComponentInteractionColors[i].originatingButton.set_defaultColor(_buttonComponentInteractionColors[i].normal);
				_buttonComponentInteractionColors[i].originatingButton.pressed = _buttonComponentInteractionColors[i].pressed;
				_buttonComponentInteractionColors[i].originatingButton.hover = _buttonComponentInteractionColors[i].hover;
			}
		}
	}
}
