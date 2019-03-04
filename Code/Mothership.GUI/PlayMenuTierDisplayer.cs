using System;
using UnityEngine;

namespace Mothership.GUI
{
	internal class PlayMenuTierDisplayer : MonoBehaviour, ITierComponent
	{
		[Serializable]
		private class Style
		{
			public Color bgColor;

			public Color textColor;
		}

		[Serializable]
		private class TierItem
		{
			public UILabel label;

			public UISprite background;
		}

		private int _tier = -1;

		[SerializeField]
		private Style _activeTierStyle;

		[SerializeField]
		private Style _inactiveTierStyle;

		[SerializeField]
		private TierItem[] _tierItems;

		public int tier
		{
			get
			{
				return _tier;
			}
			set
			{
				//IL_0049: Unknown result type (might be due to invalid IL or missing references)
				//IL_005a: Unknown result type (might be due to invalid IL or missing references)
				if (_tier != value)
				{
					_tier = value;
					for (int i = 0; i < _tierItems.Length; i++)
					{
						TierItem tierItem = _tierItems[i];
						Style style = (i != _tier) ? _inactiveTierStyle : _activeTierStyle;
						tierItem.background.set_color(style.bgColor);
						tierItem.label.set_color(style.textColor);
					}
				}
			}
		}

		public PlayMenuTierDisplayer()
			: this()
		{
		}
	}
}
