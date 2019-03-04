using Fabric;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.AeroFlak
{
	internal sealed class AeroflakWeaponEffectsEngine : SingleEntityViewEngine<AeroflakWeaponEffectsNode>, IWaitForFrameworkDestruction
	{
		private readonly Func<GameObject> _onAudioInit;

		private readonly Func<GameObject> _onImpactSuccessfulEffectAllocation;

		private readonly Func<ParticleSystemUpdateBehaviour> _onImpactExplosionEffectAllocation;

		private readonly Func<GameObject> _onImpactSelfEffectAllocation;

		private readonly Func<GameObject> _onImpactEnvironmentEffectAllocation;

		private readonly Func<GameObject> _onImpactProtoniumEffectAllocation;

		private readonly Func<GameObject> _onImpactFusionShieldEffectAllocation;

		private readonly Func<GameObject> _onImpactEqualizerEffectAllocation;

		private GameObject _currentImpactEqualizerPrefab;

		private GameObject _currentImpactSuccessfulPrefab;

		private GameObject _currentImpactExplosionPrefab;

		private GameObject _currentImpactSelfPrefab;

		private GameObject _currentImpactEnvironmentPrefab;

		private GameObject _currentImpactProtoniumPrefab;

		private GameObject _currentImpactFusionShieldPrefab;

		private Dictionary<ItemDescriptor, AeroflakWeaponEffectsNode> _effectsPerCategory = new Dictionary<ItemDescriptor, AeroflakWeaponEffectsNode>();

		private NetworkHitEffectObserver _networkFireObserver;

		[Inject]
		public GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		[Inject]
		public ParticleSystemUpdaterObjectPool particleSystemUpdaterObjectPool
		{
			get;
			private set;
		}

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			get;
			private set;
		}

		[Inject]
		public MachinePreloader machinePreloader
		{
			get;
			private set;
		}

		public AeroflakWeaponEffectsEngine()
		{
			_onAudioInit = OnAudioInit;
			_onImpactEnvironmentEffectAllocation = OnImpactEnvironmentEffectAllocation;
			_onImpactSuccessfulEffectAllocation = OnImpactSuccessfulEffectAllocation;
			_onImpactExplosionEffectAllocation = OnImpactExplosionEffectAllocation;
			_onImpactSelfEffectAllocation = OnImpactSelfEffectAllocation;
			_onImpactProtoniumEffectAllocation = OnImpactProtoniumEffectAllocation;
			_onImpactFusionShieldEffectAllocation = OnImpactFusionShieldEffectAllocation;
			_onImpactEqualizerEffectAllocation = OnImpactEqualizerEffectAllocation;
		}

		public unsafe AeroflakWeaponEffectsEngine(NetworkHitEffectObserver networkFireObserver)
			: this()
		{
			_networkFireObserver = networkFireObserver;
			_networkFireObserver.AddAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(AeroflakWeaponEffectsNode obj)
		{
			_effectsPerCategory[obj.itemDescriptorComponent.itemDescriptor] = obj;
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				RegisterEvents(obj.hitSomethingComponent);
			}
			PreallocateEffects(obj, obj.ownerComponent.isEnemy);
		}

		protected override void Remove(AeroflakWeaponEffectsNode obj)
		{
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				IHitSomethingComponent hitSomethingComponent = obj.hitSomethingComponent;
				hitSomethingComponent.hitEnemy.subscribers -= OnHitEnemy;
				hitSomethingComponent.hitEnemySplash.subscribers -= OnHitEnemySplash;
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
				if (hitInfo.hit)
				{
					OnHitEnemySplash(null, hitInfo);
				}
				else
				{
					OnHitEnvironment(null, hitInfo);
				}
				break;
			case TargetType.FusionShield:
				OnHitFusionShield(null, hitInfo);
				break;
			case TargetType.TeamBase:
				OnHitProtonium(null, hitInfo);
				break;
			case TargetType.EqualizerCrystal:
				if (!hitInfo.isMiss)
				{
					OnHitEqualizer(null, hitInfo);
				}
				else
				{
					OnHitSelf(null, hitInfo);
				}
				break;
			}
		}

		private void RegisterEvents(IHitSomethingComponent hitComponent)
		{
			hitComponent.hitEnemy.subscribers += OnHitEnemy;
			hitComponent.hitEnemySplash.subscribers += OnHitEnemySplash;
			hitComponent.hitSelf.subscribers += OnHitSelf;
			hitComponent.hitEnvironment.subscribers += OnHitEnvironment;
			hitComponent.hitProtonium.subscribers += OnHitProtonium;
			hitComponent.hitFusionShield.subscribers += OnHitFusionShield;
			hitComponent.hitEqualizer.subscribers += OnHitEqualizer;
		}

		private void PreallocateEffects(AeroflakWeaponEffectsNode node, bool isEnemy)
		{
			if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
			{
				_currentImpactProtoniumPrefab = ((!isEnemy) ? node.impactProtonium.prefab : node.impactProtonium.prefab_E);
				gameObjectPool.Preallocate(_currentImpactProtoniumPrefab.get_name(), 1, _onImpactProtoniumEffectAllocation);
				_currentImpactEqualizerPrefab = ((!isEnemy) ? node.impactEqualizer.prefab : node.impactEqualizer.prefab_E);
				gameObjectPool.Preallocate(_currentImpactEqualizerPrefab.get_name(), 2, _onImpactEqualizerEffectAllocation);
			}
			if (isEnemy)
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab_E;
				_currentImpactExplosionPrefab = node.impactExplosion.prefab_E;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab_E;
			}
			else
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab;
				_currentImpactExplosionPrefab = node.impactExplosion.prefab;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab;
			}
			_currentImpactEnvironmentPrefab = node.impactEnvironment.prefab;
			_currentImpactSelfPrefab = node.impactSelf.prefab;
			gameObjectPool.Preallocate(_currentImpactSuccessfulPrefab.get_name(), 1, _onImpactSuccessfulEffectAllocation);
			particleSystemUpdaterObjectPool.Preallocate(_currentImpactExplosionPrefab.get_name(), 1, _onImpactExplosionEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactFusionShieldPrefab.get_name(), 1, _onImpactFusionShieldEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactEnvironmentPrefab.get_name(), 1, _onImpactEnvironmentEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactSelfPrefab.get_name(), 1, _onImpactSelfEffectAllocation);
			gameObjectPool.Preallocate(1, 1, _onAudioInit);
		}

		private void OnHitEnemy(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
			//IL_012f: Unknown result type (might be due to invalid IL or missing references)
			//IL_014f: Unknown result type (might be due to invalid IL or missing references)
			AeroflakWeaponEffectsNode aeroflakWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			Vector3 hitPos = hitInfo.hitPos;
			Quaternion rotation = hitInfo.rotation;
			bool flag = GameUtility.MachineIsOnGround(hitInfo.target, hitInfo.targetType, aeroflakWeaponEffectsNode.aeroflakStats.groundClearance, machineRootContainer, machinePreloader);
			IProjectileEffectImpactSuccessfulComponent impactSuccessful = aeroflakWeaponEffectsNode.impactSuccessful;
			if (!flag)
			{
				GameObject prefab = (!isEnemy) ? aeroflakWeaponEffectsNode.impactExplosion.prefab : aeroflakWeaponEffectsNode.impactExplosion.prefab_E;
				ParticleSystemUpdateBehaviour impactExplosionEffect = GetImpactExplosionEffect(prefab);
				impactExplosionEffect.SetParticleEmissionPercent((float)hitInfo.stackCountPercent / 100f);
				PlayImpactEffect(impactExplosionEffect.get_gameObject(), hitPos, rotation);
			}
			else
			{
				GameObject prefab = (!isEnemy) ? impactSuccessful.prefab : impactSuccessful.prefab_E;
				PlayImpactEffect(GetImpactSuccessfulEffect(prefab), hitPos, rotation);
			}
			if (hitInfo.shooterIsMe)
			{
				PlayAudioEvent(impactSuccessful.audioEvent, hitInfo.hitPos, hitInfo.stackCountPercent);
			}
			else if (isEnemy && hitInfo.targetIsMe)
			{
				PlayAudioEvent(impactSuccessful.audioEventHitMe, hitInfo.hitPos, hitInfo.stackCountPercent);
			}
			else
			{
				PlayAudioEvent(impactSuccessful.audioEventEnemyHitOther, hitInfo.hitPos, hitInfo.stackCountPercent);
			}
		}

		private void OnHitEnemySplash(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_011f: Unknown result type (might be due to invalid IL or missing references)
			AeroflakWeaponEffectsNode aeroflakWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			Vector3 hitPos = hitInfo.hitPos;
			Quaternion rotation = hitInfo.rotation;
			IProjectileEffectExplosionComponent impactExplosion = aeroflakWeaponEffectsNode.impactExplosion;
			GameObject prefab = (!isEnemy) ? impactExplosion.prefab : impactExplosion.prefab_E;
			ParticleSystemUpdateBehaviour impactExplosionEffect = GetImpactExplosionEffect(prefab);
			impactExplosionEffect.SetParticleEmissionPercent((float)hitInfo.stackCountPercent / 100f);
			PlayImpactEffect(impactExplosionEffect.get_gameObject(), hitPos, rotation);
			if (hitInfo.targetType == TargetType.Environment)
			{
				PlayAudioEvent(impactExplosion.audioEvent, hitPos, hitInfo.stackCountPercent);
			}
			else if (hitInfo.targetType == TargetType.Player)
			{
				IProjectileEffectImpactSuccessfulComponent impactSuccessful = aeroflakWeaponEffectsNode.impactSuccessful;
				if (hitInfo.shooterIsMe)
				{
					PlayAudioEvent(impactSuccessful.audioEvent, hitInfo.hitPos, hitInfo.stackCountPercent);
				}
				else if (isEnemy && hitInfo.targetIsMe)
				{
					PlayAudioEvent(impactSuccessful.audioEventHitMe, hitInfo.hitPos, hitInfo.stackCountPercent);
				}
				else
				{
					PlayAudioEvent(impactSuccessful.audioEventEnemyHitOther, hitInfo.hitPos, hitInfo.stackCountPercent);
				}
			}
		}

		private void SetAudioParameter(string audioEvent, string parameterName, int stackCount, GameObject parent)
		{
			EventManager.get_Instance().SetParameter(audioEvent, parameterName, (float)stackCount / 100f, parent);
		}

		private void OnHitSelf(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			AeroflakWeaponEffectsNode aeroflakWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			Vector3 hitPos = hitInfo.hitPos;
			Quaternion rotation = hitInfo.rotation;
			IProjectileEffectImpactSelfComponent impactSelf = aeroflakWeaponEffectsNode.impactSelf;
			GameObject prefab = impactSelf.prefab;
			PlayImpactEffect(GetImpactSelfEffect(prefab), hitPos, rotation);
			PlayAudioEvent(impactSelf.audioEvent, hitPos, hitInfo.stackCountPercent);
		}

		private void OnHitEnvironment(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			AeroflakWeaponEffectsNode aeroflakWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			Vector3 hitPos = hitInfo.hitPos;
			Quaternion rotation = hitInfo.rotation;
			if (hitInfo.hit)
			{
				GameObject prefab = (!isEnemy) ? aeroflakWeaponEffectsNode.impactExplosion.prefab : aeroflakWeaponEffectsNode.impactExplosion.prefab_E;
				ParticleSystemUpdateBehaviour impactExplosionEffect = GetImpactExplosionEffect(prefab);
				impactExplosionEffect.SetParticleEmissionPercent((float)hitInfo.stackCountPercent / 100f);
				PlayImpactEffect(impactExplosionEffect.get_gameObject(), hitPos, rotation);
			}
			else
			{
				GameObject prefab = aeroflakWeaponEffectsNode.impactEnvironment.prefab;
				PlayImpactEffect(GetImpactEnvironmentEffect(prefab), hitPos, rotation);
			}
			PlayAudioEvent(aeroflakWeaponEffectsNode.impactEnvironment.audioEvent, hitPos, hitInfo.stackCountPercent);
		}

		private void OnHitProtonium(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			AeroflakWeaponEffectsNode aeroflakWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			Vector3 hitPos = hitInfo.hitPos;
			Quaternion rotation = hitInfo.rotation;
			IProjectileEffectImpactProtoniumComponent impactProtonium = aeroflakWeaponEffectsNode.impactProtonium;
			GameObject prefab = (!isEnemy) ? impactProtonium.prefab : impactProtonium.prefab_E;
			PlayImpactEffect(GetImpactProtoniumEffect(prefab), hitPos, rotation);
		}

		private void OnHitEqualizer(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			AeroflakWeaponEffectsNode aeroflakWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? aeroflakWeaponEffectsNode.impactEqualizer.prefab : aeroflakWeaponEffectsNode.impactEqualizer.prefab_E;
			PlayImpactEffect(GetImpactEqualizerEffect(prefab), hitInfo.hitPos, hitInfo.rotation);
		}

		private void OnHitFusionShield(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0058: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			AeroflakWeaponEffectsNode aeroflakWeaponEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			Vector3 hitPos = hitInfo.hitPos;
			Quaternion rotation = hitInfo.rotation;
			IProjectileEffectImpactFusionShieldComponent impactFusionShield = aeroflakWeaponEffectsNode.impactFusionShield;
			GameObject prefab = (!isEnemy) ? impactFusionShield.prefab : impactFusionShield.prefab_E;
			PlayImpactEffect(GetImpactFusionShieldEffect(prefab), hitPos, rotation);
			PlayAudioEvent(impactFusionShield.audioEvent, hitInfo.hitPos, hitInfo.stackCountPercent);
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

		private void PlayAudioEvent(string audioEvent, Vector3 hitPos, int intensity)
		{
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			if (!string.IsNullOrEmpty(audioEvent))
			{
				GameObject val = gameObjectPool.Use(1, _onAudioInit);
				val.SetActive(true);
				val.get_transform().set_position(hitPos);
				EventManager.get_Instance().PostEvent(audioEvent, 0, (object)null, val);
				SetAudioParameter(audioEvent, "DAMAGE", intensity, val);
			}
		}

		private GameObject GetImpactSuccessfulEffect(GameObject prefab)
		{
			_currentImpactSuccessfulPrefab = prefab;
			return gameObjectPool.Use(_currentImpactSuccessfulPrefab.get_name(), _onImpactSuccessfulEffectAllocation);
		}

		private ParticleSystemUpdateBehaviour GetImpactExplosionEffect(GameObject prefab)
		{
			_currentImpactExplosionPrefab = prefab;
			return particleSystemUpdaterObjectPool.Use(_currentImpactExplosionPrefab.get_name(), _onImpactExplosionEffectAllocation);
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

		private ParticleSystemUpdateBehaviour OnImpactExplosionEffectAllocation()
		{
			return SetPoolForParticleSystemUpdater(_currentImpactExplosionPrefab);
		}

		private ParticleSystemUpdateBehaviour SetPoolForParticleSystemUpdater(GameObject prefab)
		{
			ParticleSystemUpdateBehaviour particleSystemUpdateBehaviour = particleSystemUpdaterObjectPool.CreateGameObjectFromPrefab(prefab);
			GameObject gameObject = particleSystemUpdateBehaviour.get_gameObject();
			gameObject.SetActive(false);
			particleSystemUpdateBehaviour.SetPool(particleSystemUpdaterObjectPool, gameObject.get_name());
			return particleSystemUpdateBehaviour;
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
