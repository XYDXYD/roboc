using Fabric;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal sealed class HealingProjectileImpactEffectEngine : SingleEntityViewEngine<HealingProjectileEffectNode>
	{
		private GameObject _currentPrefab;

		private readonly Func<GameObject> _onAudioInit;

		private readonly Func<GameObject> _onImpactEffectAllocation;

		private Dictionary<ItemDescriptor, HealingProjectileEffectNode> _effectsPerCategory = new Dictionary<ItemDescriptor, HealingProjectileEffectNode>();

		[Inject]
		internal GameObjectPool gameObjectPool
		{
			get;
			private set;
		}

		public unsafe HealingProjectileImpactEffectEngine(NetworkHitEffectObserver networkFireObserver)
		{
			networkFireObserver.AddAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_onImpactEffectAllocation = OnImpactEffectAllocation;
			_onAudioInit = OnAudioInit;
		}

		protected override void Add(HealingProjectileEffectNode node)
		{
			if (node.ownerComponent.ownedByMe || node.ownerComponent.ownedByAi)
			{
				IHitSomethingComponent hitSomethingComponent = node.hitSomethingComponent;
				hitSomethingComponent.hitAlly.subscribers += OnHitAlly;
				hitSomethingComponent.hitEnemy.subscribers += OnHitOther;
				hitSomethingComponent.hitSelf.subscribers += OnHitMiss;
				hitSomethingComponent.hitEnvironment.subscribers += OnHitOther;
				hitSomethingComponent.hitProtonium.subscribers += OnHitOther;
				hitSomethingComponent.hitFusionShield.subscribers += OnHitOther;
				hitSomethingComponent.hitEqualizer.subscribers += OnHitOther;
			}
			_effectsPerCategory[node.itemDescriptorComponent.itemDescriptor] = node;
		}

		protected override void Remove(HealingProjectileEffectNode node)
		{
			if (node.ownerComponent.ownedByMe || node.ownerComponent.ownedByAi)
			{
				IHitSomethingComponent hitSomethingComponent = node.hitSomethingComponent;
				hitSomethingComponent.hitAlly.subscribers -= OnHitAlly;
				hitSomethingComponent.hitEnemy.subscribers -= OnHitOther;
				hitSomethingComponent.hitSelf.subscribers -= OnHitMiss;
				hitSomethingComponent.hitEnvironment.subscribers -= OnHitOther;
				hitSomethingComponent.hitProtonium.subscribers -= OnHitOther;
				hitSomethingComponent.hitFusionShield.subscribers -= OnHitOther;
				hitSomethingComponent.hitEqualizer.subscribers -= OnHitOther;
			}
		}

		private void OnNetworkHit(ref HitInfo hitInfo)
		{
			ItemDescriptor itemDescriptor = hitInfo.itemDescriptor;
			if (!_effectsPerCategory.ContainsKey(itemDescriptor))
			{
				return;
			}
			HealingProjectileEffectNode projectile = _effectsPerCategory[hitInfo.itemDescriptor];
			if (hitInfo.targetType == TargetType.Player)
			{
				if (hitInfo.hitAlly != hitInfo.isEnemy)
				{
					OnSuccessfulHit(projectile, ref hitInfo);
				}
				else
				{
					OnHitOther(projectile, ref hitInfo);
				}
			}
			else
			{
				OnHitOther(projectile, ref hitInfo);
			}
		}

		private void OnHitAlly(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			OnSuccessfulHit(_effectsPerCategory[hitInfo.itemDescriptor], ref hitInfo);
		}

		private void OnHitOther(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			OnHitOther(_effectsPerCategory[hitInfo.itemDescriptor], ref hitInfo);
		}

		private void OnSuccessfulHit(HealingProjectileEffectNode projectile, ref HitInfo hitInfo)
		{
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_005a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			GameObject prefab = (!hitInfo.isEnemy) ? projectile.impactSuccessfulComponent.prefab : projectile.impactSuccessfulComponent.prefab_E;
			PlayImpactEffect(GetImpactSuccessfulEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			if (hitInfo.shooterIsMe)
			{
				PlayImpactAudio(projectile.impactSuccessfulComponent.audioEvent, hitInfo.hitPos);
			}
			else if (hitInfo.targetIsMe)
			{
				PlayImpactAudio(projectile.impactSuccessfulComponent.audioEventHitMe, hitInfo.hitPos);
			}
			else
			{
				PlayImpactAudio(projectile.impactSuccessfulComponent.audioEventEnemyHitOther, hitInfo.hitPos);
			}
		}

		private void OnHitOther(HealingProjectileEffectNode projectile, ref HitInfo hitInfo)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			GameObject prefab = projectile.hitEnvironmentComponent.prefab;
			PlayImpactEffect(GetImpactEnvironmentEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			PlayImpactAudio(projectile.hitEnvironmentComponent.audioEvent, hitInfo.hitPos);
		}

		private void OnHitMiss(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			OnHitMiss(_effectsPerCategory[hitInfo.itemDescriptor], ref hitInfo);
		}

		private void OnHitMiss(HealingProjectileEffectNode projectile, ref HitInfo hitInfo)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			GameObject prefab = projectile.hitMissComponent.prefab;
			PlayImpactEffect(GetImpactEnvironmentEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			PlayImpactAudio(projectile.hitMissComponent.audioEvent, hitInfo.hitPos);
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

		private void PlayImpactAudio(string audioOnImpact, Vector3 hitPosition)
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = gameObjectPool.Use(1, _onAudioInit);
			val.SetActive(true);
			val.get_transform().set_position(hitPosition);
			EventManager.get_Instance().PostEvent(audioOnImpact, 0, (object)null, val);
		}

		private GameObject GetImpactSuccessfulEffect(GameObject prefab)
		{
			_currentPrefab = prefab;
			return gameObjectPool.Use(_currentPrefab.get_name(), _onImpactEffectAllocation);
		}

		private GameObject GetImpactEnvironmentEffect(GameObject prefab)
		{
			_currentPrefab = prefab;
			return gameObjectPool.Use(_currentPrefab.get_name(), _onImpactEffectAllocation);
		}

		private GameObject OnImpactEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentPrefab);
		}

		private GameObject OnAudioInit()
		{
			return gameObjectPool.AddRecycleOnDisableForAudio();
		}
	}
}
