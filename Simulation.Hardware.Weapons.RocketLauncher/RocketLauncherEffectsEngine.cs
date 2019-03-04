using Fabric;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal sealed class RocketLauncherEffectsEngine : SingleEntityViewEngine<RocketLauncherEffectsNode>, IWaitForFrameworkDestruction
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

		private GameObject _currentImpactFusionShieldPrefab;

		private GameObject _currentImpactEqualizerPrefab;

		private Dictionary<ItemDescriptor, RocketLauncherEffectsNode> _effectsPerCategory = new Dictionary<ItemDescriptor, RocketLauncherEffectsNode>();

		private NetworkHitEffectObserver _networkFireObserver;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		public RocketLauncherEffectsEngine()
		{
			_onAudioInit = OnAudioInit;
			_onImpactEnvironmentEffectAllocation = OnImpactEnvironmentEffectAllocation;
			_onImpactSuccessfulEffectAllocation = OnImpactSuccessfulEffectAllocation;
			_onImpactSelfEffectAllocation = OnImpactSelfEffectAllocation;
			_onImpactProtoniumEffectAllocation = OnImpactProtoniumEffectAllocation;
			_onImpactFusionShieldEffectAllocation = OnImpactFusionShieldEffectAllocation;
			_onImpactEqualizerEffectAllocation = OnImpactEqualizerEffectAllocation;
		}

		public unsafe RocketLauncherEffectsEngine(NetworkHitEffectObserver networkFireObserver)
			: this()
		{
			_networkFireObserver = networkFireObserver;
			_networkFireObserver.AddAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(RocketLauncherEffectsNode obj)
		{
			_effectsPerCategory[obj.itemDescriptorComponent.itemDescriptor] = obj;
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				RegisterEvents(obj.hitSomethingComponent);
			}
			PreallocateEffects(obj, obj.ownerComponent.isEnemy);
		}

		protected override void Remove(RocketLauncherEffectsNode obj)
		{
			if (obj.ownerComponent.ownedByMe || obj.ownerComponent.ownedByAi)
			{
				DeregisterEvents(obj.hitSomethingComponent);
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
			case TargetType.TeamBase:
				if (!hitInfo.isMiss)
				{
					OnHitProtonium(null, hitInfo);
				}
				OnHitEnvironment(null, hitInfo);
				break;
			case TargetType.EqualizerCrystal:
				if (!hitInfo.isMiss)
				{
					OnHitEqualizer(null, hitInfo);
				}
				OnHitEnvironment(null, hitInfo);
				break;
			case TargetType.FusionShield:
				OnHitFusionShield(null, hitInfo);
				break;
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
			default:
				OnHitEnvironment(null, hitInfo);
				break;
			}
		}

		private void RegisterEvents(IHitSomethingComponent hitComponent)
		{
			hitComponent.hitEnemy.subscribers += OnHitEnemy;
			hitComponent.hitSelf.subscribers += OnHitSelf;
			hitComponent.hitEnvironment.subscribers += OnHitEnvironment;
			hitComponent.hitProtonium.subscribers += OnHitProtonium;
			hitComponent.hitFusionShield.subscribers += OnHitFusionShield;
			hitComponent.hitEqualizer.subscribers += OnHitEqualizer;
		}

		private void DeregisterEvents(IHitSomethingComponent hitComponent)
		{
			hitComponent.hitEnemy.subscribers -= OnHitEnemy;
			hitComponent.hitSelf.subscribers -= OnHitSelf;
			hitComponent.hitEnvironment.subscribers -= OnHitEnvironment;
			hitComponent.hitProtonium.subscribers -= OnHitProtonium;
			hitComponent.hitFusionShield.subscribers -= OnHitFusionShield;
			hitComponent.hitEqualizer.subscribers -= OnHitEqualizer;
		}

		private void PreallocateEffects(RocketLauncherEffectsNode node, bool isEnemy)
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
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab_E;
			}
			else
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab;
			}
			_currentImpactEnvironmentPrefab = node.impactEnvironment.prefab;
			_currentImpactSelfPrefab = node.impactSelf.prefab;
			gameObjectPool.Preallocate(_currentImpactSuccessfulPrefab.get_name(), 1, _onImpactSuccessfulEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactFusionShieldPrefab.get_name(), 1, _onImpactFusionShieldEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactEnvironmentPrefab.get_name(), 1, _onImpactEnvironmentEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactSelfPrefab.get_name(), 1, _onImpactSelfEffectAllocation);
		}

		private void OnHitEnemy(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00be: Unknown result type (might be due to invalid IL or missing references)
			RocketLauncherEffectsNode rocketLauncherEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? rocketLauncherEffectsNode.impactSuccessful.prefab : rocketLauncherEffectsNode.impactSuccessful.prefab_E;
			PlayImpactEffect(GetImpactSuccessfulEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			if (hitInfo.shooterIsMe)
			{
				PlayImpactAudio(rocketLauncherEffectsNode.impactSuccessful.audioEvent, hitInfo.hitPos);
			}
			else if (isEnemy && hitInfo.targetIsMe)
			{
				PlayImpactAudio(rocketLauncherEffectsNode.impactSuccessful.audioEventHitMe, hitInfo.hitPos);
			}
			else
			{
				PlayImpactAudio(rocketLauncherEffectsNode.impactSuccessful.audioEventEnemyHitOther, hitInfo.hitPos);
			}
		}

		private void OnHitSelf(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			RocketLauncherEffectsNode rocketLauncherEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = rocketLauncherEffectsNode.impactSelf.prefab;
			PlayImpactEffect(GetImpactSelfEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			PlayImpactAudio(rocketLauncherEffectsNode.impactSelf.audioEvent, hitInfo.hitPos);
		}

		private void OnHitEnvironment(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			RocketLauncherEffectsNode rocketLauncherEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = rocketLauncherEffectsNode.impactEnvironment.prefab;
			PlayImpactEffect(GetImpactEnvironmentEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			PlayImpactAudio(rocketLauncherEffectsNode.impactEnvironment.audioEvent, hitInfo.hitPos);
		}

		private void OnHitProtonium(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			RocketLauncherEffectsNode rocketLauncherEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? rocketLauncherEffectsNode.impactProtonium.prefab : rocketLauncherEffectsNode.impactProtonium.prefab_E;
			PlayImpactEffect(GetImpactProtoniumEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			PlayImpactAudio(rocketLauncherEffectsNode.impactEnvironment.audioEvent, hitInfo.hitPos);
		}

		private void OnHitFusionShield(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			RocketLauncherEffectsNode rocketLauncherEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? rocketLauncherEffectsNode.impactFusionShield.prefab : rocketLauncherEffectsNode.impactFusionShield.prefab_E;
			PlayImpactEffect(GetImpactFusionShieldEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			PlayImpactAudio(rocketLauncherEffectsNode.impactFusionShield.audioEvent, hitInfo.hitPos);
		}

		private void OnHitEqualizer(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			RocketLauncherEffectsNode rocketLauncherEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? rocketLauncherEffectsNode.impactEqualizer.prefab : rocketLauncherEffectsNode.impactEqualizer.prefab_E;
			PlayImpactEffect(GetImpactEqualizerEffect(prefab), hitInfo.hitPos, hitInfo.normal);
		}

		private void PlayImpactEffect(GameObject impactParticle, Vector3 hitPos, Vector3 normal)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			if (impactParticle != null)
			{
				impactParticle.get_transform().set_position(hitPos);
				impactParticle.get_transform().LookAt(impactParticle.get_transform().get_position() + normal);
				impactParticle.SetActive(true);
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

		private void PlayImpactAudio(string audioOnImpact, Vector3 hitPosition)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectPool.Use(1, _onAudioInit);
			val.SetActive(true);
			val.get_transform().set_position(hitPosition);
			EventManager.get_Instance().PostEvent(audioOnImpact, 0, (object)null, val);
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
