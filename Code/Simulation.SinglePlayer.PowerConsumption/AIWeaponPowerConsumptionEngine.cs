using Battle;
using Simulation.Hardware.Weapons;
using Simulation.Hardware.Weapons.Laser;
using Simulation.Hardware.Weapons.Plasma;
using Svelto.Context;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.SinglePlayer.PowerConsumption
{
	internal class AIWeaponPowerConsumptionEngine : MultiEntityViewsEngine<AIAgentDataComponentsNode, LaserWeaponShootingNode, PlasmaWeaponShootingNode>, IInitialize, IQueryingEntityViewEngine, IPhysicallyTickable, IWaitForFrameworkDestruction, IEngine, ITickableBase
	{
		private Dictionary<int, AIAgentDataComponentsNode> _agentslookupTable = new Dictionary<int, AIAgentDataComponentsNode>();

		private Dictionary<int, float> _powerConsumptionMap = new Dictionary<int, float>();

		private PowerBarSettingsData _powerBarSettingsData;

		private IEntityViewsDB _nodeDb;

		[Inject]
		public IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		[Inject]
		public DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		public BattlePlayers battlePlayers
		{
			private get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get
			{
				return _nodeDb;
			}
			set
			{
				_nodeDb = value;
			}
		}

		public void OnDependenciesInjected()
		{
			destructionReporter.OnMachineDestroyed += HandleMachineDestroyed;
			IGetPowerBarSettingsRequest getPowerBarSettingsRequest = serviceFactory.Create<IGetPowerBarSettingsRequest>();
			getPowerBarSettingsRequest.SetAnswer(new ServiceAnswer<PowerBarSettingsData>(OnPowerBarSettingsLoaded));
			getPowerBarSettingsRequest.Execute();
		}

		protected override void Add(LaserWeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByAi)
			{
				obj.shootingComponent.shotIsGoingToBeFired.subscribers += HandleWeaponShot;
				SetWeaponPowerConsumption(obj);
			}
		}

		protected override void Remove(LaserWeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByAi)
			{
				obj.shootingComponent.shotIsGoingToBeFired.subscribers -= HandleWeaponShot;
			}
		}

		protected override void Add(PlasmaWeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByAi)
			{
				obj.shootingComponent.shotIsGoingToBeFired.subscribers += HandleWeaponShot;
				SetWeaponPowerConsumption(obj);
			}
		}

		protected override void Remove(PlasmaWeaponShootingNode obj)
		{
			if (obj.weaponOwner.ownedByAi)
			{
				obj.shootingComponent.shotIsGoingToBeFired.subscribers -= HandleWeaponShot;
			}
		}

		protected override void Add(AIAgentDataComponentsNode obj)
		{
			obj.aiPowerConsumptionComponent.maxPower = (float)(double)_powerBarSettingsData.PowerForAllRobots;
			obj.aiPowerConsumptionComponent.power = obj.aiPowerConsumptionComponent.maxPower;
			if (_powerConsumptionMap.ContainsKey(obj.aiBotIdData.playerId))
			{
				obj.aiPowerConsumptionComponent.currentWeaponPowerConsumption = _powerConsumptionMap[obj.aiBotIdData.playerId];
			}
			_agentslookupTable[obj.aiBotIdData.playerId] = obj;
		}

		protected override void Remove(AIAgentDataComponentsNode obj)
		{
			_agentslookupTable.Remove(obj.aiBotIdData.playerId);
		}

		public void PhysicsTick(float deltaSec)
		{
			foreach (KeyValuePair<int, AIAgentDataComponentsNode> item in _agentslookupTable)
			{
				IAIPowerConsumptionComponent aiPowerConsumptionComponent = item.Value.aiPowerConsumptionComponent;
				float num = aiPowerConsumptionComponent.power + _powerBarSettingsData.RefillRatePerSecond * aiPowerConsumptionComponent.maxPower * deltaSec;
				num = (aiPowerConsumptionComponent.power = Mathf.Clamp(num, 0f, aiPowerConsumptionComponent.maxPower));
			}
		}

		public void OnFrameworkDestroyed()
		{
			destructionReporter.OnMachineDestroyed -= HandleMachineDestroyed;
		}

		private void SetWeaponPowerConsumption(WeaponShootingNode node)
		{
			_powerConsumptionMap[node.weaponOwner.ownerId] = node.weaponFireCostComponent.weaponFireCost;
			if (_agentslookupTable.TryGetValue(node.weaponOwner.ownerId, out AIAgentDataComponentsNode value))
			{
				value.aiPowerConsumptionComponent.currentWeaponPowerConsumption = node.weaponFireCostComponent.weaponFireCost;
			}
		}

		private void HandleMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			if (_agentslookupTable.TryGetValue(playerId, out AIAgentDataComponentsNode value))
			{
				value.aiPowerConsumptionComponent.power = value.aiPowerConsumptionComponent.maxPower;
			}
		}

		private void HandleWeaponShot(IShootingComponent shootingComponent, int nodeId)
		{
			LaserWeaponShootingNode laserWeaponShootingNode = default(LaserWeaponShootingNode);
			PlasmaWeaponShootingNode plasmaWeaponShootingNode = default(PlasmaWeaponShootingNode);
			if (entityViewsDB.TryQueryEntityView<LaserWeaponShootingNode>(nodeId, ref laserWeaponShootingNode))
			{
				int ownerId = laserWeaponShootingNode.weaponOwner.ownerId;
				UpdatePlayerAvailablePower(laserWeaponShootingNode, ownerId);
			}
			else if (entityViewsDB.TryQueryEntityView<PlasmaWeaponShootingNode>(nodeId, ref plasmaWeaponShootingNode))
			{
				int ownerId2 = plasmaWeaponShootingNode.weaponOwner.ownerId;
				UpdatePlayerAvailablePower(plasmaWeaponShootingNode, ownerId2);
			}
		}

		private void UpdatePlayerAvailablePower(WeaponShootingNode node, int playerId)
		{
			IAIPowerConsumptionComponent aiPowerConsumptionComponent = _agentslookupTable[playerId].aiPowerConsumptionComponent;
			aiPowerConsumptionComponent.power -= node.weaponFireCostComponent.weaponFireCost;
		}

		private void OnPowerBarSettingsLoaded(PowerBarSettingsData powerBarSettingsData)
		{
			_powerBarSettingsData = powerBarSettingsData;
		}

		public void Ready()
		{
		}
	}
}
