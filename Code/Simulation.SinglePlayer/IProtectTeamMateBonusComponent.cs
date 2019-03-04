using System;
using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	public interface IProtectTeamMateBonusComponent
	{
		int attackedPlayerId
		{
			get;
			set;
		}

		DateTime timestamp
		{
			get;
			set;
		}

		Dictionary<int, Dictionary<uint, uint>> cachedcubes
		{
			get;
		}
	}
}
