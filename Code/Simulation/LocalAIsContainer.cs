using System.Collections;
using System.Collections.Generic;

namespace Simulation
{
	internal sealed class LocalAIsContainer
	{
		private List<int> _localAiIDs = new List<int>();

		private bool _initialized;

		internal void OnReceiveHostedAIs(PlayerIDsDependency dependency)
		{
			_localAiIDs.Clear();
			_localAiIDs.AddRange(dependency.playerIds);
			_initialized = true;
		}

		public bool IsAIHostedLocally(int playerId)
		{
			return _localAiIDs.Contains(playerId);
		}

		public IEnumerator LoadData()
		{
			while (!_initialized)
			{
				yield return null;
			}
		}
	}
}
