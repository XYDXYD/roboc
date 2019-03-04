using Fabric;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Tesla
{
	internal sealed class TeslaRamEffectsEngine : MultiEntityViewsEngine<TeslaRamEffectsNode, MachineStunNode>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IEngine
	{
		private const string TESLA_OPEN_ANIMATION = "Tesla_Blade_Open";

		private readonly Func<GameObject> _onAudioInit;

		private readonly Func<GameObject> _onImpactSuccessfulEffectAllocation;

		private readonly Func<GameObject> _onImpactEnvironmentEffectAllocation;

		private readonly Func<GameObject> _onImpactProtoniumEffectAllocation;

		private readonly Func<GameObject> _onImpactEqualizerEffectAllocation;

		private GameObject _currentImpactSuccessfulPrefab;

		private GameObject _currentImpactEnvironmentPrefab;

		private GameObject _currentImpactProtoniumPrefab;

		private GameObject _currentImpactEqualizerPrefab;

		private Dictionary<int, GameObject> _currentPlayingEnviromentEffects = new Dictionary<int, GameObject>();

		private Dictionary<int, TeslaRamEffectsNode> _weapons = new Dictionary<int, TeslaRamEffectsNode>();

		private Dictionary<ItemDescriptor, TeslaRamEffectsNode> _effectsPerCategory = new Dictionary<ItemDescriptor, TeslaRamEffectsNode>();

		private NetworkHitEffectObserver _networkFireObserver;

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

		public TeslaRamEffectsEngine()
		{
			_onAudioInit = OnAudioInit;
			_onImpactSuccessfulEffectAllocation = OnImpactSuccessfulEffectAllocation;
			_onImpactEnvironmentEffectAllocation = OnImpactEnvironmentEffectAllocation;
			_onImpactProtoniumEffectAllocation = OnImpactProtoniumEffectAllocation;
			_onImpactEqualizerEffectAllocation = OnImpactEqualizerEffectAllocation;
		}

		public unsafe TeslaRamEffectsEngine(NetworkHitEffectObserver networkFireObserver)
			: this()
		{
			_networkFireObserver = networkFireObserver;
			_networkFireObserver.AddAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Ready()
		{
		}

		protected override void Add(TeslaRamEffectsNode node)
		{
			_effectsPerCategory[node.itemDescriptorComponent.itemDescriptor] = node;
			PreallocateEffects(node, node.weaponOwnerComponent.isEnemy);
			_weapons.Add(node.get_ID(), node);
			IHitSomethingComponent hitComponent = node.hitComponent;
			hitComponent.hitEnemy.subscribers += OnHitEnemy;
			hitComponent.hitEnvironment.subscribers += OnHitEnvironment;
			hitComponent.hitProtonium.subscribers += OnHitProtonium;
			hitComponent.hitEqualizer.subscribers += OnHitEqualizer;
			ITeslaEffectComponent effectComponent = node.effectComponent;
			effectComponent.triggerExit.subscribers += OnEffectExit;
			node.hardwareDisabledComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnHardwareEnabled);
			node.weaponActiveComponent.onActiveChanged.NotifyOnValueSet((Action<int, bool>)OnTeslaEnabled);
			if (node.weaponActiveComponent.active)
			{
				OnTeslaEnabled(node.get_ID(), active: true);
			}
		}

		protected override void Add(MachineStunNode node)
		{
			node.stunComponent.machineStunned.subscribers += OnMachineStunnedByEMP;
			node.stunComponent.remoteMachineStunned.subscribers += OnMachineStunnedByEMP;
		}

		protected override void Remove(TeslaRamEffectsNode node)
		{
			_weapons.Remove(node.get_ID());
			EventManager.get_Instance().PostEvent(node.effectComponent.audioLoop, 1, (object)null, node.gameObjectComponent.gameObject);
			IHitSomethingComponent hitComponent = node.hitComponent;
			hitComponent.hitEnemy.subscribers -= OnHitEnemy;
			hitComponent.hitEnvironment.subscribers -= OnHitEnvironment;
			hitComponent.hitProtonium.subscribers -= OnHitProtonium;
			hitComponent.hitEqualizer.subscribers -= OnHitEqualizer;
			ITeslaEffectComponent effectComponent = node.effectComponent;
			effectComponent.triggerExit.subscribers -= OnEffectExit;
			node.hardwareDisabledComponent.isPartEnabled.StopNotify((Action<int, bool>)OnHardwareEnabled);
			node.weaponActiveComponent.onActiveChanged.StopNotify((Action<int, bool>)OnTeslaEnabled);
		}

		protected override void Remove(MachineStunNode node)
		{
			node.stunComponent.machineStunned.subscribers -= OnMachineStunnedByEMP;
			node.stunComponent.remoteMachineStunned.subscribers -= OnMachineStunnedByEMP;
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_networkFireObserver.RemoveAction(new ObserverAction<HitInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleRemoteHitEvent(ref HitInfo hitInfo)
		{
			ItemDescriptor itemDescriptor = hitInfo.itemDescriptor;
			if (_effectsPerCategory.ContainsKey(itemDescriptor))
			{
				switch (hitInfo.targetType)
				{
				case TargetType.FusionShield:
					break;
				case TargetType.Player:
					OnHitEnemy(null, hitInfo);
					break;
				case TargetType.Environment:
					OnHitEnvironment(null, hitInfo);
					break;
				case TargetType.EqualizerCrystal:
					OnHitEqualizer(null, hitInfo);
					OnHitEnemy(null, hitInfo);
					break;
				case TargetType.TeamBase:
					OnHitProtonium(null, hitInfo);
					OnHitEnemy(null, hitInfo);
					break;
				}
			}
		}

		private void PreallocateEffects(TeslaRamEffectsNode node, bool isEnemy)
		{
			if (WorldSwitching.GetGameModeType() == GameModeType.Normal)
			{
				_currentImpactProtoniumPrefab = ((!isEnemy) ? node.impactProtonium.prefab : node.impactProtonium.prefab_E);
				gameObjectPool.Preallocate(_currentImpactProtoniumPrefab.get_name(), 1, _onImpactProtoniumEffectAllocation);
				_currentImpactEqualizerPrefab = ((!isEnemy) ? node.impactEqualizer.prefab : node.impactEqualizer.prefab_E);
				gameObjectPool.Preallocate(_currentImpactEqualizerPrefab.get_name(), 2, _onImpactEqualizerEffectAllocation);
			}
			_currentImpactSuccessfulPrefab = ((!isEnemy) ? node.impactSuccessful.prefab : node.impactSuccessful.prefab_E);
			_currentImpactEnvironmentPrefab = node.impactEnvironment.prefab;
			gameObjectPool.Preallocate(_currentImpactSuccessfulPrefab.get_name(), 1, _onImpactSuccessfulEffectAllocation);
			gameObjectPool.Preallocate(_currentImpactEnvironmentPrefab.get_name(), 1, _onImpactEnvironmentEffectAllocation);
		}

		private void OnHitEnemy(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			TeslaRamEffectsNode teslaRamEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			bool isEnemy = hitInfo.isEnemy;
			GameObject prefab = (!isEnemy) ? teslaRamEffectsNode.impactSuccessful.prefab : teslaRamEffectsNode.impactSuccessful.prefab_E;
			PlayImpactEffect(GetImpactSuccessfulEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			if (teslaRamEffectsNode.weaponOwnerComponent.ownedByMe)
			{
				PlayImpactAudio(teslaRamEffectsNode.impactSuccessful.audioEvent, hitInfo.hitPos);
			}
			else if (isEnemy && hitInfo.targetIsMe)
			{
				PlayImpactAudio(teslaRamEffectsNode.impactSuccessful.audioEventHitMe, hitInfo.hitPos);
			}
			else
			{
				PlayImpactAudio(teslaRamEffectsNode.impactSuccessful.audioEventEnemyHitOther, hitInfo.hitPos);
			}
		}

		private void OnHitEnvironment(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
			TeslaRamEffectsNode teslaRamEffectsNode = _weapons[hitInfo.senderId];
			if (!_currentPlayingEnviromentEffects.ContainsKey(hitInfo.senderId))
			{
				GameObject prefab = teslaRamEffectsNode.impactEnvironment.prefab;
				GameObject impactEnvironmentEffect = GetImpactEnvironmentEffect(prefab);
				_currentPlayingEnviromentEffects[hitInfo.senderId] = impactEnvironmentEffect;
				_currentPlayingEnviromentEffects[hitInfo.senderId].SetActive(true);
			}
			_currentPlayingEnviromentEffects[hitInfo.senderId].get_transform().set_position(hitInfo.hitPos);
			_currentPlayingEnviromentEffects[hitInfo.senderId].get_transform().set_parent(teslaRamEffectsNode.transformComponent.T);
			PlayImpactAudio(teslaRamEffectsNode.impactEnvironment.audioEvent, hitInfo.hitPos);
		}

		private void OnHitProtonium(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			TeslaRamEffectsNode teslaRamEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? teslaRamEffectsNode.impactProtonium.prefab : teslaRamEffectsNode.impactProtonium.prefab_E;
			PlayImpactEffect(GetImpactProtoniumEffect(prefab), hitInfo.hitPos, hitInfo.normal);
			PlayImpactAudio(teslaRamEffectsNode.impactEnvironment.audioEvent, hitInfo.hitPos);
		}

		private void OnHitEqualizer(IHitSomethingComponent sender, HitInfo hitInfo)
		{
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			TeslaRamEffectsNode teslaRamEffectsNode = _effectsPerCategory[hitInfo.itemDescriptor];
			GameObject prefab = (!hitInfo.isEnemy) ? teslaRamEffectsNode.impactEqualizer.prefab : teslaRamEffectsNode.impactEqualizer.prefab_E;
			PlayImpactEffect(GetImpactEqualizerEffect(prefab), hitInfo.hitPos, hitInfo.normal);
		}

		private void OnTeslaEnabled(int id, bool active)
		{
			TeslaRamEffectsNode teslaRamEffectsNode = _weapons[id];
			ITeslaEffectComponent effectComponent = teslaRamEffectsNode.effectComponent;
			effectComponent.isOpen = active;
			PlayAnimation(effectComponent, id, effectComponent.animation, active);
			if (active)
			{
				EventManager.get_Instance().PostEvent(teslaRamEffectsNode.disabledAudio.audioOnDisabled, 1, (object)null, teslaRamEffectsNode.gameObjectComponent.gameObject);
				EventManager.get_Instance().PostEvent(teslaRamEffectsNode.enabledAudio.audioOnEnabled, 0, (object)null, teslaRamEffectsNode.gameObjectComponent.gameObject);
				EventManager.get_Instance().PostEvent(effectComponent.audioLoop, 0, (object)null, teslaRamEffectsNode.gameObjectComponent.gameObject);
			}
			else
			{
				EventManager.get_Instance().PostEvent(teslaRamEffectsNode.enabledAudio.audioOnEnabled, 1, (object)null, teslaRamEffectsNode.gameObjectComponent.gameObject);
				EventManager.get_Instance().PostEvent(effectComponent.audioLoop, 1, (object)null, teslaRamEffectsNode.gameObjectComponent.gameObject);
				EventManager.get_Instance().PostEvent(teslaRamEffectsNode.disabledAudio.audioOnDisabled, 0, (object)null, teslaRamEffectsNode.gameObjectComponent.gameObject);
			}
		}

		private void OnHardwareEnabled(int id, bool enabled)
		{
			if (!_weapons.TryGetValue(id, out TeslaRamEffectsNode value))
			{
				return;
			}
			if (enabled)
			{
				if (value.effectComponent.isOpen)
				{
					PlayAnimation(value.effectComponent, id, value.effectComponent.animation, open: true);
					EventManager.get_Instance().PostEvent(value.effectComponent.audioLoop, 0, (object)null, value.gameObjectComponent.gameObject);
				}
			}
			else
			{
				EventManager.get_Instance().PostEvent(value.effectComponent.audioLoop, 1, (object)null, value.gameObjectComponent.gameObject);
			}
		}

		private void OnMachineStunnedByEMP(IMachineStunComponent sender, int machineId)
		{
			int num = default(int);
			TeslaRamEffectsNode[] array = entityViewsDB.QueryGroupedEntityViewsAsArray<TeslaRamEffectsNode>(machineId, ref num);
			for (int i = 0; i < num; i++)
			{
				TeslaRamEffectsNode teslaRamEffectsNode = array[i];
				teslaRamEffectsNode.teslaBladesComponent.teslaBladesParentGameObject.SetActive(!sender.stunned);
			}
		}

		private void PlayAnimation(ITeslaEffectComponent enabledComponent, int nodeId, Animation animation, bool open)
		{
			if (animation.get_gameObject().get_activeInHierarchy())
			{
				AnimationState val = animation.get_Item("Tesla_Blade_Open");
				val.set_speed((float)(open ? 1 : (-1)));
				if (!animation.get_isPlaying())
				{
					val.set_time((!open) ? val.get_length() : 0f);
					animation.Play();
				}
			}
		}

		private void OnEffectExit(ITeslaEffectComponent sender, int senderId)
		{
			if (_currentPlayingEnviromentEffects.ContainsKey(senderId))
			{
				GameObject val = _currentPlayingEnviromentEffects[senderId];
				_currentPlayingEnviromentEffects.Remove(senderId);
				val.SetActive(false);
			}
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

		private GameObject OnImpactSuccessfulEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactSuccessfulPrefab);
		}

		private GameObject GetImpactEqualizerEffect(GameObject prefab)
		{
			_currentImpactEqualizerPrefab = prefab;
			return gameObjectPool.Use(_currentImpactEqualizerPrefab.get_name(), _onImpactEqualizerEffectAllocation);
		}

		private GameObject GetImpactEnvironmentEffect(GameObject prefab)
		{
			_currentImpactEnvironmentPrefab = prefab;
			return gameObjectPool.Use(_currentImpactEnvironmentPrefab.get_name(), _onImpactEnvironmentEffectAllocation);
		}

		private GameObject OnImpactEnvironmentEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactEnvironmentPrefab);
		}

		private GameObject GetImpactProtoniumEffect(GameObject prefab)
		{
			_currentImpactProtoniumPrefab = prefab;
			return gameObjectPool.Use(_currentImpactProtoniumPrefab.get_name(), _onImpactProtoniumEffectAllocation);
		}

		private GameObject OnImpactProtoniumEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactProtoniumPrefab);
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

		private GameObject OnImpactEqualizerEffectAllocation()
		{
			return gameObjectPool.AddRecycleOnDisableForParticlesGO(_currentImpactEqualizerPrefab);
		}
	}
}
