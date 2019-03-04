namespace Game.RoboPass.GUI.Components
{
	internal interface IRoboPassRewardUICellComponent
	{
		bool isDeluxeCell
		{
			get;
			set;
		}

		bool rewardLockedWidgetVisible
		{
			set;
		}

		bool rewardUnlockedWidgetVisible
		{
			set;
		}

		bool visible
		{
			set;
		}

		bool isSpriteFullSize
		{
			set;
		}

		string rewardGradeLabel
		{
			set;
		}

		string rewardName
		{
			set;
		}

		string rewardSprite
		{
			set;
		}

		string rewardType
		{
			set;
		}
	}
}
