using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules.PowerModule
{
	internal sealed class PowerModuleEffectsEngine : SingleEntityViewEngine<PowerModuleEffectsNode>, IQueryingEntityViewEngine, IEngine
	{
		private readonly Func<GameObject> _onActivationEffectAllocation;

		private PowerUpdateObserver _observer;

		private GameObject _currentActivationPrefab;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe PowerModuleEffectsEngine(PowerUpdateObserver observer)
		{
			_observer = observer;
			_observer.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_onActivationEffectAllocation = OnActivationEffectAllocation;
		}

		public void Ready()
		{
		}

		protected override void Add(PowerModuleEffectsNode node)
		{
			PreallocateEffects(node);
		}

		protected override void Remove(PowerModuleEffectsNode node)
		{
		}

		private void PreallocateEffects(PowerModuleEffectsNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_currentActivationPrefab = node.effectsComponent.LocalPlayerEffectPrefab;
			}
			else if (node.ownerComponent.isEnemy)
			{
				_currentActivationPrefab = node.effectsComponent.EnemyEffectPrefab;
			}
			else
			{
				_currentActivationPrefab = node.effectsComponent.AllyEffectPrefab;
			}
			gameObjectPool.Preallocate(_currentActivationPrefab.get_name(), 1, _onActivationEffectAllocation);
		}

		private GameObject GetActivationEffect(GameObject prefab)
		{
			_currentActivationPrefab = prefab;
			return gameObjectPool.Use(_currentActivationPrefab.get_name(), _onActivationEffectAllocation);
		}

		private void HandleOnPowerModuleActivated(ref int playerId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<PowerModuleEffectsNode> val = entityViewsDB.QueryEntityViews<PowerModuleEffectsNode>();
			int num = 0;
			PowerModuleEffectsNode powerModuleEffectsNode;
			while (true)
			{
				if (num < val.get_Count())
				{
					powerModuleEffectsNode = val.get_Item(num);
					if (powerModuleEffectsNode.ownerComponent.ownerId == playerId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			IPowerModuleEffectsComponent effectsComponent = powerModuleEffectsNode.effectsComponent;
			GameObject prefab = powerModuleEffectsNode.ownerComponent.ownedByMe ? effectsComponent.LocalPlayerEffectPrefab : ((!powerModuleEffectsNode.ownerComponent.isEnemy) ? effectsComponent.AllyEffectPrefab : effectsComponent.EnemyEffectPrefab);
			PlayEffect(GetActivationEffect(prefab), powerModuleEffectsNode);
		}

		private void PlayEffect(GameObject gameObject, PowerModuleEffectsNode node)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			gameObject.SetActive(true);
			Transform transform = gameObject.get_transform();
			Transform transform2 = node.rigidBodyComponent.rb.get_transform();
			Vector3 machineSize = node.machineDimensionComponent.machineSize;
			machineSize.x = (machineSize.z = Mathf.Max(machineSize.x, machineSize.z));
			transform.set_localScale(machineSize);
			transform.set_parent(transform2);
			transform.set_localPosition(node.machineDimensionComponent.machineCenter);
			transform.set_localRotation(Quaternion.get_identity());
		}

		private GameObject OnActivationEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForLoopParticles(_currentActivationPrefab, 1f);
		}
	}
}
