using Game.RoboPass.GUI.Implementors;

namespace Game.RoboPass.GUI.Components
{
	public interface IRoboPassBattleSummaryRewardItemComponent
	{
		RobopassRewardItemScreenType ItemScreenType
		{
			get;
		}

		string ItemName
		{
			set;
		}

		string ItemSprite
		{
			set;
		}

		string ItemType
		{
			set;
		}

		bool IsSpriteFullSize
		{
			set;
		}

		bool ItemActive
		{
			set;
		}

		bool IsDeluxe
		{
			get;
		}

		bool IsLocked
		{
			set;
		}
	}
}
