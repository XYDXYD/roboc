using Fabric;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.IonDistorter
{
	internal sealed class IonDistorterEffectsEngine : SingleEntityViewEngine<IonDistorterEffectsNode>, IWaitForFrameworkDestruction
	{
		private readonly Func<GameObject> _onAudioInit;

		private readonly Func<GameObject> _onImpactEnvironmentEffectAllocation;

		private readonly Func<GameObject> _onImpactFusionShieldEffectAllocation;

		private readonly Func<GameObject> _onImpactProtoniumEffectAllocation;

		private readonly Func<GameObject> _onImpactSelfEffectAllocation;

		private readonly Func<GameObject> _onImpactSuccessfulEffectAllocation;

		private readonly Func<GameObject> _onProjectileDeadEffectAllocation;

		private readonly Func<GameObject> _onImpactEqualizerEffectAllocation;

		private GameObject _currentImpactEnvironmentPrefab;

		private GameObject _currentImpactFusionShieldPrefab;

		private GameObject _currentImpactProtoniumPrefab;

		private GameObject _currentImpactSelfPrefab;

		private GameObject _currentImpactSuccessfulPrefab;

		private GameObject _currentProjectileDeadEffectPrefab;

		private GameObject _currentImpactEqualizerPrefab;

		private Dictionary<ItemDescriptor, IonDistorterEffectsNode> _effectsPerCategory = new Dictionary<ItemDescriptor, IonDistorterEffectsNode>();

		private NetworkHitEffectObserver _networkFireObserver;

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		public IonDistorterEffectsEngine()
		{
			_onAudioInit = OnAudioInit;
			_onImpactEnvironmentEffectAllocation = OnImpactEnvironmentEffectAllocation;
			_onImpactSuccessfulEffectAllocation = OnImpactSuccessfulEffectAllocation;
			_onImpactSelfEffectAllocation = OnImpactSelfEffectAllocation;
			_onImpactProtoniumEffectAllocation = OnImpactProtoniumEffectAllocation;
			_onImpactEqualizerEffectAllocation = OnImpactEqualizerEffectAllocation;
			_onImpactFusionShieldEffectAllocation = OnImpactFusionShieldEffectAllocation;
			_onProjectileDeadEffectAllocation = OnProjectileDeadEffectAllocation;
		}

		public unsafe IonDistorterEffectsEngine(NetworkHitEffectObserver networkFireObserver)
			: this()
		{
			_networkFireObserver = networkFireObserver;
			_networkFireObserver.AddAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		protected override void Add(IonDistorterEffectsNode obj)
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

		protected override void Remove(IonDistorterEffectsNode obj)
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

		private void PreallocateEffects(IonDistorterEffectsNode node, bool isEnemy)
		{
			if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
			{
				_currentImpactProtoniumPrefab = ((!isEnemy) ? node.impactProtonium.prefab : node.impactProtonium.prefab_E);
				gameObjectPool.Preallocate(_currentImpactProtoniumPrefab.get_name(), 20, _onImpactProtoniumEffectAllocation);
				_currentImpactEqualizerPrefab = ((!isEnemy) ? node.impactEqualizer.prefab : node.impactEqualizer.prefab_E);
				gameObjectPool.Preallocate(_currentImpactEqualizerPrefab.get_name(), 20, _onImpactEqualizerEffectAllocation);
			}
			if (isEnemy)
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab_E;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab_E;
				_currentProjectileDeadEffectPrefab = node.deadEffect.prefab_E;
			}
			else
			{
				_currentImpactSuccessfulPrefab = node.impactSuccessful.prefab;
				_currentImpactFusionShieldPrefab = node.impactFusionShield.prefab;
				_currentProjectileDeadEffectPrefab = node.deadEffect.prefab;
			}
			_currentImpactEnvironmentPrefab = node.impactEnvironment.prefab;
			_currentImpactSelfPrefab = node.impactSelf.prefab;
			gameObjectPool.Preallocate(_currentImpactSuccessfulPrefab.get_name(), 20, _onImpactSuccessfulEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactFusionShieldPrefab.get_name(), 20, _onImpactFusionShieldEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactEnvironmentPrefab.get_name(), 20, _onImpactEnvironmentEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactSelfPrefab.get_name(), 20, _onImpactSelfEffectAllocation);
			gameObjectPool.Preallocate(_currentProjectileDeadEffectPrefab.get_name(), 20, _onProjectileDeadEffectAllocation);
			gameObjectPool.Preallocate(1, 20, _onAudioInit);
		}

		private void OnHitEnemy(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterEffectsNode ionDistorterEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? ionDistorterEffectsNode.impactSuccessful.prefab : ionDistorterEffectsNode.impactSuccessful.prefab_E;
			GameObject prefab2 = (!isEnemy) ? ionDistorterEffectsNode.deadEffect.prefab : ionDistorterEffectsNode.deadEffect.prefab_E;
			PlayImpactEffect(GetImpactSuccessfulEffect(prefab), GetProjectileDeadEffect(prefab2), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				if (hitInfo.shooterIsMe)
				{
					PlayAudioEvent(ionDistorterEffectsNode.impactSuccessful.audioEvent, hitInfo.hitPos);
				}
				else if (isEnemy && hitInfo.targetIsMe)
				{
					PlayAudioEvent(ionDistorterEffectsNode.impactSuccessful.audioEventHitMe, hitInfo.hitPos);
				}
				else
				{
					PlayAudioEvent(ionDistorterEffectsNode.impactSuccessful.audioEventEnemyHitOther, hitInfo.hitPos);
				}
			}
		}

		private void OnHitSelf(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterEffectsNode ionDistorterEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = ionDistorterEffectsNode.impactSelf.prefab;
			GameObject prefab2 = ionDistorterEffectsNode.deadEffect.prefab;
			PlayImpactEffect(GetImpactSelfEffect(prefab), GetProjectileDeadEffect(prefab2), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				string audioEvent = ionDistorterEffectsNode.impactSelf.audioEvent;
				PlayAudioEvent(audioEvent, hitInfo.hitPos);
			}
		}

		private void OnHitEnvironment(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0089: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterEffectsNode ionDistorterEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = ionDistorterEffectsNode.impactEnvironment.prefab;
			GameObject prefab2 = (!isEnemy) ? ionDistorterEffectsNode.deadEffect.prefab : ionDistorterEffectsNode.deadEffect.prefab_E;
			PlayImpactEffect(GetImpactEnvironmentEffect(prefab), GetProjectileDeadEffect(prefab2), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				string audioEvent = ionDistorterEffectsNode.impactEnvironment.audioEvent;
				PlayAudioEvent(audioEvent, hitInfo.hitPos);
			}
		}

		private void OnHitProtonium(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterEffectsNode ionDistorterEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? ionDistorterEffectsNode.impactProtonium.prefab : ionDistorterEffectsNode.impactProtonium.prefab_E;
			GameObject prefab2 = (!isEnemy) ? ionDistorterEffectsNode.deadEffect.prefab : ionDistorterEffectsNode.deadEffect.prefab_E;
			PlayImpactEffect(GetImpactProtoniumEffect(prefab), GetProjectileDeadEffect(prefab2), hitInfo.hitPos, hitInfo.rotation);
		}

		private void OnHitEqualizer(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterEffectsNode ionDistorterEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? ionDistorterEffectsNode.impactEqualizer.prefab : ionDistorterEffectsNode.impactEqualizer.prefab_E;
			GameObject prefab2 = (!isEnemy) ? ionDistorterEffectsNode.deadEffect.prefab : ionDistorterEffectsNode.deadEffect.prefab_E;
			PlayImpactEffect(GetImpactEqualizerEffect(prefab), GetProjectileDeadEffect(prefab2), hitInfo.hitPos, hitInfo.rotation);
		}

		private void OnHitFusionShield(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			IonDistorterEffectsNode ionDistorterEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? ionDistorterEffectsNode.impactFusionShield.prefab : ionDistorterEffectsNode.impactFusionShield.prefab_E;
			GameObject prefab2 = (!isEnemy) ? ionDistorterEffectsNode.deadEffect.prefab : ionDistorterEffectsNode.deadEffect.prefab_E;
			PlayImpactEffect(GetImpactFusionShieldEffect(prefab), GetProjectileDeadEffect(prefab2), hitInfo.hitPos, hitInfo.rotation);
			if (hitInfo.playSound)
			{
				string audioEvent = ionDistorterEffectsNode.impactFusionShield.audioEvent;
				PlayAudioEvent(audioEvent, hitInfo.hitPos);
			}
		}

		private void PlayImpactEffect(GameObject impactParticle, GameObject projectileDeadEffect, Vector3 hitPos, Quaternion rotation)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Unknown result type (might be due to invalid IL or missing references)
			if (impactParticle != null)
			{
				impactParticle.SetActive(true);
				impactParticle.get_transform().set_position(hitPos);
				impactParticle.get_transform().set_rotation(rotation);
			}
			if (projectileDeadEffect != null)
			{
				projectileDeadEffect.SetActive(true);
				projectileDeadEffect.get_transform().set_position(hitPos);
				projectileDeadEffect.get_transform().set_rotation(rotation);
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

		private GameObject GetProjectileDeadEffect(GameObject prefab)
		{
			_currentProjectileDeadEffectPrefab = prefab;
			return gameObjectPool.Use(_currentProjectileDeadEffectPrefab.get_name(), _onProjectileDeadEffectAllocation);
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

		private GameObject OnProjectileDeadEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentProjectileDeadEffectPrefab);
		}

		private GameObject OnImpactEqualizerEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactEqualizerPrefab);
		}
	}
}
