using Svelto.Context;
using Svelto.IoC;
using System;

namespace Simulation
{
	internal class BattleEventStreamManager : IInitialize, IWaitForFrameworkDestruction, IBattleEventStreamManager
	{
		[Inject]
		public MachineSpawnDispatcher machineDispatcher
		{
			private get;
			set;
		}

		public event Action<int> OnPlayerSpawnedIn = delegate
		{
		};

		public event Action<int> OnPlayerSpawnedOut = delegate
		{
		};

		public event Action<int, int> OnPlayerWasKilledBy = delegate
		{
		};

		void IInitialize.OnDependenciesInjected()
		{
			machineDispatcher.OnPlayerSpawnedIn += PlayerSpawnedIn;
			machineDispatcher.OnPlayerSpawnedOut += PlayerSpawnedOut;
		}

		private void PlayerSpawnedIn(SpawnInParametersPlayer paramaters)
		{
			this.OnPlayerSpawnedIn(paramaters.playerId);
		}

		private void PlayerSpawnedOut(SpawnOutParameters paramaters)
		{
			this.OnPlayerSpawnedOut(paramaters.playerId);
		}

		public void PlayerWasKilledBy(int player, int shooter)
		{
			this.OnPlayerWasKilledBy(player, shooter);
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			machineDispatcher.OnPlayerSpawnedIn -= PlayerSpawnedIn;
			machineDispatcher.OnPlayerSpawnedOut -= PlayerSpawnedOut;
		}
	}
}
