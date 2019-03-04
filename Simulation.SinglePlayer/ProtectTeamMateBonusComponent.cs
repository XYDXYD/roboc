using System;
using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	internal class ProtectTeamMateBonusComponent : IProtectTeamMateBonusComponent
	{
		private Dictionary<int, Dictionary<uint, uint>> _cachedData = new Dictionary<int, Dictionary<uint, uint>>();

		private int _attackedPlayerId;

		private DateTime _timestamp;

		int IProtectTeamMateBonusComponent.attackedPlayerId
		{
			get
			{
				return _attackedPlayerId;
			}
			set
			{
				_attackedPlayerId = value;
			}
		}

		DateTime IProtectTeamMateBonusComponent.timestamp
		{
			get
			{
				return _timestamp;
			}
			set
			{
				_timestamp = value;
			}
		}

		Dictionary<int, Dictionary<uint, uint>> IProtectTeamMateBonusComponent.cachedcubes
		{
			get
			{
				return _cachedData;
			}
		}

		public ProtectTeamMateBonusComponent()
		{
			_timestamp = DateTime.UtcNow;
		}
	}
}
