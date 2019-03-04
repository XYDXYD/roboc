using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class ChaingunSpinEngine : MultiEntityViewsEngine<ChaingunSpinNode, PowerBarNode>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IStep<object>, IEngine, IStep
	{
		private Dictionary<int, FasterList<ChaingunSpinNode>> _chaingunsPerGroup;

		private NetworkWeaponFiredObserver _networkWeaponFiredObserver;

		private PowerBarNode _powerBarNode;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		[Inject]
		internal CrosshairController crosshairController
		{
			private get;
			set;
		}

		public unsafe ChaingunSpinEngine(NetworkWeaponFiredObserver networkWeaponFiredObserver)
		{
			_networkWeaponFiredObserver = networkWeaponFiredObserver;
			_networkWeaponFiredObserver.AddAction(new ObserverAction<FiringInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_chaingunsPerGroup = new Dictionary<int, FasterList<ChaingunSpinNode>>();
		}

		public void Ready()
		{
		}

		public unsafe void OnFrameworkDestroyed()
		{
			_networkWeaponFiredObserver.RemoveAction(new ObserverAction<FiringInfo>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void Step(ref object data, Enum condition)
		{
			if (TryGetDeltaTimeComponent(out IDeltaTimeComponent deltaTimeComponent))
			{
				Tick(deltaTimeComponent.deltaTime);
			}
		}

		protected override void Add(ChaingunSpinNode node)
		{
			int key = WeaponGroupUtility.MakeID(node.weaponOwner.machineId, node.itemDescriptorComponent.itemDescriptor);
			if (!_chaingunsPerGroup.TryGetValue(key, out FasterList<ChaingunSpinNode> value))
			{
				FasterList<ChaingunSpinNode> val = new FasterList<ChaingunSpinNode>();
				_chaingunsPerGroup[key] = val;
				value = val;
			}
			value.Add(node);
			node.shootingComponent.shotIsGoingToBeFired.subscribers += HandleWeaponFired;
			node.healthStatusComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)HandleWeaponEnabled);
			if (node.healthStatusComponent.enabled)
			{
				HandleWeaponEnabled(node.get_ID(), value: true);
			}
		}

		protected override void Remove(ChaingunSpinNode node)
		{
			if (node.healthStatusComponent.enabled)
			{
				HandleWeaponEnabled(node.get_ID(), value: false);
			}
			int key = WeaponGroupUtility.MakeID(node.weaponOwner.machineId, node.itemDescriptorComponent.itemDescriptor);
			if (_chaingunsPerGroup.TryGetValue(key, out FasterList<ChaingunSpinNode> value))
			{
				value.Remove(node);
				if (value.get_Count() == 0)
				{
					_chaingunsPerGroup.Remove(key);
				}
			}
			node.shootingComponent.shotIsGoingToBeFired.subscribers -= HandleWeaponFired;
			node.healthStatusComponent.isPartEnabled.StopNotify((Action<int, bool>)HandleWeaponEnabled);
		}

		protected override void Add(PowerBarNode obj)
		{
			_powerBarNode = obj;
		}

		protected override void Remove(PowerBarNode obj)
		{
			_powerBarNode = null;
		}

		private void Tick(float deltaTime)
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			if (_chaingunsPerGroup.Count == 0)
			{
				return;
			}
			FasterReadOnlyList<SharedSpinDataNode> val = entityViewsDB.QueryMetaEntityViews<SharedSpinDataNode>();
			float num = 0f;
			for (int i = 0; i < val.get_Count(); i++)
			{
				SharedSpinDataNode sharedSpinDataNode = val.get_Item(i);
				SharedSpinData sharedSpinData = sharedSpinDataNode.sharedChaingunSpinComponent.sharedSpinData;
				if (sharedSpinData.enabledWeaponCount != 0)
				{
					if (sharedSpinDataNode.hardwareOwner.ownedByMe)
					{
						UpdateManaDrain(sharedSpinData, deltaTime, sharedSpinDataNode.get_ID());
					}
					UpdateSpin(sharedSpinData, deltaTime, sharedSpinDataNode);
				}
				if (sharedSpinDataNode.hardwareOwner.ownedByMe)
				{
					num = Mathf.Max(sharedSpinData.spinPower, num);
				}
			}
			crosshairController.weaponSpinPower = num;
		}

		private void HandleOnRemoteWeaponFired(ref FiringInfo firingInfo)
		{
			ChaingunSpinNode chaingunSpinNode = default(ChaingunSpinNode);
			if (entityViewsDB.TryQueryEntityView<ChaingunSpinNode>(firingInfo.weaponId, ref chaingunSpinNode))
			{
				StartSpinUp(firingInfo.weaponId, chaingunSpinNode.weaponOwner.machineId, chaingunSpinNode.itemDescriptorComponent.itemDescriptor);
			}
		}

		private void HandleWeaponFired(IShootingComponent sender, int weaponId)
		{
			ChaingunSpinNode chaingunSpinNode = default(ChaingunSpinNode);
			if (entityViewsDB.TryQueryEntityView<ChaingunSpinNode>(weaponId, ref chaingunSpinNode))
			{
				StartSpinUp(weaponId, chaingunSpinNode.weaponOwner.machineId, chaingunSpinNode.itemDescriptorComponent.itemDescriptor);
			}
		}

		private void HandleWeaponEnabled(int weaponId, bool value)
		{
			SharedSpinDataNode sharedSpinDataNode2 = default(SharedSpinDataNode);
			if (value)
			{
				SharedSpinDataNode sharedSpinDataNode = default(SharedSpinDataNode);
				if (entityViewsDB.TryQueryEntityView<SharedSpinDataNode>(weaponId, ref sharedSpinDataNode))
				{
					SharedSpinData sharedSpinData = sharedSpinDataNode.sharedChaingunSpinComponent.sharedSpinData;
					sharedSpinData.enabledWeaponCount++;
					ChaingunSpinNode chaingunSpinNode = default(ChaingunSpinNode);
					if (sharedSpinData.spinPower > 0f && entityViewsDB.TryQueryEntityView<ChaingunSpinNode>(weaponId, ref chaingunSpinNode))
					{
						chaingunSpinNode.spinEventComponent.spinStarted.Dispatch(ref weaponId);
					}
				}
			}
			else if (entityViewsDB.TryQueryEntityView<SharedSpinDataNode>(weaponId, ref sharedSpinDataNode2))
			{
				SharedSpinData sharedSpinData2 = sharedSpinDataNode2.sharedChaingunSpinComponent.sharedSpinData;
				ChaingunSpinNode chaingunSpinNode2 = default(ChaingunSpinNode);
				if (sharedSpinData2.spinPower >= 0f && entityViewsDB.TryQueryEntityView<ChaingunSpinNode>(weaponId, ref chaingunSpinNode2))
				{
					chaingunSpinNode2.spinEventComponent.spinStopped.Dispatch(ref weaponId);
				}
				sharedSpinData2.enabledWeaponCount = Math.Max(sharedSpinData2.enabledWeaponCount - 1, 0);
				if (sharedSpinData2.enabledWeaponCount == 0)
				{
					sharedSpinData2.spinPower = 0f;
					sharedSpinData2.spinningUp = false;
				}
			}
		}

		private void StartSpinUp(int weaponId, int machineId, ItemDescriptor itemDescriptor)
		{
			int num = WeaponGroupUtility.MakeID(machineId, itemDescriptor);
			SharedSpinDataNode sharedSpinDataNode = entityViewsDB.QueryMetaEntityView<SharedSpinDataNode>(num);
			SharedSpinData sharedSpinData = sharedSpinDataNode.sharedChaingunSpinComponent.sharedSpinData;
			sharedSpinData.fireCooldown = 0f;
			sharedSpinData.spinningUp = true;
			sharedSpinData.maxFireCooldown = GetWeaponCooldownByWeaponId(weaponId);
			if (!(sharedSpinData.spinPower <= 0f))
			{
				return;
			}
			FasterList<ChaingunSpinNode> val = _chaingunsPerGroup[num];
			for (int i = 0; i < val.get_Count(); i++)
			{
				ChaingunSpinNode chaingunSpinNode = val.get_Item(i);
				if (!chaingunSpinNode.healthStatusComponent.disabled)
				{
					int value = chaingunSpinNode.get_ID();
					chaingunSpinNode.spinEventComponent.spinStarted.Dispatch(ref value);
				}
			}
		}

		private float GetWeaponCooldownByWeaponId(int weaponId)
		{
			return GetWeaponCooldown(entityViewsDB.QueryEntityView<ChaingunSpinNode>(weaponId));
		}

		private float GetWeaponCooldownByGroupId(int groupId)
		{
			return GetWeaponCooldown(_chaingunsPerGroup[groupId].get_Item(0));
		}

		private float GetWeaponCooldown(ChaingunSpinNode gun)
		{
			if (gun.weaponOwner.ownedByMe)
			{
				return gun.cooldownComponent.weaponCooldown;
			}
			return 1f;
		}

		private void UpdateSpin(SharedSpinData sharedData, float deltaSec, SharedSpinDataNode sharedNode)
		{
			float weaponCooldownByGroupId = GetWeaponCooldownByGroupId(sharedNode.get_ID());
			if (weaponCooldownByGroupId >= sharedData.maxFireCooldown)
			{
				sharedData.maxFireCooldown = weaponCooldownByGroupId;
			}
			float spinUpTime = sharedNode.weaponSpinComponent.spinUpTime;
			float spinDownTime = CalculateSpinDownTime(sharedNode);
			if (sharedData.spinningUp && sharedData.fireCooldown <= sharedData.maxFireCooldown)
			{
				SpinUp(sharedData, deltaSec, spinUpTime);
			}
			else
			{
				SpinDown(sharedData, deltaSec, spinDownTime, sharedNode.get_ID());
			}
		}

		private float CalculateSpinDownTime(SharedSpinDataNode sharedNode)
		{
			float num = (!(_powerBarNode.powerBarDataComponent.powerValue < float.Epsilon)) ? 1f : 0.5f;
			return sharedNode.weaponSpinComponent.spinDownTime * num;
		}

		private static void SpinUp(SharedSpinData sharedData, float deltaSec, float spinUpTime)
		{
			sharedData.fireCooldown += deltaSec;
			if (sharedData.spinPower < 1f)
			{
				if (spinUpTime > 0f)
				{
					sharedData.spinPower += deltaSec / spinUpTime;
				}
				else
				{
					sharedData.spinPower = 1f;
				}
			}
		}

		private void SpinDown(SharedSpinData sharedData, float deltaSec, float spinDownTime, int groupId)
		{
			sharedData.spinningUp = false;
			if (sharedData.spinPower > 0f)
			{
				if (spinDownTime > 0f)
				{
					sharedData.spinPower -= deltaSec / spinDownTime;
				}
				else
				{
					sharedData.spinPower = 0f;
				}
				if (sharedData.spinPower <= 0f)
				{
					StopSpinning(sharedData, groupId);
				}
			}
		}

		private void StopSpinning(SharedSpinData sharedData, int groupId)
		{
			FasterList<ChaingunSpinNode> val = _chaingunsPerGroup[groupId];
			for (int i = 0; i < val.get_Count(); i++)
			{
				ChaingunSpinNode chaingunSpinNode = val.get_Item(i);
				if (!chaingunSpinNode.healthStatusComponent.disabled)
				{
					int value = chaingunSpinNode.get_ID();
					chaingunSpinNode.spinEventComponent.spinStopped.Dispatch(ref value);
				}
			}
			sharedData.spinPower = 0f;
		}

		private void UpdateManaDrain(SharedSpinData sharedData, float deltaSec, int groupId)
		{
			if (sharedData.spinningUp)
			{
				ChaingunSpinNode chaingunSpinNode = _chaingunsPerGroup[groupId].get_Item(0);
				float weaponFireCost = chaingunSpinNode.fireCostComponent.weaponFireCost;
				_powerBarNode.powerBarDataComponent.powerValue -= weaponFireCost * deltaSec;
			}
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
