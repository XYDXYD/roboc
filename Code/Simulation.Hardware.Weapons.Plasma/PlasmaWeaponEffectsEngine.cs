using Fabric;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Plasma
{
	internal sealed class PlasmaWeaponEffectsEngine : SingleEntityViewEngine<PlasmaWeaponEffectsNode>, IWaitForFrameworkDestruction
	{
		private readonly Func<GameObject> _onAudioInit;

		private readonly Func<GameObject> _onImpactSuccessfulEffectAllocation;

		private readonly Func<GameObject> _onImpactSelfEffectAllocation;

		private readonly Func<GameObject> _onImpactProtoniumEffectAllocation;

		private readonly Func<GameObject> _onImpactFusionShieldEffectAllocation;

		private readonly Func<GameObject> _onImpactSecondaryEffectAllocation;

		private readonly Func<GameObject> _onImpactEqualizerEffectAllocation;

		private GameObject _currentImpactSuccessfulPrefab;

		private GameObject _currentImpactSelfPrefab;

		private GameObject _currentImpactProtoniumPrefab;

		private GameObject _currentImpactFusionShieldPrefab;

		private GameObject _currentImpactSecondaryPrefab;

		private GameObject _currentImpactEqualizerPrefab;

		private Dictionary<ItemDescriptor, PlasmaWeaponEffectsNode> _effectsPerCategory = new Dictionary<ItemDescriptor, PlasmaWeaponEffectsNode>();

		private NetworkHitEffectObserver _networkFireObserver;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		public PlasmaWeaponEffectsEngine()
		{
			_onAudioInit = OnAudioInit;
			_onImpactSuccessfulEffectAllocation = OnImpactSuccessfulEffectAllocation;
			_onImpactSelfEffectAllocation = OnImpactSelfEffectAllocation;
			_onImpactProtoniumEffectAllocation = OnImpactProtoniumEffectAllocation;
			_onImpactEqualizerEffectAllocation = OnImpactEqualizerEffectAllocation;
			_onImpactFusionShieldEffectAllocation = OnImpactFusionShieldEffectAllocation;
			_onImpactSecondaryEffectAllocation = OnImpactSecondaryEffectAllocation;
		}

		public unsafe PlasmaWeaponEffectsEngine(NetworkHitEffectObserver networkFireObserver)
			: this()
		{
			_networkFireObserver = networkFireObserver;
			_networkFireObserver.AddAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(PlasmaWeaponEffectsNode obj)
		{
			_effectsPerCategory[obj.itemDescriptorComponent.itemDescriptor] = obj;
			PreallocateEffects(obj, obj.ownerComponent.isEnemy);
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				IHitSomethingComponent hitSomethingComponent = obj.hitSomethingComponent;
				hitSomethingComponent.hitEnemy.subscribers += OnHitEnemy;
				hitSomethingComponent.hitEnvironment.subscribers += OnHitEnemy;
				hitSomethingComponent.hitSelf.subscribers += OnHitSelf;
				hitSomethingComponent.hitProtonium.subscribers += OnHitProtonium;
				hitSomethingComponent.hitFusionShield.subscribers += OnHitFusionShield;
				hitSomethingComponent.hitSecondaryImpact.subscribers += OnHitSecondaryImpact;
				hitSomethingComponent.hitEqualizer.subscribers += OnHitEqualizer;
			}
		}

		protected override void Remove(PlasmaWeaponEffectsNode obj)
		{
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				IHitSomethingComponent hitSomethingComponent = obj.hitSomethingComponent;
				hitSomethingComponent.hitEnemy.subscribers -= OnHitEnemy;
				hitSomethingComponent.hitEnvironment.subscribers -= OnHitEnemy;
				hitSomethingComponent.hitSelf.subscribers -= OnHitSelf;
				hitSomethingComponent.hitProtonium.subscribers -= OnHitProtonium;
				hitSomethingComponent.hitFusionShield.subscribers -= OnHitFusionShield;
				hitSomethingComponent.hitSecondaryImpact.subscribers -= OnHitSecondaryImpact;
				hitSomethingComponent.hitEqualizer.subscribers -= OnHitEqualizer;
			}
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_networkFireObserver.RemoveAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleRemoteHitEvent(ref HitInfo hitInfo)
		{
			ItemDescriptor itemDescriptor = hitInfo.itemDescriptor;
			if (!_effectsPerCategory.ContainsKey(itemDescriptor))
			{
				return;
			}
			switch (hitInfo.targetType)
			{
			case TargetType.Player:
				if (hitInfo.hitSelf)
				{
					OnHitSelf(null, hitInfo);
					break;
				}
				OnHitEnemy(null, hitInfo);
				OnHitSecondaryImpact(null, hitInfo);
				break;
			case TargetType.Environment:
				OnHitEnemy(null, hitInfo);
				break;
			case TargetType.FusionShield:
				OnHitFusionShield(null, hitInfo);
				break;
			case TargetType.TeamBase:
				OnHitEnemy(null, hitInfo);
				OnHitProtonium(null, hitInfo);
				break;
			case TargetType.EqualizerCrystal:
				OnHitEnemy(null, hitInfo);
				if (!hitInfo.isMiss)
				{
					OnHitEqualizer(null, hitInfo);
				}
				break;
			}
		}

		private void PreallocateEffects(PlasmaWeaponEffectsNode node, bool isEnemy)
		{
			if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
			{
				_currentImpactProtoniumPrefab = ((!isEnemy) ? node.impactProtonium.prefab : node.impactProtonium.prefab_E);
				gameObjectPool.Preallocate(_currentImpactProtoniumPrefab.get_name(), 1, _onImpactProtoniumEffectAllocation);
				_currentImpactEqualizerPrefab = ((!isEnemy) ? node.impactEqualizer.prefab : node.impactEqualizer.prefab_E);
				gameObjectPool.Preallocate(_currentImpactEqualizerPrefab.get_name(), 1, _onImpactEqualizerEffectAllocation);
			}
			if (isEnemy)
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab_E;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab_E;
				_currentImpactSecondaryPrefab = node.impactSecondary.prefab_E;
			}
			else
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab;
				_currentImpactSecondaryPrefab = node.impactSecondary.prefab;
			}
			_currentImpactSelfPrefab = node.impactSelf.prefab;
			gameObjectPool.Preallocate(_currentImpactSuccessfulPrefab.get_name(), 1, _onImpactSuccessfulEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactFusionShieldPrefab.get_name(), 1, _onImpactFusionShieldEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactSecondaryPrefab.get_name(), 1, _onImpactSecondaryEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactSelfPrefab.get_name(), 1, _onImpactSelfEffectAllocation);
			gameObjectPool.Preallocate(1, 1, _onAudioInit);
		}

		private static Quaternion GetEffectRotation(Vector3 normal)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			Quaternion result = Quaternion.get_identity();
			if (normal != Vector3.get_up())
			{
				Vector3 val = Vector3.Cross(normal, Vector3.get_up());
				result = Quaternion.LookRotation(val, normal);
			}
			return result;
		}

		private void OnHitEnemy(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			PlasmaWeaponEffectsNode plasmaWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? plasmaWeaponEffectsNode.impactSuccessful.prefab : plasmaWeaponEffectsNode.impactSuccessful.prefab_E;
			PlayImpactEffect(GetImpactSuccessfulEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			if (hitInfo.shooterIsMe)
			{
				PlayAudioEvent(plasmaWeaponEffectsNode.impactSuccessful.audioEvent, hitInfo.hitPos);
			}
			else if (isEnemy && hitInfo.targetIsMe)
			{
				PlayAudioEvent(plasmaWeaponEffectsNode.impactSuccessful.audioEventHitMe, hitInfo.hitPos);
			}
			else
			{
				PlayAudioEvent(plasmaWeaponEffectsNode.impactSuccessful.audioEventEnemyHitOther, hitInfo.hitPos);
			}
		}

		private void OnHitSelf(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			PlasmaWeaponEffectsNode plasmaWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = plasmaWeaponEffectsNode.impactSelf.prefab;
			PlayImpactEffect(GetImpactSelfEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			string audioEvent = plasmaWeaponEffectsNode.impactSelf.audioEvent;
			PlayAudioEvent(audioEvent, hitInfo.hitPos);
		}

		private void OnHitEqualizer(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			PlasmaWeaponEffectsNode plasmaWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? plasmaWeaponEffectsNode.impactEqualizer.prefab : plasmaWeaponEffectsNode.impactEqualizer.prefab_E;
			PlayImpactEffect(GetImpactEqualizerEffect(prefab), hitInfo.hitPos, hitInfo.normal);
		}

		private void OnHitProtonium(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			PlasmaWeaponEffectsNode plasmaWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? plasmaWeaponEffectsNode.impactProtonium.prefab : plasmaWeaponEffectsNode.impactProtonium.prefab_E;
			PlayImpactEffect(GetImpactProtoniumEffect(prefab), hitInfo.hitPos, hitInfo.normal);
		}

		private void OnHitFusionShield(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			PlasmaWeaponEffectsNode plasmaWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? plasmaWeaponEffectsNode.impactFusionShield.prefab : plasmaWeaponEffectsNode.impactFusionShield.prefab_E;
			PlayImpactEffect(GetImpactFusionShieldEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			string audioEvent = plasmaWeaponEffectsNode.impactFusionShield.audioEvent;
			PlayAudioEvent(audioEvent, hitInfo.hitPos);
		}

		private void OnHitSecondaryImpact(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			PlasmaWeaponEffectsNode plasmaWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? plasmaWeaponEffectsNode.impactSecondary.prefab : plasmaWeaponEffectsNode.impactSecondary.prefab_E;
			PlayImpactEffect(GetImpactSecondaryEffect(prefab), hitInfo.hitPos, hitInfo.normal);
		}

		private void PlayImpactEffect(GameObject impactParticle, Vector3 hitPos, Vector3 hitNormal)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			if (impactParticle != null)
			{
				impactParticle.SetActive(true);
				impactParticle.get_transform().set_position(hitPos);
				impactParticle.get_transform().set_rotation(GetEffectRotation(hitNormal));
			}
		}

		private void PlayAudioEvent(string audioEvent, Vector3 hitPos)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(audioEvent))
			{
				GameObject val = gameObjectPool.Use(1, _onAudioInit);
				val.SetActive(true);
				val.get_transform().set_position(hitPos);
				EventManager.get_Instance().PostEvent(audioEvent, 0, (object)null, val);
				Camera main = Camera.get_main();
				float num = 30f;
				if (main != null)
				{
					num = Vector3.Distance(hitPos, main.get_transform().get_position());
				}
				EventManager.get_Instance().SetParameter(audioEvent, "Distance", num, val);
			}
		}

		private GameObject GetImpactSuccessfulEffect(GameObject prefab)
		{
			_currentImpactSuccessfulPrefab = prefab;
			return gameObjectPool.Use(_currentImpactSuccessfulPrefab.get_name(), _onImpactSuccessfulEffectAllocation);
		}

		private GameObject GetImpactSelfEffect(GameObject prefab)
		{
			_currentImpactSelfPrefab = prefab;
			return gameObjectPool.Use(_currentImpactSelfPrefab.get_name(), _onImpactSelfEffectAllocation);
		}

		private GameObject GetImpactEqualizerEffect(GameObject prefab)
		{
			_currentImpactEqualizerPrefab = prefab;
			return gameObjectPool.Use(_currentImpactEqualizerPrefab.get_name(), _onImpactEqualizerEffectAllocation);
		}

		private GameObject GetImpactProtoniumEffect(GameObject prefab)
		{
			_currentImpactProtoniumPrefab = prefab;
			return gameObjectPool.Use(_currentImpactProtoniumPrefab.get_name(), _onImpactProtoniumEffectAllocation);
		}

		private GameObject GetImpactFusionShieldEffect(GameObject prefab)
		{
			_currentImpactFusionShieldPrefab = prefab;
			return gameObjectPool.Use(_currentImpactFusionShieldPrefab.get_name(), _onImpactFusionShieldEffectAllocation);
		}

		private GameObject GetImpactSecondaryEffect(GameObject prefab)
		{
			_currentImpactSecondaryPrefab = prefab;
			return gameObjectPool.Use(_currentImpactSecondaryPrefab.get_name(), _onImpactSecondaryEffectAllocation);
		}

		private GameObject OnAudioInit()
		{
			return gameObjectPool.AddRecycleOnDisableForAudio();
		}

		private GameObject OnImpactSuccessfulEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactSuccessfulPrefab);
		}

		private GameObject OnImpactSelfEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactSelfPrefab);
		}

		private GameObject OnImpactProtoniumEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactProtoniumPrefab);
		}

		private GameObject OnImpactFusionShieldEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactFusionShieldPrefab);
		}

		private GameObject OnImpactSecondaryEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactSecondaryPrefab);
		}

		private GameObject OnImpactEqualizerEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactEqualizerPrefab);
		}
	}
}
