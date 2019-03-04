using Fabric;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Observer;
using Svelto.Ticker.Legacy;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class WeaponFireTimingEngine : MultiEntityViewsEngine<WeaponFireTimingNode, PowerBarNode, SmartWeaponFireNode>, IWaitForFrameworkDestruction, ILateTickable, IQueryingEntityViewEngine, ITickableBase, IEngine
	{
		private const float SOUND_INTERVAL = 0.2f;

		private const float NUM_FRAMES_BETWEEN_AUDIO_EVENTS = 3f;

		private readonly WeaponReadyObservable _weaponReadyObservable;

		private readonly WeaponNoFireObservable _noFireObservable;

		private IPowerBarDataComponent _powerBarComponent;

		private readonly Dictionary<int, WeaponFireTimingNode> _allWeapons = new Dictionary<int, WeaponFireTimingNode>(200);

		private readonly Dictionary<int, WeaponFireTimingData> _fireTimingData = new Dictionary<int, WeaponFireTimingData>(24);

		private bool _canSignalNoMana = true;

		private float _enableSoundTime;

		private float _enableSoundMana;

		private readonly FirePressedObserver _firePressedObserver;

		private readonly FireHeldDownObserver _fireHeldDownObserver;

		private readonly TeslaFireObserver _teslaFireObserver;

		private readonly WeaponCooldownEndedObservable _weaponCooldownEndedObserver;

		[Inject]
		internal WeaponOrderPresenter weaponOrderPresenter
		{
			get;
			set;
		}

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public unsafe WeaponFireTimingEngine(WeaponReadyObservable weaponReadyObservable, WeaponNoFireObservable noFireObservable, FirePressedObserver firePressedObserver, FireHeldDownObserver fireHeldDownObserver, TeslaFireObserver teslaFireObserver, WeaponCooldownEndedObservable weaponCooldownEndedObserver)
		{
			_weaponReadyObservable = weaponReadyObservable;
			_noFireObservable = noFireObservable;
			_firePressedObserver = firePressedObserver;
			_fireHeldDownObserver = fireHeldDownObserver;
			_teslaFireObserver = teslaFireObserver;
			_weaponCooldownEndedObserver = weaponCooldownEndedObserver;
			_firePressedObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_fireHeldDownObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_teslaFireObserver.AddAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void LateTick(float deltaSec)
		{
			using (Dictionary<int, WeaponFireTimingData>.Enumerator enumerator = _fireTimingData.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					WeaponFireTimingData value = enumerator.Current.Value;
					WeaponFireTimingData weaponFireTimingData = value;
					TickRefireTimes(value, deltaSec, enumerator.Current.Key);
					if (value.isLocalHumanPlayer)
					{
						UpdateViewCooldowns(value);
					}
					if (TryGetCurrentActiveWeapon(enumerator.Current.Key, out ItemDescriptor activeWeapon) && weaponFireTimingData.misfireData.ContainsKey(activeWeapon))
					{
						int key = enumerator.Current.Key;
						MisfireTimingData misfireTimingData = weaponFireTimingData.misfireData[activeWeapon];
						if (misfireTimingData.misfireDebuffTimeLeft > 0f)
						{
							misfireTimingData.misfireDebuffTimeLeft -= deltaSec;
							if (misfireTimingData.misfireDebuffTimeLeft <= 0f)
							{
								misfireTimingData.misfireDebuffTimeLeft = 0f;
								misfireTimingData.misfireDebuffPower = 0f;
							}
						}
						UpdateMachineFireTiming(weaponFireTimingData, key);
					}
				}
			}
		}

		private void TickRefireTimes(WeaponFireTimingData timingData, float deltaSec, int machineId)
		{
			if (!TryGetCurrentActiveWeapon(machineId, out ItemDescriptor activeWeapon) || !TickSlotRefireTime(timingData, activeWeapon, deltaSec))
			{
				foreach (ItemDescriptor key in timingData.enabledWeaponCount.Keys)
				{
					if (TickSlotRefireTime(timingData, key, deltaSec))
					{
						break;
					}
				}
			}
		}

		private bool TickSlotRefireTime(WeaponFireTimingData timingData, ItemDescriptor item, float deltaSec)
		{
			if (timingData.refireTimeRemaining.TryGetValue(item, out float value) && value > 0f)
			{
				bool flag = false;
				float num = value;
				value -= deltaSec;
				if (num > 0f && value <= 0f)
				{
					flag = true;
				}
				timingData.refireTimeRemaining[item] = value;
				if (flag)
				{
					_weaponCooldownEndedObserver.Dispatch(ref item);
				}
				return true;
			}
			return false;
		}

		private void UpdateViewCooldowns(WeaponFireTimingData timingData)
		{
			using (Dictionary<ItemDescriptor, float>.Enumerator enumerator = timingData.refireTimeRemaining.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ItemDescriptor key = enumerator.Current.Key;
					float value = enumerator.Current.Value;
					weaponOrderPresenter.SetWeaponCooldownTime(key, value);
				}
			}
		}

		protected override void Add(WeaponFireTimingNode node)
		{
			if (node.weaponOwner.ownedByMe || node.weaponOwner.ownedByAi)
			{
				_allWeapons.Add(node.get_ID(), node);
				WeaponFireTimingData orCreateTimingData = GetOrCreateTimingData(node.weaponOwner);
				ItemDescriptor itemDescriptor = node.itemDescriptorComponent.itemDescriptor;
				if (orCreateTimingData.weaponsBySubCategory.ContainsKey(itemDescriptor))
				{
					orCreateTimingData.weaponsBySubCategory[itemDescriptor].Add(node);
				}
				else
				{
					orCreateTimingData.AddSubCategory(itemDescriptor);
					orCreateTimingData.weaponsBySubCategory[itemDescriptor].Add(node);
				}
				node.healthStatusComponent.isPartEnabled.NotifyOnValueSet((Action<int, bool>)OnWeaponEnabled);
				if (node.healthStatusComponent.enabled)
				{
					int num = orCreateTimingData.enabledWeaponCount[itemDescriptor];
					orCreateTimingData.enabledWeaponCount[itemDescriptor] = num + 1;
					UpdateMachineFireTiming(orCreateTimingData, node.weaponOwner.machineId);
				}
				node.fireTiming.timingsLoaded.subscribers += HandleOnFireTimingsLoaded;
			}
		}

		protected override void Remove(WeaponFireTimingNode node)
		{
			if (node.weaponOwner.ownedByMe || node.weaponOwner.ownedByAi)
			{
				node.healthStatusComponent.isPartEnabled.StopNotify((Action<int, bool>)OnWeaponEnabled);
				node.fireTiming.timingsLoaded.subscribers -= HandleOnFireTimingsLoaded;
				int machineId = node.weaponOwner.machineId;
				_allWeapons.Remove(node.get_ID());
				_fireTimingData.Remove(machineId);
			}
		}

		protected override void Add(PowerBarNode obj)
		{
			_powerBarComponent = obj.powerBarDataComponent;
		}

		protected override void Remove(PowerBarNode obj)
		{
			_powerBarComponent = null;
		}

		protected override void Add(SmartWeaponFireNode node)
		{
			if (node.ownerComponent.ownedByMe || node.ownerComponent.ownedByAi)
			{
				node.misfireComponent.weaponMisfired.subscribers += HandleWeaponMisfired;
			}
		}

		protected override void Remove(SmartWeaponFireNode node)
		{
			if (node.ownerComponent.ownedByMe || node.ownerComponent.ownedByAi)
			{
				node.misfireComponent.weaponMisfired.subscribers -= HandleWeaponMisfired;
			}
		}

		public void Ready()
		{
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_firePressedObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_fireHeldDownObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_teslaFireObserver.RemoveAction(new ObserverAction<int>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private WeaponFireTimingData GetOrCreateTimingData(IHardwareOwnerComponent owner)
		{
			WeaponFireTimingData value = null;
			if (!_fireTimingData.TryGetValue(owner.machineId, out value))
			{
				WeaponFireTimingData weaponFireTimingData = new WeaponFireTimingData(owner.ownedByMe);
				_fireTimingData[owner.machineId] = weaponFireTimingData;
				value = weaponFireTimingData;
			}
			return value;
		}

		private void OnWeaponEnabled(int weaponId, bool enabled)
		{
			WeaponFireTimingNode weaponFireTimingNode = _allWeapons[weaponId];
			ItemDescriptor itemDescriptor = weaponFireTimingNode.itemDescriptorComponent.itemDescriptor;
			int machineId = weaponFireTimingNode.weaponOwner.machineId;
			WeaponFireTimingData weaponFireTimingData = _fireTimingData[machineId];
			int num = weaponFireTimingData.enabledWeaponCount[itemDescriptor];
			weaponFireTimingData.enabledWeaponCount[itemDescriptor] = Mathf.Max(num + (enabled ? 1 : (-1)), 0);
			UpdateMachineFireTiming(weaponFireTimingData, machineId);
		}

		private void HandleOnFirePressed(ref int machineId)
		{
			ItemDescriptor activeWeapon;
			if (_fireTimingData.TryGetValue(machineId, out WeaponFireTimingData value) && TryGetCurrentActiveWeapon(machineId, out activeWeapon) && activeWeapon.itemCategory != ItemCategory.Tesla)
			{
				CheckCanFire(machineId, value, isButtonHeldDown: false);
			}
		}

		private void HandleOnFireHeldDown(ref int machineId)
		{
			ItemDescriptor activeWeapon;
			if (_fireTimingData.TryGetValue(machineId, out WeaponFireTimingData value) && TryGetCurrentActiveWeapon(machineId, out activeWeapon) && activeWeapon.itemCategory != ItemCategory.Tesla)
			{
				CheckCanFire(machineId, value, isButtonHeldDown: true);
			}
		}

		private void HandleOnTeslaFirePressed(ref int machineId)
		{
			if (_fireTimingData.TryGetValue(machineId, out WeaponFireTimingData value))
			{
				CheckCanFire(machineId, value, isButtonHeldDown: false);
			}
		}

		private void HandleWeaponMisfired(IMisfireComponent sender, int weaponId)
		{
			if (!_allWeapons.TryGetValue(weaponId, out WeaponFireTimingNode value) || !_fireTimingData.TryGetValue(value.weaponOwner.machineId, out WeaponFireTimingData value2))
			{
				return;
			}
			ItemDescriptor itemDescriptor = value.itemDescriptorComponent.itemDescriptor;
			MisfireTimingData value3;
			if (sender.misfireDebuffDuration > 0f && value2.misfireData.TryGetValue(itemDescriptor, out value3) && value3.currentMisfireCount == 0)
			{
				float misfireDebuffPower = value3.misfireDebuffPower;
				float num = value3.misfireDebuffTimeLeft / sender.misfireDebuffDuration;
				misfireDebuffPower = sender.coolDownPenalty + num * misfireDebuffPower;
				value3.misfireDebuffTimeLeft = sender.misfireDebuffDuration;
				float num2 = (float)sender.misfireDebuffMaxStacks * sender.coolDownPenalty;
				if (misfireDebuffPower > num2)
				{
					misfireDebuffPower = num2;
				}
				value3.misfireDebuffPower = misfireDebuffPower;
				value3.currentMisfireCount++;
				UpdateMachineFireTiming(value2, value.weaponOwner.machineId);
			}
		}

		private void HandleOnFireTimingsLoaded(IFireTimingComponent sender, ItemDescriptor itemDescriptor)
		{
			Dictionary<int, WeaponFireTimingData>.Enumerator enumerator = _fireTimingData.GetEnumerator();
			while (enumerator.MoveNext())
			{
				WeaponFireTimingData value = enumerator.Current.Value;
				UpdateMachineFireTiming(value, enumerator.Current.Key);
			}
		}

		private void HandleOnWeaponSwitched(int machineId, ItemDescriptor currentActiveSubCategory)
		{
			WeaponFireTimingData value = null;
			if (_fireTimingData.TryGetValue(machineId, out value))
			{
				UpdateMachineFireTiming(value, machineId);
			}
		}

		private void CheckCanFire(int machineId, WeaponFireTimingData timingData, bool isButtonHeldDown)
		{
			if (!_fireTimingData.TryGetValue(machineId, out timingData))
			{
				return;
			}
			bool flag = false;
			bool flag2 = false;
			MachineWeaponsBlockedNode machineWeaponsBlockedNode = default(MachineWeaponsBlockedNode);
			if (timingData.isLocalHumanPlayer && entityViewsDB.TryQueryEntityView<MachineWeaponsBlockedNode>(machineId, ref machineWeaponsBlockedNode))
			{
				flag = machineWeaponsBlockedNode.machineWeaponsBlockedComponent.blocked;
				flag2 = machineWeaponsBlockedNode.machineWeaponsBlockedComponent.lastWeaponShotBlocked;
			}
			if (!TryGetCurrentActiveWeapon(machineId, out ItemDescriptor activeWeapon) || timingData.enabledWeaponCount[activeWeapon] <= 0)
			{
				return;
			}
			FasterList<WeaponFireTimingNode> val = timingData.weaponsBySubCategory[activeWeapon];
			WeaponFireTimingNode weaponFireTimingNode = val.get_Item(0);
			if (CheckFireTiming(weaponFireTimingNode, timingData, machineId) && CheckManaCost(weaponFireTimingNode, timingData))
			{
				if (!flag)
				{
					_weaponReadyObservable.Dispatch(ref machineId);
				}
				MisfireTimingData misfireTimingData = timingData.misfireData[activeWeapon];
				Dictionary<ItemDescriptor, float> currentCooldownBetweenShots = timingData.currentCooldownBetweenShots;
				float num = currentCooldownBetweenShots[activeWeapon];
				Dictionary<ItemDescriptor, float> refireTimeRemaining;
				ItemDescriptor key;
				(refireTimeRemaining = timingData.refireTimeRemaining)[key = activeWeapon] = refireTimeRemaining[key] + num;
				if (timingData.isLocalHumanPlayer && !flag && !flag2)
				{
					weaponOrderPresenter.StartCooldown(activeWeapon, num);
				}
				misfireTimingData.currentMisfireCount = 0;
			}
			else if (!isButtonHeldDown && Time.get_timeSinceLevelLoad() >= _enableSoundTime)
			{
				int iD = weaponFireTimingNode.get_ID();
				_noFireObservable.Dispatch(ref iD);
			}
			if (timingData.isLocalHumanPlayer)
			{
				UpdateFireSoundTime(timingData);
			}
		}

		private bool CheckManaCost(WeaponFireTimingNode fireTimingNode, WeaponFireTimingData timingData)
		{
			float num = fireTimingNode.weaponFireCostComponent.weaponFireCost;
			if (timingData.isLocalHumanPlayer)
			{
				if (!TryGetCurrentActiveWeapon(fireTimingNode.weaponOwner.machineId, out ItemDescriptor activeWeapon))
				{
					return false;
				}
				if (activeWeapon.itemCategory == ItemCategory.Chaingun)
				{
					num = 0f;
				}
				if (_powerBarComponent.powerValue - num < 0f)
				{
					if (timingData.isLocalHumanPlayer)
					{
						weaponOrderPresenter.PlayNotEnoughPowerAnimation(activeWeapon);
					}
					_powerBarComponent.PlayNotEnoughPowerAnimation();
					if (Time.get_timeSinceLevelLoad() >= _enableSoundMana && _powerBarComponent.powerValue - num < (0f - num / 100f) * 10f)
					{
						if (_canSignalNoMana)
						{
							EventManager.get_Instance().PostEvent("GUI_NoMana", 0);
							_canSignalNoMana = false;
						}
						_enableSoundMana = Time.get_timeSinceLevelLoad() + 0.2f;
					}
					return false;
				}
				_canSignalNoMana = true;
			}
			return true;
		}

		private bool CheckFireTiming(WeaponFireTimingNode fireTimingNode, WeaponFireTimingData timingData, int machineId)
		{
			if (TryGetCurrentActiveWeapon(machineId, out ItemDescriptor activeWeapon))
			{
				return timingData.refireTimeRemaining[activeWeapon] <= 0f;
			}
			return false;
		}

		private void UpdateFireSoundTime(WeaponFireTimingData timingData)
		{
			_enableSoundTime = Time.get_timeSinceLevelLoad() + 0.2f;
		}

		private void UpdateMachineFireTiming(WeaponFireTimingData timingData, int machineID)
		{
			float multiplier = 1f;
			ItemDescriptor activeWeapon;
			SharedSpinDataNode sharedSpinDataNode = default(SharedSpinDataNode);
			if (TryGetCurrentActiveWeapon(machineID, out activeWeapon) && activeWeapon.itemCategory == ItemCategory.Chaingun && entityViewsDB.TryQueryMetaEntityView<SharedSpinDataNode>(WeaponGroupUtility.MakeID(machineID, activeWeapon), ref sharedSpinDataNode))
			{
				SharedSpinData sharedSpinData = sharedSpinDataNode.sharedChaingunSpinComponent.sharedSpinData;
				if (sharedSpinData.enabledWeaponCount == 0)
				{
					return;
				}
				IWeaponSpinStatsComponent weaponSpinComponent = sharedSpinDataNode.weaponSpinComponent;
				multiplier = Mathf.Lerp(weaponSpinComponent.spinInitialCooldown, 1f, sharedSpinData.spinPower);
			}
			if (!(activeWeapon != null))
			{
				return;
			}
			FasterList<WeaponFireTimingNode> val = timingData.weaponsBySubCategory[activeWeapon];
			if (val.get_Count() > 0)
			{
				WeaponFireTimingNode weaponFireTimingNode = val.get_Item(0);
				float num = ComputeFireTimingValue(timingData, weaponFireTimingNode.fireTiming, multiplier, activeWeapon);
				timingData.currentCooldownBetweenShots[activeWeapon] = num;
				for (int i = 0; i < val.get_Count(); i++)
				{
					val.get_Item(i).weaponCooldownComponent.weaponCooldown = num;
				}
			}
		}

		private float ComputeFireTimingValue(WeaponFireTimingData timingData, IFireTimingComponent fireTiming, float multiplier, ItemDescriptor currentActiveWeapon)
		{
			float num = fireTiming.delayBetweenShots;
			if (fireTiming.groupFirePeriod != null && timingData.enabledWeaponCount.ContainsKey(currentActiveWeapon))
			{
				int num2 = Mathf.Min(timingData.enabledWeaponCount[currentActiveWeapon], fireTiming.groupFirePeriod.Length);
				num *= fireTiming.groupFirePeriod[Mathf.Max(0, num2 - 1)];
			}
			MisfireTimingData misfireTimingData = timingData.misfireData[currentActiveWeapon];
			float num3 = num * (1f + misfireTimingData.misfireDebuffPower);
			return num3 * multiplier;
		}

		private bool TryGetCurrentActiveWeapon(int machineId, out ItemDescriptor activeWeapon)
		{
			foreach (KeyValuePair<int, WeaponFireTimingNode> allWeapon in _allWeapons)
			{
				WeaponFireTimingNode value = allWeapon.Value;
				if (value.weaponOwner.machineId == machineId && value.activeComponent.active)
				{
					activeWeapon = value.itemDescriptorComponent.itemDescriptor;
					return true;
				}
			}
			activeWeapon = null;
			return false;
		}
	}
}
