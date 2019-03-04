using RCNetwork.Events;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using UnityEngine;

namespace Simulation.Hardware.Modules
{
	internal sealed class MachineInvisibilityEngine : MultiEntityViewsEngine<MachineInvisibilityNode, CloakModuleActivationNode>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IInitialize, IEngine
	{
		private Action<Material, int> _swapMaterialCallback;

		private Action<Material, int> _swapRemoteMaterialCallback;

		private RemotePlayerBecomeInvisibleObserver _remotePlayerBecomeInvisibleObserver;

		private RemotePlayerBecomeVisibleObserver _remotePlayerBecomeVisibleObserver;

		private WeaponFiredObserver _weaponFiredObserver;

		private CloakModuleEventDependency _cloakModuleEventDependency = new CloakModuleEventDependency();

		private int _localMachineId;

		private ManaDrainingActivationData _manaDrainingActivationData = default(ManaDrainingActivationData);

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal INetworkEventManagerClient networkManager
		{
			private get;
			set;
		}

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		internal MachineFadeEffect machineFadeEffect
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe MachineInvisibilityEngine(RemotePlayerBecomeInvisibleObserver remotePlayerBecomeInvisibleObserver, RemotePlayerBecomeVisibleObserver remotePlayerBecomeVisibleObserver, WeaponFiredObserver weaponFiredObserver)
		{
			_remotePlayerBecomeInvisibleObserver = remotePlayerBecomeInvisibleObserver;
			_remotePlayerBecomeVisibleObserver = remotePlayerBecomeVisibleObserver;
			_weaponFiredObserver = weaponFiredObserver;
			weaponFiredObserver.AddAction(new ObserverAction<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			remotePlayerBecomeInvisibleObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			remotePlayerBecomeVisibleObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_swapMaterialCallback = SwapMaterial;
			_swapRemoteMaterialCallback = SwapMaterialRemote;
		}

		void IInitialize.OnDependenciesInjected()
		{
			destructionReporter.OnPlayerDamageApplied += HandleOnPlayerTakeDamage;
			destructionReporter.OnMachineKilled += HandleOnMachineKilled;
		}

		private void HandleOnManaDrained(IManaDrainComponent sender, int machineId)
		{
			CancelPlayerInvisibility();
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerTakeDamage;
			destructionReporter.OnMachineKilled -= HandleOnMachineKilled;
			_weaponFiredObserver.RemoveAction(new ObserverAction<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_remotePlayerBecomeInvisibleObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_remotePlayerBecomeVisibleObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void HandleOnPlayerTakeDamage(DestructionData data)
		{
			if (data.shooterIsMe || data.targetIsMe)
			{
				CancelPlayerInvisibility();
			}
		}

		private void HandleOnMachineKilled(int ownerId, int shooterId)
		{
			if (ownerId == _localMachineId)
			{
				CancelPlayerInvisibility();
			}
		}

		private void HandleOnWeaponFired(ref float weaponFireCost)
		{
			CancelPlayerInvisibility();
		}

		public void MakeOtherPlayerInvisible(ref int playerId)
		{
			ToggleRemotePlayerVisibility(playerId, visible: false);
		}

		public void MakeOtherPlayerVisible(ref int playerId)
		{
			ToggleRemotePlayerVisibility(playerId, visible: true);
		}

		private void ToggleRemotePlayerVisibility(int playerId, bool visible)
		{
			//IL_0072: Unknown result type (might be due to invalid IL or missing references)
			//IL_0077: Unknown result type (might be due to invalid IL or missing references)
			int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, playerId);
			MachineInvisibilityNode machineInvisibilityNode = default(MachineInvisibilityNode);
			if (!entityViewsDB.TryQueryEntityView<MachineInvisibilityNode>(activeMachine, ref machineInvisibilityNode))
			{
				return;
			}
			if (visible)
			{
				machineFadeEffect.ToVisible(machineInvisibilityNode.cloakStatsComponent.toVisibleDuration, activeMachine);
			}
			else
			{
				machineFadeEffect.ToInvisible(machineInvisibilityNode.cloakStatsComponent.toInvisibleDuration, activeMachine, _swapRemoteMaterialCallback);
			}
			machineInvisibilityNode.machineVisibilityComponent.isVisible = visible;
			FasterReadOnlyList<CloakModuleActivationNode> val = entityViewsDB.QueryEntityViews<CloakModuleActivationNode>();
			int num = 0;
			CloakModuleActivationNode cloakModuleActivationNode;
			while (true)
			{
				if (num < val.get_Count())
				{
					cloakModuleActivationNode = val.get_Item(num);
					if (cloakModuleActivationNode.ownerComponent.machineId == activeMachine)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			ICloakAudioObjectsComponent cloakAudioComponent = cloakModuleActivationNode.cloakAudioComponent;
			cloakAudioComponent.soundObjectActive.SetActive(!visible);
			cloakAudioComponent.soundObjectInactive.SetActive(visible);
		}

		private void CancelPlayerInvisibility()
		{
			MachineInvisibilityNode machineInvisibilityNode = default(MachineInvisibilityNode);
			if (entityViewsDB.TryQueryEntityView<MachineInvisibilityNode>(_localMachineId, ref machineInvisibilityNode) && !machineInvisibilityNode.machineVisibilityComponent.isVisible)
			{
				MakePlayerMachineVisible(machineInvisibilityNode);
				machineInvisibilityNode.machineVisibilityComponent.machineBecameVisible.Dispatch(ref _localMachineId);
			}
		}

		private void HandleOnModuleActivated(IModuleActivationComponent sender, int moduleId)
		{
			CloakModuleActivationNode cloakModuleActivationNode = default(CloakModuleActivationNode);
			if (!entityViewsDB.TryQueryEntityView<CloakModuleActivationNode>(moduleId, ref cloakModuleActivationNode))
			{
				return;
			}
			int value = cloakModuleActivationNode.ownerComponent.machineId;
			MachineInvisibilityNode machineInvisibilityNode = default(MachineInvisibilityNode);
			if (entityViewsDB.TryQueryEntityView<MachineInvisibilityNode>(value, ref machineInvisibilityNode))
			{
				if (machineInvisibilityNode.machineVisibilityComponent.isVisible)
				{
					MakePlayerMachineInvisible(machineInvisibilityNode);
					machineInvisibilityNode.machineVisibilityComponent.machineBecameInvisible.Dispatch(ref value);
				}
				else
				{
					MakePlayerMachineVisible(machineInvisibilityNode);
					machineInvisibilityNode.machineVisibilityComponent.machineBecameVisible.Dispatch(ref value);
				}
			}
		}

		private void MakePlayerMachineInvisible(MachineInvisibilityNode node)
		{
			TogglePlayerVisibility(node, visible: false);
		}

		private void MakePlayerMachineVisible(MachineInvisibilityNode node)
		{
			TogglePlayerVisibility(node, visible: true);
		}

		private void TogglePlayerVisibility(MachineInvisibilityNode node, bool visible)
		{
			//IL_0091: Unknown result type (might be due to invalid IL or missing references)
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			_cloakModuleEventDependency.SetVariables(playerTeamsContainer.localPlayerId);
			if (!visible)
			{
				networkManager.SendEventToServer(NetworkEvent.BroadcastInvisible, _cloakModuleEventDependency);
				machineFadeEffect.ToInvisible(node.cloakStatsComponent.toInvisibleDuration, _localMachineId, _swapMaterialCallback);
			}
			else
			{
				networkManager.SendEventToServer(NetworkEvent.BroadcastVisible, _cloakModuleEventDependency);
				machineFadeEffect.ToVisible(node.cloakStatsComponent.toVisibleDuration, _localMachineId);
			}
			float drainRate_ = 0f;
			FasterReadOnlyList<CloakModuleActivationNode> val = entityViewsDB.QueryEntityViews<CloakModuleActivationNode>();
			for (int i = 0; i < val.get_Count(); i++)
			{
				CloakModuleActivationNode cloakModuleActivationNode = val.get_Item(i);
				if (cloakModuleActivationNode.ownerComponent.machineId == _localMachineId)
				{
					ICloakAudioObjectsComponent cloakAudioComponent = cloakModuleActivationNode.cloakAudioComponent;
					cloakAudioComponent.soundObjectActive.SetActive(!visible);
					cloakAudioComponent.soundObjectInactive.SetActive(visible);
					drainRate_ = cloakModuleActivationNode.manaCostComponent.weaponFireCost;
					break;
				}
			}
			bool activate_ = !visible;
			_manaDrainingActivationData.SetValues(activate_, _localMachineId, drainRate_);
			node.manaDrainComponent.activateManaDraining.Dispatch(ref _manaDrainingActivationData);
			node.machineVisibilityComponent.isVisible = visible;
		}

		private void SwapMaterial(Material current, int machineId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CloakModuleActivationNode> val = entityViewsDB.QueryEntityViews<CloakModuleActivationNode>();
			int num = 0;
			CloakModuleActivationNode cloakModuleActivationNode;
			while (true)
			{
				if (num < val.get_Count())
				{
					cloakModuleActivationNode = val.get_Item(num);
					if (cloakModuleActivationNode.ownerComponent.machineId == machineId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			Material fadeToMaterial = cloakModuleActivationNode.materialsComponent.fadeToMaterial;
			if (current.HasProperty("_TextureOffset"))
			{
				current.set_shader(cloakModuleActivationNode.materialsComponent.skinnedShader);
			}
			else
			{
				current.set_shader(cloakModuleActivationNode.materialsComponent.nonSkinnedShader);
			}
			SetValues(current, fadeToMaterial);
		}

		private void SwapMaterialRemote(Material current, int machineId)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<CloakModuleActivationNode> val = entityViewsDB.QueryEntityViews<CloakModuleActivationNode>();
			int num = 0;
			CloakModuleActivationNode cloakModuleActivationNode;
			while (true)
			{
				if (num < val.get_Count())
				{
					cloakModuleActivationNode = val.get_Item(num);
					if (cloakModuleActivationNode.ownerComponent.machineId == machineId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			Material remoteMaterialUsingQualitySettings = GetRemoteMaterialUsingQualitySettings(cloakModuleActivationNode.materialsComponent);
			if (current.HasProperty("_TextureOffset"))
			{
				current.set_shader(cloakModuleActivationNode.materialsComponent.skinnedShader);
			}
			else
			{
				current.set_shader(cloakModuleActivationNode.materialsComponent.nonSkinnedShader);
			}
			SetValues(current, remoteMaterialUsingQualitySettings);
		}

		private static Material GetRemoteMaterialUsingQualitySettings(ICloakMaterialsComponent component)
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
			if (qualityLevel <= component.lowQualityThreshold)
			{
				return component.fadeToMaterialRemoteLow;
			}
			return component.fadeToMaterialRemote;
		}

		private static void SetValues(Material current, Material m)
		{
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			current.SetTexture("_RefractionMask", m.GetTexture("_RefractionMask"));
			current.SetTextureScale("_RefractionMask", m.GetTextureScale("_RefractionMask"));
			current.SetTexture("_Noise_01", m.GetTexture("_Noise_01"));
			current.SetTextureScale("_Noise_01", m.GetTextureScale("_Noise_01"));
			current.SetFloat("_WhiteFade", m.GetFloat("_WhiteFade"));
			current.SetFloat("_RefractionStrength", m.GetFloat("_RefractionStrength"));
			current.SetFloat("_Noise_Strength", m.GetFloat("_Noise_Strength"));
			current.SetFloat("_VPanSpeedDistort", m.GetFloat("_VPanSpeedDistort"));
			current.SetFloat("_UPanSpeedDistort", m.GetFloat("_UPanSpeedDistort"));
			current.SetFloat("_Fresnel_Strength", m.GetFloat("_Fresnel_Strength"));
			current.SetColor("_BaseColour", m.GetColor("_BaseColour"));
		}

		public void Ready()
		{
		}

		protected override void Add(MachineInvisibilityNode moduleNode)
		{
			moduleNode.machineVisibilityComponent.isVisible = true;
			if (moduleNode.ownerComponent.ownedByMe)
			{
				moduleNode.manaDrainComponent.manaDrained.subscribers += HandleOnManaDrained;
				_localMachineId = moduleNode.get_ID();
			}
		}

		protected override void Remove(MachineInvisibilityNode moduleNode)
		{
			if (moduleNode.ownerComponent.ownedByMe)
			{
				if (!moduleNode.machineVisibilityComponent.isVisible)
				{
					MakePlayerMachineVisible(moduleNode);
					moduleNode.machineVisibilityComponent.machineBecameVisible.Dispatch(ref _localMachineId);
				}
				moduleNode.manaDrainComponent.manaDrained.subscribers -= HandleOnManaDrained;
			}
		}

		protected override void Add(CloakModuleActivationNode moduleNode)
		{
			if (moduleNode.ownerComponent.ownedByMe)
			{
				moduleNode.activationComponent.activate.subscribers += HandleOnModuleActivated;
			}
			machineFadeEffect.Register(moduleNode.ownerComponent.machineId);
		}

		protected override void Remove(CloakModuleActivationNode moduleNode)
		{
			if (moduleNode.ownerComponent.ownedByMe)
			{
				moduleNode.activationComponent.activate.subscribers -= HandleOnModuleActivated;
			}
			machineFadeEffect.Unregister(moduleNode.ownerComponent.machineId);
		}
	}
}
