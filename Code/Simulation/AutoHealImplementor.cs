using Svelto.ECS;

namespace Simulation
{
	internal class AutoHealImplementor : IAutoHealComponent
	{
		public float timer
		{
			get;
			set;
		}

		public float healTimer
		{
			get;
			set;
		}

		public float spawnHealTimer
		{
			get;
			set;
		}

		public int amountOfHealthToRestore
		{
			get;
			set;
		}

		public float timeSinceLastHealthRestore
		{
			get;
			set;
		}

		public DispatchOnSet<bool> healCancelled
		{
			get;
			set;
		}

		public AutoHealImplementor(int entityId)
		{
			healCancelled = new DispatchOnChange<bool>(entityId);
		}
	}
}
