using System;
using UnityEngine;

namespace Mothership.RobotConfiguration
{
	[Serializable]
	internal sealed class CheckBoxSetting
	{
		public UIToggle checkboxToggle;

		public UIButton checkBoxButton;

		public UILabel checkBoxText;

		public UISprite checkMarkSprite;

		public bool Selected
		{
			get
			{
				return checkboxToggle.get_value();
			}
			set
			{
				checkboxToggle.Set(value, true);
			}
		}

		public void SetEnabled(bool enabled, Color color)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			checkBoxText.set_color(color);
			checkMarkSprite.set_color(color);
			checkMarkSprite.set_alpha((!checkboxToggle.get_value()) ? 0f : 1f);
			checkBoxButton.set_isEnabled(enabled);
		}
	}
}
