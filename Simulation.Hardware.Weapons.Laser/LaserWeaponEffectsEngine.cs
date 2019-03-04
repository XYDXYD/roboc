using Fabric;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Laser
{
	internal sealed class LaserWeaponEffectsEngine : SingleEntityViewEngine<LaserWeaponEffectsNode>, IWaitForFrameworkDestruction
	{
		private readonly Func<GameObject> _onAudioInit;

		private readonly Func<GameObject> _onImpactSuccessfulEffectAllocation;

		private readonly Func<GameObject> _onImpactSelfEffectAllocation;

		private readonly Func<GameObject> _onImpactEnvironmentEffectAllocation;

		private readonly Func<GameObject> _onImpactProtoniumEffectAllocation;

		private readonly Func<GameObject> _onImpactFusionShieldEffectAllocation;

		private readonly Func<GameObject> _onImpactEqualizerEffectAllocation;

		private GameObject _currentImpactSuccessfulPrefab;

		private GameObject _currentImpactSelfPrefab;

		private GameObject _currentImpactEnvironmentPrefab;

		private GameObject _currentImpactProtoniumPrefab;

		private GameObject _currentImpactEqualizerPrefab;

		private GameObject _currentImpactFusionShieldPrefab;

		private Dictionary<ItemDescriptor, LaserWeaponEffectsNode> _effectsPerCategory = new Dictionary<ItemDescriptor, LaserWeaponEffectsNode>();

		private NetworkHitEffectObserver _networkFireObserver;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		public LaserWeaponEffectsEngine()
		{
			_onAudioInit = OnAudioInit;
			_onImpactEnvironmentEffectAllocation = OnImpactEnvironmentEffectAllocation;
			_onImpactSuccessfulEffectAllocation = OnImpactSuccessfulEffectAllocation;
			_onImpactSelfEffectAllocation = OnImpactSelfEffectAllocation;
			_onImpactProtoniumEffectAllocation = OnImpactProtoniumEffectAllocation;
			_onImpactFusionShieldEffectAllocation = OnImpactFusionShieldEffectAllocation;
			_onImpactEqualizerEffectAllocation = OnImpactEqualizerEffectAllocation;
		}

		public unsafe LaserWeaponEffectsEngine(NetworkHitEffectObserver networkFireObserver)
			: this()
		{
			_networkFireObserver = networkFireObserver;
			_networkFireObserver.AddAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(LaserWeaponEffectsNode obj)
		{
			_effectsPerCategory[obj.itemDescriptorComponent.itemDescriptor] = obj;
			PreallocateEffects(obj, obj.ownerComponent.isEnemy);
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				IHitSomethingComponent hitSomethingComponent = obj.hitSomethingComponent;
				hitSomethingComponent.hitEnemy.subscribers += OnHitEnemy;
				hitSomethingComponent.hitSelf.subscribers += OnHitSelf;
				hitSomethingComponent.hitEnvironment.subscribers += OnHitEnvironment;
				hitSomethingComponent.hitProtonium.subscribers += OnHitProtonium;
				hitSomethingComponent.hitFusionShield.subscribers += OnHitFusionShield;
				hitSomethingComponent.hitEqualizer.subscribers += OnHitEqualizer;
			}
		}

		protected override void Remove(LaserWeaponEffectsNode obj)
		{
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				IHitSomethingComponent hitSomethingComponent = obj.hitSomethingComponent;
				hitSomethingComponent.hitEnemy.subscribers -= OnHitEnemy;
				hitSomethingComponent.hitSelf.subscribers -= OnHitSelf;
				hitSomethingComponent.hitEnvironment.subscribers -= OnHitEnvironment;
				hitSomethingComponent.hitProtonium.subscribers -= OnHitProtonium;
				hitSomethingComponent.hitFusionShield.subscribers -= OnHitFusionShield;
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
				}
				else
				{
					OnHitEnemy(null, hitInfo);
				}
				break;
			case TargetType.Environment:
				OnHitEnvironment(null, hitInfo);
				break;
			case TargetType.FusionShield:
				OnHitFusionShield(null, hitInfo);
				break;
			case TargetType.TeamBase:
				OnHitProtonium(null, hitInfo);
				break;
			case TargetType.EqualizerCrystal:
				if (hitInfo.isMiss)
				{
					OnHitSelf(null, hitInfo);
				}
				else
				{
					OnHitEqualizer(null, hitInfo);
				}
				break;
			}
		}

		private void PreallocateEffects(LaserWeaponEffectsNode node, bool isEnemy)
		{
			if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
			{
				_currentImpactProtoniumPrefab = ((!isEnemy) ? node.impactProtonium.prefab : node.impactProtonium.prefab_E);
				gameObjectPool.Preallocate(_currentImpactProtoniumPrefab.get_name(), 2, _onImpactProtoniumEffectAllocation);
				_currentImpactEqualizerPrefab = ((!isEnemy) ? node.impactEqualizer.prefab : node.impactEqualizer.prefab_E);
				gameObjectPool.Preallocate(_currentImpactEqualizerPrefab.get_name(), 2, _onImpactEqualizerEffectAllocation);
			}
			if (isEnemy)
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab_E;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab_E;
			}
			else
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab;
			}
			_currentImpactEnvironmentPrefab = node.impactEnvironment.prefab;
			_currentImpactSelfPrefab = node.impactSelf.prefab;
			gameObjectPool.Preallocate(_currentImpactSuccessfulPrefab.get_name(), 2, _onImpactSuccessfulEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactFusionShieldPrefab.get_name(), 2, _onImpactFusionShieldEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactEnvironmentPrefab.get_name(), 2, _onImpactEnvironmentEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactSelfPrefab.get_name(), 2, _onImpactSelfEffectAllocation);
			gameObjectPool.Preallocate(1, 1, _onAudioInit);
		}

		private void OnHitEnemy(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			LaserWeaponEffectsNode laserWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? laserWeaponEffectsNode.impactSuccessful.prefab : laserWeaponEffectsNode.impactSuccessful.prefab_E;
			PlayImpactEffect(GetImpactSuccessfulEffect(prefab), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				if (hitInfo.shooterIsMe)
				{
					PlayAudioEvent(laserWeaponEffectsNode.impactSuccessful.audioEvent, hitInfo.hitPos);
				}
				else if (isEnemy && hitInfo.targetIsMe)
				{
					PlayAudioEvent(laserWeaponEffectsNode.impactSuccessful.audioEventHitMe, hitInfo.hitPos);
				}
				else
				{
					PlayAudioEvent(laserWeaponEffectsNode.impactSuccessful.audioEventEnemyHitOther, hitInfo.hitPos);
				}
			}
		}

		private void OnHitSelf(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			LaserWeaponEffectsNode laserWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = laserWeaponEffectsNode.impactSelf.prefab;
			PlayImpactEffect(GetImpactSelfEffect(prefab), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				string audioEvent = laserWeaponEffectsNode.impactSelf.audioEvent;
				PlayAudioEvent(audioEvent, hitInfo.hitPos);
			}
		}

		private void OnHitEnvironment(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			LaserWeaponEffectsNode laserWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = laserWeaponEffectsNode.impactEnvironment.prefab;
			PlayImpactEffect(GetImpactEnvironmentEffect(prefab), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				string audioEvent = laserWeaponEffectsNode.impactEnvironment.audioEvent;
				PlayAudioEvent(audioEvent, hitInfo.hitPos);
			}
		}

		private void OnHitProtonium(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			LaserWeaponEffectsNode laserWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? laserWeaponEffectsNode.impactProtonium.prefab : laserWeaponEffectsNode.impactProtonium.prefab_E;
			PlayImpactEffect(GetImpactProtoniumEffect(prefab), hitInfo.hitPos, hitInfo.rotation);
		}

		private void OnHitEqualizer(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			LaserWeaponEffectsNode laserWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? laserWeaponEffectsNode.impactEqualizer.prefab : laserWeaponEffectsNode.impactEqualizer.prefab_E;
			PlayImpactEffect(GetImpactEqualizerEffect(prefab), hitInfo.hitPos, hitInfo.rotation);
		}

		private void OnHitFusionShield(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			LaserWeaponEffectsNode laserWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? laserWeaponEffectsNode.impactFusionShield.prefab : laserWeaponEffectsNode.impactFusionShield.prefab_E;
			PlayImpactEffect(GetImpactFusionShieldEffect(prefab), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				string audioEvent = laserWeaponEffectsNode.impactFusionShield.audioEvent;
				PlayAudioEvent(audioEvent, hitInfo.hitPos);
			}
		}

		private void PlayImpactEffect(GameObject impactParticle, Vector3 hitPos, Quaternion rotation)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (impactParticle != null)
			{
				impactParticle.SetActive(true);
				impactParticle.get_transform().set_position(hitPos);
				impactParticle.get_transform().set_rotation(rotation);
			}
		}

		private void PlayAudioEvent(string audioEvent, Vector3 hitPos)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(audioEvent))
			{
				GameObject val = gameObjectPool.Use(1, _onAudioInit);
				val.SetActive(true);
				val.get_transform().set_position(hitPos);
				EventManager.get_Instance().PostEvent(audioEvent, 0, (object)null, val);
			}
		}

		private GameObject GetImpactSuccessfulEffect(GameObject prefab)
		{
			_currentImpactSuccessfulPrefab = prefab;
			return gameObjectPool.Use(_currentImpactSuccessfulPrefab.get_name(), _onImpactSuccessfulEffectAllocation);
		}

		private GameObject GetImpactEnvironmentEffect(GameObject prefab)
		{
			_currentImpactEnvironmentPrefab = prefab;
			return gameObjectPool.Use(_currentImpactEnvironmentPrefab.get_name(), _onImpactEnvironmentEffectAllocation);
		}

		private GameObject GetImpactSelfEffect(GameObject prefab)
		{
			_currentImpactSelfPrefab = prefab;
			return gameObjectPool.Use(_currentImpactSelfPrefab.get_name(), _onImpactSelfEffectAllocation);
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

		private GameObject GetImpactEqualizerEffect(GameObject prefab)
		{
			_currentImpactEqualizerPrefab = prefab;
			return gameObjectPool.Use(_currentImpactEqualizerPrefab.get_name(), _onImpactEqualizerEffectAllocation);
		}

		private GameObject OnAudioInit()
		{
			return gameObjectPool.AddRecycleOnDisableForAudio();
		}

		private GameObject OnImpactEnvironmentEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactEnvironmentPrefab);
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

		private GameObject OnImpactEqualizerEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactEqualizerPrefab);
		}
	}
}
