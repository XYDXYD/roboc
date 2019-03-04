using System;

namespace RCNetwork.Events
{
	internal sealed class PlayerIdEventArgs : EventArgs
	{
		public int playerId;

		public PlayerIdEventArgs(int _playerId)
		{
			playerId = _playerId;
		}
	}
}
