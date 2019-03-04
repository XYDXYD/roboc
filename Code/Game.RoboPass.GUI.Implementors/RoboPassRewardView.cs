using Game.RoboPass.GUI.Components;
using System.Diagnostics;
using UnityEngine;

namespace Game.RoboPass.GUI.Implementors
{
	internal class RoboPassRewardView : MonoBehaviour, IRoboPassRewardUICellComponent
	{
		[SerializeField]
		private UILabel _rewardGradeLabel;

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
		private GameObject _rewardUnlockedGO;

		private string _strLocalizedRoboPassGrade;

		public bool isDeluxeCell
		{
			get;
			set;
		}

		public string rewardGradeLabel
		{
			set
			{
				_rewardGradeLabel.set_text(_strLocalizedRoboPassGrade + value);
			}
		}

		public bool rewardLockedWidgetVisible
		{
			set
			{
				_rewardLockedGO.SetActive(value);
			}
		}

		public bool rewardUnlockedWidgetVisible
		{
			set
			{
				_rewardUnlockedGO.SetActive(value);
			}
		}

		public bool visible
		{
			set
			{
				this.get_gameObject().SetActive(value);
			}
		}

		public bool isSpriteFullSize
		{
			set
			{
				_rewardSpriteFullSize.get_gameObject().SetActive(value);
				_rewardSprite.get_gameObject().SetActive(!value);
			}
		}

		public string rewardSprite
		{
			set
			{
				_rewardSprite.set_spriteName(value);
				_rewardSpriteFullSize.set_spriteName(value);
			}
		}

		public string rewardType
		{
			set
			{
				_rewardType.set_text(value);
				_rewardType.get_gameObject().get_transform().get_parent()
					.get_gameObject()
					.SetActive(!string.IsNullOrEmpty(value));
			}
		}

		public string rewardName
		{
			set
			{
				_rewardName.set_text(value);
			}
		}

		public RoboPassRewardView()
			: this()
		{
		}

		internal void Initialize(bool isDeluxeCell_)
		{
			isDeluxeCell = isDeluxeCell_;
			_strLocalizedRoboPassGrade = StringTableBase<StringTable>.Instance.GetString("strRoboPassGrade").ToUpper() + " ";
		}

		[Conditional("DEBUG")]
		private void CheckIfExposedFieldsAreSet()
		{
		}
	}
}
