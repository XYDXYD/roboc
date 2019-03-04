using Svelto.ECS;
using System;

namespace Game.RoboPass.Components
{
	internal interface IRoboPassSeasonInfoComponent
	{
		TimeSpan timeRemaining
		{
			get;
			set;
		}

		int gradesHighestIndex
		{
			get;
			set;
		}

		int[] xpBetweenGrades
		{
			get;
			set;
		}

		string robopassSeasonName
		{
			get;
			set;
		}

		string robopassSeasonNameKey
		{
			get;
			set;
		}

		RoboPassSeasonRewardData[][] gradesRewards
		{
			get;
			set;
		}

		DispatchOnSet<bool> dataUpdated
		{
			get;
		}

		bool isValidSeason
		{
			get;
			set;
		}
	}
}
