using System;

namespace GameServer
{
	internal sealed class CapturePointsObserverServer
	{
		public event Action<int, int, int> OnPointCaptured = delegate
		{
		};

		internal void PointCaptured(int id, int team, int previousTeam)
		{
			this.OnPointCaptured(id, team, previousTeam);
		}
	}
}
