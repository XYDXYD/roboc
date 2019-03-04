using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;
using Xft;

namespace Simulation.Hardware.Modules
{
	internal sealed class TeleportModuleEffectsEngine : SingleEntityViewEngine<TeleportModuleEffectsNode>, IQueryingEntityViewEngine, IEngine
	{
		private float _teleportTime;

		private GameObject _currentPrefab;

		private Func<GameObject> _onParticleEffectAllocation;

		private Func<GameObject> _onTrailEffectAllocation;

		private Func<GameObject> _onCenterEffectAllocation;

		private NetworkPlayerBlinkedObserver _observer;

		private Dictionary<int, XWeaponTrail[]> _weaponTrailsPerPlayer = new Dictionary<int, XWeaponTrail[]>();

		private Dictionary<int, FasterList<Renderer>> _renderersPerPlayer = new Dictionary<int, FasterList<Renderer>>();

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			private get;
			set;
		}

		[Inject]
		internal MachinePreloader machinePreloader
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe TeleportModuleEffectsEngine(NetworkPlayerBlinkedObserver observer)
		{
			_observer = observer;
			_observer.AddAction(new ObserverAction<NetworkPlayerBlinkedData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_onParticleEffectAllocation = OnParticleEffectAllocation;
			_onTrailEffectAllocation = OnTrailEffectAllocation;
			_onCenterEffectAllocation = OnCenterEffectAllocation;
		}

		public void Ready()
		{
		}

		protected override void Add(TeleportModuleEffectsNode effectsNode)
		{
			effectsNode.teleporterComponent.teleportStarted.subscribers += PlayAllStartEffects;
			effectsNode.teleporterComponent.teleportEnded.subscribers += PlayAllEndEffects;
			_teleportTime = effectsNode.settingsComponent.teleportTime;
			PreallocateEffects(effectsNode);
			int machineId = effectsNode.ownerComponent.machineId;
			if (!_renderersPerPlayer.ContainsKey(machineId))
			{
				StoreMachineRenderers(machineId, effectsNode.rigidbodyComponent.rb);
			}
		}

		protected override void Remove(TeleportModuleEffectsNode effectsNode)
		{
			effectsNode.teleporterComponent.teleportStarted.subscribers -= PlayAllStartEffects;
			effectsNode.teleporterComponent.teleportEnded.subscribers -= PlayAllEndEffects;
			int machineId = effectsNode.ownerComponent.machineId;
			_renderersPerPlayer.Remove(machineId);
			_weaponTrailsPerPlayer.Remove(machineId);
		}

		private void PreallocateEffects(TeleportModuleEffectsNode effectsNode)
		{
			GameObject val;
			GameObject val2;
			GameObject val3;
			GameObject val4;
			if (!effectsNode.ownerComponent.isEnemy)
			{
				val = effectsNode.effectsComponent.teleportStartEffectAlly;
				val2 = effectsNode.effectsComponent.teleportEndEffectAlly;
				val3 = effectsNode.effectsComponent.teleportTrailEffectAlly;
				val4 = effectsNode.effectsComponent.teleportGlowingCenterEffectAlly;
			}
			else
			{
				val = effectsNode.effectsComponent.teleportStartEffectEnemy;
				val2 = effectsNode.effectsComponent.teleportEndEffectEnemy;
				val3 = effectsNode.effectsComponent.teleportTrailEffectEnemy;
				val4 = effectsNode.effectsComponent.teleportGlowingCenterEffectEnemy;
			}
			_currentPrefab = val;
			gameObjectPool.Preallocate(val.get_name(), 1, _onParticleEffectAllocation);
			_currentPrefab = val2;
			gameObjectPool.Preallocate(val2.get_name(), 1, _onParticleEffectAllocation);
			_currentPrefab = val4;
			gameObjectPool.Preallocate(val4.get_name(), 1, _onCenterEffectAllocation);
			_currentPrefab = val3;
			gameObjectPool.Preallocate(val3.get_name(), 1, _onTrailEffectAllocation);
		}

		private void StoreMachineRenderers(int machineId, Rigidbody rb)
		{
			PreloadedMachine preloadedMachine = machinePreloader.GetPreloadedMachine(machineId);
			_renderersPerPlayer.Add(machineId, preloadedMachine.allRenderers);
			XWeaponTrail[] componentsInChildren = rb.get_gameObject().GetComponentsInChildren<XWeaponTrail>();
			_weaponTrailsPerPlayer.Add(machineId, componentsInChildren);
		}

		private void PlayAllStartEffects(ITeleporterComponent sender, int moduleId)
		{
			PlayStartEffect(moduleId);
			PlayTrailEffect(moduleId);
		}

		private void PlayAllEndEffects(ITeleporterComponent sender, int moduleId)
		{
			PlayEndEffect(moduleId);
		}

		private void PlayEndEffect(int moduleId)
		{
			TeleportModuleEffectsNode teleportModuleEffectsNode = default(TeleportModuleEffectsNode);
			if (entityViewsDB.TryQueryEntityView<TeleportModuleEffectsNode>(moduleId, ref teleportModuleEffectsNode))
			{
				PlayEffect(effectPrefab: (!teleportModuleEffectsNode.ownerComponent.isEnemy) ? teleportModuleEffectsNode.effectsComponent.teleportEndEffectAlly : teleportModuleEffectsNode.effectsComponent.teleportEndEffectEnemy, effectsNode: teleportModuleEffectsNode);
				MakePlayerInvisibleOrVisible(teleportModuleEffectsNode.ownerComponent.machineId, visible: true);
			}
		}

		private void PlayTrailEffect(int moduleId)
		{
			TeleportModuleEffectsNode teleportModuleEffectsNode = default(TeleportModuleEffectsNode);
			if (entityViewsDB.TryQueryEntityView<TeleportModuleEffectsNode>(moduleId, ref teleportModuleEffectsNode))
			{
				GameObject effectPrefab;
				GameObject effectPrefab2;
				if (teleportModuleEffectsNode.ownerComponent.isEnemy)
				{
					effectPrefab = teleportModuleEffectsNode.effectsComponent.teleportTrailEffectEnemy;
					effectPrefab2 = teleportModuleEffectsNode.effectsComponent.teleportGlowingCenterEffectEnemy;
				}
				else
				{
					effectPrefab = teleportModuleEffectsNode.effectsComponent.teleportTrailEffectAlly;
					effectPrefab2 = teleportModuleEffectsNode.effectsComponent.teleportGlowingCenterEffectAlly;
				}
				PlayEffect(teleportModuleEffectsNode, effectPrefab, parent: true);
				PlayEffect(teleportModuleEffectsNode, effectPrefab2, parent: true);
			}
		}

		private void PlayStartEffect(int moduleId)
		{
			TeleportModuleEffectsNode teleportModuleEffectsNode = default(TeleportModuleEffectsNode);
			if (entityViewsDB.TryQueryEntityView<TeleportModuleEffectsNode>(moduleId, ref teleportModuleEffectsNode))
			{
				PlayEffect(effectPrefab: (!teleportModuleEffectsNode.ownerComponent.isEnemy) ? teleportModuleEffectsNode.effectsComponent.teleportStartEffectAlly : teleportModuleEffectsNode.effectsComponent.teleportStartEffectEnemy, effectsNode: teleportModuleEffectsNode);
				MakePlayerInvisibleOrVisible(teleportModuleEffectsNode.ownerComponent.machineId, visible: false);
			}
		}

		private void PlayEffect(TeleportModuleEffectsNode effectsNode, GameObject effectPrefab, bool parent = false)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			Rigidbody rb = effectsNode.rigidbodyComponent.rb;
			GameObject val = gameObjectPool.Use(effectPrefab.get_name(), _onParticleEffectAllocation);
			if (!parent)
			{
				val.get_transform().set_position(rb.get_worldCenterOfMass());
			}
			else
			{
				val.get_transform().set_parent(rb.get_transform());
				val.get_transform().set_localPosition(rb.get_centerOfMass());
			}
			val.SetActive(true);
		}

