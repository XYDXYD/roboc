using Achievements;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using System.Collections.Generic;

namespace Simulation.Achievements
{
	internal class AchievementModuleActivatedTrackerEngine : MultiEntityViewsEngine<AchievementModuleActivationNode, AchievementMachineVisibilityNode>, IQueryingEntityViewEngine, IInitialize, IWaitForFrameworkDestruction, IEngine
	{
		private Dictionary<ItemCategory, int> _localPlayerModuleActivatedCount = new Dictionary<ItemCategory, int>();

		[Inject]
		private IAchievementManager achievementManager
		{
			get;
			set;
		}

		[Inject]
		private GameEndedObserver gameEndedObserver
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			gameEndedObserver.OnGameEnded += SendModuleActivatedCountDuringBattle;
		}

		public void OnFrameworkDestroyed()
		{
			gameEndedObserver.OnGameEnded += SendModuleActivatedCountDuringBattle;
		}

		private void AddActivatedModuleCount(IModuleActivationComponent activationComponent, int moduleId)
		{
			AchievementModuleActivationNode achievementModuleActivationNode = default(AchievementModuleActivationNode);
			if (entityViewsDB.TryQueryEntityView<AchievementModuleActivationNode>(moduleId, ref achievementModuleActivationNode))
			{
				ItemCategory itemCategory = achievementModuleActivationNode.itemDescriptorComponent.itemDescriptor.itemCategory;
				Dictionary<ItemCategory, int> localPlayerModuleActivatedCount;
				ItemCategory key;
				(localPlayerModuleActivatedCount = _localPlayerModuleActivatedCount)[key = itemCategory] = localPlayerModuleActivatedCount[key] + 1;
			}
		}

		private void AddActivatedCloakModuleCount(IMachineVisibilityComponent component, int moduleId)
		{
			Dictionary<ItemCategory, int> localPlayerModuleActivatedCount;
			(localPlayerModuleActivatedCount = _localPlayerModuleActivatedCount)[ItemCategory.GhostModule] = localPlayerModuleActivatedCount[ItemCategory.GhostModule] + 1;
		}

		private void SendModuleActivatedCountDuringBattle(bool won)
		{
			using (Dictionary<ItemCategory, int>.Enumerator enumerator = _localPlayerModuleActivatedCount.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					achievementManager.ActivatedModule(enumerator.Current.Key, enumerator.Current.Value);
				}
			}
		}

		protected override void Add(AchievementModuleActivationNode node)
		{
			if (!node.ownerComponent.ownedByMe)
			{
				return;
			}
			ItemCategory itemCategory = node.itemDescriptorComponent.itemDescriptor.itemCategory;
			if (itemCategory != ItemCategory.GhostModule)
			{
				if (!_localPlayerModuleActivatedCount.ContainsKey(itemCategory))
				{
					_localPlayerModuleActivatedCount.Add(itemCategory, 0);
				}
				node.activationComponent.activate.subscribers += AddActivatedModuleCount;
			}
		}

		protected override void Remove(AchievementModuleActivationNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				ItemCategory itemCategory = node.itemDescriptorComponent.itemDescriptor.itemCategory;
				if (itemCategory != ItemCategory.GhostModule)
				{
					node.activationComponent.activate.subscribers -= AddActivatedModuleCount;
				}
			}
		}

		protected override void Add(AchievementMachineVisibilityNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				if (!_localPlayerModuleActivatedCount.ContainsKey(ItemCategory.GhostModule))
				{
					_localPlayerModuleActivatedCount.Add(ItemCategory.GhostModule, 0);
				}
				node.machineVisibilityComponent.machineBecameInvisible.subscribers += AddActivatedCloakModuleCount;
			}
		}

		protected override void Remove(AchievementMachineVisibilityNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				node.machineVisibilityComponent.machineBecameInvisible.subscribers -= AddActivatedCloakModuleCount;
			}
		}

		public void Ready()
		{
		}
	}
}
