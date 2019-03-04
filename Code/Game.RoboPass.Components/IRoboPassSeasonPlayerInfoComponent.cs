using Svelto.ECS;

namespace Game.RoboPass.Components
{
	internal interface IRoboPassSeasonPlayerInfoComponent
	{
		float progressInGrade
		{
			get;
			set;
		}

		int currentGradeIndex
		{
			get;
			set;
		}

		int deltaXPToShow
		{
			get;
			set;
		}

		bool hasDeluxe
		{
			get;
			set;
		}

		DispatchOnSet<bool> reachedNewGrade
		{
			get;
		}

		DispatchOnSet<bool> dataUpdated
		{
			get;
		}
	}
}
