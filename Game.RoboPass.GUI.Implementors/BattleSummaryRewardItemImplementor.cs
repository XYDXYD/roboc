using Game.RoboPass.GUI.Components;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	public class BattleSummaryRewardItemImplementor : MonoBehaviour, IRoboPassBattleSummaryRewardItemComponent
	{
		[SerializeField]
		private UISprite _rewardSprite;

		[SerializeField]
		private UISprite _rewardSpriteFullSize;

		[SerializeField]
		private UILabel _rewardType;

		[SerializeField]
		private UILabel _rewardName;

		[SerializeField]
		private GameObject _rewardLockedGO;

		[SerializeField]
		private bool _isDeluxe;

		[SerializeField]
		private RobopassRewardItemScreenType _itemScreenType;

		public string ItemName
		{
			set
			{
				_rewardName.set_text(value);
			}
		}

		public string ItemType
		{
			set
			{
				_rewardType.set_text(value);
			}
		}

		public string ItemSprite
		{
			set
			{
				_rewardSpriteFullSize.set_spriteName(value);
				_rewardSprite.set_spriteName(value);
			}
		}

		public bool ItemActive
		{
			set
			{
				this.get_gameObject().SetActive(value);
			}
		}

		public bool IsDeluxe => _isDeluxe;

		public RobopassRewardItemScreenType ItemScreenType => _itemScreenType;

		public bool IsSpriteFullSize
		{
			set
			{
				_rewardSpriteFullSize.get_gameObject().SetActive(value);
				_rewardSprite.get_gameObject().SetActive(!value);
			}
		}

		public bool IsLocked
		{
			set
			{
				_rewardLockedGO.SetActive(value);
			}
		}

		public BattleSummaryRewardItemImplementor()
			: this()
		{
		}
	}
}
