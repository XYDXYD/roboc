using Fabric;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Observer;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Simulation.Hardware.Modules
{
	internal sealed class PowerBarEngine : MultiEntityViewsEngine<PowerBarNode, ModulePowerConsumptionNode, MachineStunNode, ManaDrainNode>, IWaitForFrameworkDestruction, IQueryingEntityViewEngine, IStep<object>, IEngine, IStep
	{
		private readonly WeaponFiredObserver _weaponFiredObserver;

		private readonly IServiceRequestFactory _serviceRequestFactory;

		private readonly MachineSpawnDispatcher _machineSpawnDispatcher;

		private PowerBarSettingsData _powerBarSettingsData;

		private bool _soundPlayed = true;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public unsafe PowerBarEngine(WeaponFiredObserver weaponFiredObserver, IServiceRequestFactory serviceRequestFactory, MachineSpawnDispatcher machineSpawnDispatcher)
		{
			_weaponFiredObserver = weaponFiredObserver;
			_weaponFiredObserver.AddAction(new ObserverAction<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_serviceRequestFactory = serviceRequestFactory;
			_machineSpawnDispatcher = machineSpawnDispatcher;
			_machineSpawnDispatcher.OnPlayerRespawnedIn += HandleOnPlayerRespawned;
			TaskRunner.get_Instance().Run((Func<IEnumerator>)GetSettings);
		}

		public void Ready()
		{
		}

		private IEnumerator GetSettings()
		{
			TaskService<PowerBarSettingsData> taskService = _serviceRequestFactory.Create<IGetPowerBarSettingsRequest>().AsTask();
			HandleTaskServiceWithError taskServiceWithErrorHandling = new HandleTaskServiceWithError(taskService, delegate
			{
				RemoteLogger.Error("Unable to load power bar settings", "PowerBarEngine IGetPowerBarSettingsRequest", null);
				Console.LogWarning("Unable to load power bar settings");
			}, null);
			yield return taskServiceWithErrorHandling.GetEnumerator();
			PowerBarSettingsData powerBarSettingsData = taskService.result;
			Console.Log("---------------------------- OnPowerBarSettingsLoaded: RefillRatePerSecond: " + powerBarSettingsData.RefillRatePerSecond + " PowerForAllRobots: " + powerBarSettingsData.PowerForAllRobots);
			_powerBarSettingsData = powerBarSettingsData;
			yield return null;
			if (TryGetPowerBarData(out IPowerBarDataComponent powerBarData))
			{
				UpdateBar(powerBarData, (float)(double)_powerBarSettingsData.PowerForAllRobots);
			}
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_weaponFiredObserver.RemoveAction(new ObserverAction<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_machineSpawnDispatcher.OnPlayerRespawnedIn -= HandleOnPlayerRespawned;
		}

		public void Step(ref object data, Enum condition)
		{
			if (TryGetDeltaTimeComponent(out IDeltaTimeComponent deltaTimeComponent))
			{
				Tick(deltaTimeComponent.deltaTime);
			}
		}

		protected override void Add(PowerBarNode node)
		{
			if (_powerBarSettingsData != null)
			{
				UpdateBar(node.powerBarDataComponent, (float)(double)_powerBarSettingsData.PowerForAllRobots);
			}
		}

		protected override void Remove(PowerBarNode node)
		{
		}

		protected override void Add(ModulePowerConsumptionNode node)
		{
			node.confirmActivationComponent.activationConfirmed.subscribers += HandleActivateModule;
		}

		protected override void Remove(ModulePowerConsumptionNode node)
		{
			node.confirmActivationComponent.activationConfirmed.subscribers -= HandleActivateModule;
		}

		protected override void Add(MachineStunNode stunNode)
		{
			if (stunNode.ownerComponent.ownedByMe)
			{
				stunNode.stunComponent.machineStunned.subscribers += HandleMachineStunned;
			}
		}

		protected override void Remove(MachineStunNode stunNode)
		{
			if (stunNode.ownerComponent.ownedByMe)
			{
				stunNode.stunComponent.machineStunned.subscribers -= HandleMachineStunned;
			}
		}

		protected override void Add(ManaDrainNode drainNode)
		{
			if (drainNode.ownerComponent.ownedByMe)
			{
				drainNode.manaDrainComponent.manaDrained.subscribers += HandleManaDrained;
			}
		}

		protected override void Remove(ManaDrainNode drainNode)
		{
			if (drainNode.ownerComponent.ownedByMe)
			{
				drainNode.manaDrainComponent.manaDrained.subscribers -= HandleManaDrained;
			}
		}

		private void HandleOnPlayerRespawned(SpawnInParametersPlayer parameters)
		{
			if (parameters.isMe && TryGetPowerBarData(out IPowerBarDataComponent powerBarDataComponent))
			{
				powerBarDataComponent.powerValue = (float)(double)_powerBarSettingsData.PowerForAllRobots;
				UpdateBar(powerBarDataComponent, powerBarDataComponent.powerValue);
			}
		}

		private void HandleMachineStunned(IMachineStunComponent sender, int machineId)
		{
			if (TryGetPowerBarData(out IPowerBarDataComponent powerBarDataComponent))
			{
				if (sender.stunned)
				{
					powerBarDataComponent.powerValue = 0f;
					powerBarDataComponent.progressiveIncrementActive = false;
					UpdateBar(powerBarDataComponent, 0f);
				}
				else
				{
					powerBarDataComponent.progressiveIncrementActive = true;
				}
			}
		}

		private void Tick(float deltaTime)
		{
			if (TryGetPowerBarData(out IPowerBarDataComponent powerBarDataComponent))
			{
				float num = Math.Max(0f, powerBarDataComponent.powerValue);
				if (powerBarDataComponent.progressiveIncrementActive)
				{
					float num2 = (float)(double)_powerBarSettingsData.PowerForAllRobots * _powerBarSettingsData.RefillRatePerSecond * deltaTime;
					num = Math.Min(num + num2, (float)(double)_powerBarSettingsData.PowerForAllRobots);
				}
				UpdateBar(powerBarDataComponent, num);
			}
		}

		private void UpdateBar(IPowerBarDataComponent powerBarData, float powerValue)
		{
			powerBarData.powerValue = powerValue;
			powerBarData.powerPercent = powerBarData.powerValue / (float)(double)_powerBarSettingsData.PowerForAllRobots;
			if (powerBarData.powerPercent <= 0.6f)
			{
				_soundPlayed = false;
			}
			if (powerBarData.powerPercent >= 1f && !_soundPlayed)
			{
				EventManager.get_Instance().PostEvent("GUI_Mana_OK", 0);
				_soundPlayed = true;
			}
		}

		private void ConsumePower(ref float power)
		{
			if (TryGetPowerBarData(out IPowerBarDataComponent powerBarDataComponent))
			{
				powerBarDataComponent.powerValue -= power;
			}
		}

		private void HandleActivateModule(IModuleConfirmActivationComponent sender, int moduleId)
		{
			ModulePowerConsumptionNode modulePowerConsumptionNode = default(ModulePowerConsumptionNode);
			if (entityViewsDB.TryQueryEntityView<ModulePowerConsumptionNode>(moduleId, ref modulePowerConsumptionNode) && TryGetPowerBarData(out IPowerBarDataComponent powerBarDataComponent))
			{
				float num = powerBarDataComponent.powerValue - modulePowerConsumptionNode.manaCostComponent.weaponFireCost;
				UpdateBar(powerBarDataComponent, Mathf.Clamp(num, 0f, (float)(double)_powerBarSettingsData.PowerForAllRobots));
			}
		}

		private void HandleManaDrained(IManaDrainComponent sender, int machineId)
		{
			MachineStunNode machineStunNode = default(MachineStunNode);
			if (entityViewsDB.TryQueryEntityView<MachineStunNode>(machineId, ref machineStunNode) && !machineStunNode.stunComponent.stunned && TryGetPowerBarData(out IPowerBarDataComponent powerBarDataComponent))
			{
				powerBarDataComponent.progressiveIncrementActive = true;
			}
		}

		private bool TryGetPowerBarData(out IPowerBarDataComponent powerBarDataComponent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<PowerBarNode> val = entityViewsDB.QueryEntityViews<PowerBarNode>();
			if (val.get_Count() > 0)
			{
				powerBarDataComponent = val.get_Item(0).powerBarDataComponent;
				return true;
			}
			powerBarDataComponent = null;
			return false;
		}

		private bool TryGetDeltaTimeComponent(out IDeltaTimeComponent deltaTimeComponent)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			FasterReadOnlyList<PowerBarConsumptionEntityView> val = entityViewsDB.QueryEntityViews<PowerBarConsumptionEntityView>();
			if (val.get_Count() > 0)
			{
				deltaTimeComponent = val.get_Item(0).deltaTimeComponent;
				return true;
			}
			deltaTimeComponent = null;
			return false;
		}
	}
}
