using Simulation.NamedCollections;
using Simulation.SinglePlayer.ServerMock;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation.SinglePlayer
{
	internal abstract class AggregateCubesCpuBonusServerMockCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private SinglePlayerAggregateCubesCountBonusDependency _dependency;

		[Inject]
		public TeamDeathMatchAIScoreServerMock teamDeathMatchAIScoreServerMock
		{
			private get;
			set;
		}

		[Inject]
		public ICubeList cubeData
		{
			private get;
			set;
		}

		protected InGameStatId gameStatId
		{
			get;
			set;
		}

		public AggregateCubesCpuBonusServerMockCommand()
		{
			InitialiseGameStatId();
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_dependency = (dependency as SinglePlayerAggregateCubesCountBonusDependency);
			return this;
		}

		public void Execute()
		{
			foreach (KeyValuePair<int, PlayersCubes> shooterTargetPlayer in _dependency.shooterTargetPlayers)
			{
				int key = shooterTargetPlayer.Key;
				int num = 0;
				foreach (KeyValuePair<int, CubeAmounts> item in shooterTargetPlayer.Value)
				{
					foreach (KeyValuePair<uint, uint> item2 in item.Value)
					{
						uint cubeCPURating = cubeData.GetCubeCPURating(new CubeTypeID(item2.Key));
						num += (int)(cubeCPURating * item2.Value);
					}
				}
				teamDeathMatchAIScoreServerMock.UpdateStats(key, gameStatId, num);
			}
		}

		protected abstract void InitialiseGameStatId();
	}
}
