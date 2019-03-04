using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections.Generic;

namespace Simulation.Hardware.Movement
{
	internal class LoadMovementStatsEngine : SingleEntityViewEngine<MovementStatsNode>, IInitialize
	{
		protected IDictionary<int, MovementStatsData> _movementStatsData;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			ILoadMovementStatsRequest loadMovementStatsRequest = serviceFactory.Create<ILoadMovementStatsRequest>();
			loadMovementStatsRequest.SetAnswer(new ServiceAnswer<MovementStats>(delegate(MovementStats statsData)
			{
				_movementStatsData = statsData.data;
			})).Execute();
		}

		protected override void Add(MovementStatsNode node)
		{
			int key = node.itemDescriptorComponent.itemDescriptor.GenerateKey();
			MovementStatsData movementstats = _movementStatsData[key];
			SetStats(node, movementstats);
		}

		protected override void Remove(MovementStatsNode node)
		{
		}

		private void SetStats(MovementStatsNode node, MovementStatsData movementstats)
		{
			node.statsComponent.horizontalMaxSpeed = movementstats.horizontalTopSpeed;
			node.statsComponent.verticalMaxSpeed = movementstats.verticalTopSpeed;
			node.statsComponent.speedBoost = movementstats.speedBoost;
			node.statsComponent.minRequiredItems = movementstats.minRequiredItems;
			node.statsComponent.minRequiredItemsModifier = movementstats.minRequiredItemsModifier;
			node.maxCarryMassComponent.maxCarryMass = movementstats.maxCarryMass;
		}
	}
}
