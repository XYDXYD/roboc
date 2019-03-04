using System;
using UnityEngine;

namespace Mothership.GUI
{
	internal class TierProgressionWidget : MonoBehaviour, ITierComponent, ITierProgressionWidgetComponent
	{
		[Serializable]
		private class RankStyle
		{
			public Color color;

			public float scale;
		}

		[SerializeField]
		private UISprite[] _progressBars;

		[SerializeField]
		private UISprite[] _rankIcons;

		[SerializeField]
		private UILabel[] _iconTierLabels;

		[SerializeField]
		private UILabel _tierLabel;

		[SerializeField]
		private UILabel _rankLabel;

		[SerializeField]
		private RankStyle unlockedRankStyle;

		[SerializeField]
		private RankStyle currentRankStyle;

		[SerializeField]
		private RankStyle lockedRankStyle;

		private int _rank;

		private int _tier;

		public float progressInRank
		{
			set
			{
				if (_rank < _progressBars.Length)
				{
					_progressBars[_rank].set_fillAmount(value);
				}
			}
		}

		public int rank
		{
			set
			{
				//IL_0091: Unknown result type (might be due to invalid IL or missing references)
				//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
				//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
				_rank = value;
				for (int i = 0; i < _progressBars.Length; i++)
				{
					_progressBars[i].set_fillAmount((i >= _rank) ? 0f : 1f);
				}
				for (int j = 0; j < _rankIcons.Length; j++)
				{
					UISprite val = _rankIcons[j];
					RankStyle rankStyle = (j >= _rank) ? ((j != _rank) ? lockedRankStyle : currentRankStyle) : unlockedRankStyle;
					val.set_color(rankStyle.color);
					val.get_transform().set_localScale(Vector3.get_one() * rankStyle.scale);
				}
			}
		}

		public int tier
		{
			get
			{
				return _tier;
			}
			set
			{
				_tier = value;
				string text = RRAndTiers.ConvertTierIndexToStringNoMegabotCheck((uint)_tier);
				for (int i = 0; i < _rankIcons.Length; i++)
				{
					_iconTierLabels[i].set_text(text);
				}
			}
		}

		public string tierString
		{
			set
			{
				_tierLabel.set_text(value);
			}
		}

		public string rankString
		{
			set
			{
				_rankLabel.set_text(value);
			}
		}

		public TierProgressionWidget()
			: this()
		{
		}
	}
}
