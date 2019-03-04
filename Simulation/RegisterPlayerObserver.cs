using System;

namespace Simulation
{
	internal sealed class RegisterPlayerObserver
	{
		public event Action<string, int, bool, bool> OnRegisterPlayer = delegate
		{
		};

		public void RegisterPlayer(string name, int id, bool isMe, bool isMyTeam)
		{
			this.OnRegisterPlayer(name, id, isMe, isMyTeam);
		}
	}
}
