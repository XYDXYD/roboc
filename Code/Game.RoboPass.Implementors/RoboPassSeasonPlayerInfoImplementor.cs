using Game.RoboPass.Components;
using Svelto.ECS;

namespace Game.RoboPass.Implementors
{
	internal class RoboPassSeasonPlayerInfoImplementor : IRoboPassSeasonPlayerInfoComponent
	{
		public DispatchOnSet<bool> dataUpdated
		{
			get;
			private set;
		}

		public DispatchOnSet<bool> reachedNewGrade
		{
			get;
			private set;
		}

		public float progressInGrade
		{
			get;
			set;
		}

		public int deltaXPToShow
		{
			get;
			set;
		}

		public bool hasDeluxe
		{
			get;
			set;
		}

		public int currentGradeIndex
		{
			get;
			set;
		}

		public int gradesHighestIndex
		{
			get;
			set;
		}

		public RoboPassSeasonPlayerInfoImplementor(int senderID)
		{
			dataUpdated = new DispatchOnSet<bool>(senderID);
			reachedNewGrade = new DispatchOnSet<bool>(senderID);
			progressInGrade = 0f;
			deltaXPToShow = -1;
			currentGradeIndex = -1;
			gradesHighestIndex = -1;
		}
	}
}