		private void MakePlayerInvisibleOrVisible(int machineId, bool visible)
		{
			FasterList<Renderer> val = _renderersPerPlayer[machineId];
			for (int i = 0; i < val.get_Count(); i++)
			{
				val.get_Item(i).set_enabled(visible);
			}
			XWeaponTrail[] array = _weaponTrailsPerPlayer[machineId];
			for (int j = 0; j < array.Length; j++)
			{
				array[j].set_enabled(visible);
			}
		}

		private void HandleRemotePlayerBlinked(ref NetworkPlayerBlinkedData data)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<TeleportModuleEffectsNode> val = entityViewsDB.QueryEntityViews<TeleportModuleEffectsNode>();
			TeleportModuleEffectsNode teleportModuleEffectsNode = null;
			for (int i = 0; i < val.get_Count(); i++)
			{
				if (val.get_Item(i).ownerComponent.ownerId == data.playerId)
				{
					teleportModuleEffectsNode = val.get_Item(i);
					break;
				}
			}
			if (teleportModuleEffectsNode != null)
			{
				if (data.teleportStarted)
				{
					PlayStartEffect(teleportModuleEffectsNode.get_ID());
					PlayTrailEffect(teleportModuleEffectsNode.get_ID());
				}
				else
				{
					PlayEndEffect(teleportModuleEffectsNode.get_ID());
				}
			}
		}

		private GameObject OnParticleEffectAllocation()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentPrefab);
			val.get_transform().set_position(new Vector3(0f, 1000f, 0f));
			val.set_name(_currentPrefab.get_name());
			return val;
		}

		private GameObject OnTrailEffectAllocation()
		{
			GameObject val = gameObjectPool.AddRecycleOnDisableForLoopParticles(_currentPrefab, _teleportTime + 1f);
			val.set_name(_currentPrefab.get_name());
			return val;
		}

		private GameObject OnCenterEffectAllocation()
		{
			GameObject val = gameObjectPool.AddRecycleOnDisableForLoopParticles(_currentPrefab, _teleportTime);
			val.set_name(_currentPrefab.get_name());
			return val;
		}
	}
}
