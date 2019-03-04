using Svelto.ECS;

namespace Simulation
{
	internal interface IAutoHealComponent
	{
		float timer
		{
			get;
			set;
		}

		float healTimer
		{
			get;
			set;
		}

		float spawnHealTimer
		{
			get;
			set;
		}

		int amountOfHealthToRestore
		{
			get;
			set;
		}

		float timeSinceLastHealthRestore
		{
			get;
			set;
		}

		DispatchOnSet<bool> healCancelled
		{
			get;
		}
	}
}
