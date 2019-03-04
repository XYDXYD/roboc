using Simulation.Hardware;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.SinglePlayer.Weapons
{
	internal class AIWeaponAccuracyEngine : WeaponAccuracyEngine, IWaitForFrameworkInitialization
	{
		private class MachineInputData
		{
			public IMachineInputComponent machineInputcomponent
			{
				get;
				private set;
			}

			public bool startedFiring
			{
				get;
				set;
			}

			public float startFireTime
			{
				get;
				set;
			}

			public MachineInputData(IMachineInputComponent machineInputcomponent)
			{
				this.machineInputcomponent = machineInputcomponent;
			}

			public void UpdatePressedFireStatus(bool pressedFire)
			{
				if (pressedFire)
				{
					if (!startedFiring)
					{
						startFireTime = Time.get_time();
						startedFiring = true;
					}
				}
				else
				{
					startedFiring = false;
				}
			}
		}

		private Dictionary<int, MachineInputData> _machinInputTable = new Dictionary<int, MachineInputData>();

		public void OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_updateScheduler(), (Func<IEnumerator>)Tick);
		}

		protected override void Add(MachineInputNode node)
		{
			if (!node.ownerComponent.ownedByMe)
			{
				_machinInputTable.Add(node.get_ID(), new MachineInputData(node.machineInput));
			}
		}

		protected override void Remove(MachineInputNode node)
		{
			if (!node.ownerComponent.ownedByMe)
			{
				_machinInputTable.Remove(node.get_ID());
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				using (Dictionary<int, MachineInputData>.Enumerator enumerator = _machinInputTable.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MachineInputData value = enumerator.Current.Value;
						int key = enumerator.Current.Key;
						bool pressedFire = value.machineInputcomponent.fire1 > 0f;
						value.UpdatePressedFireStatus(pressedFire);
						int num = default(int);
						WeaponAccuracyNode[] array = base.entityViewsDB.QueryGroupedEntityViewsAsArray<WeaponAccuracyNode>(key, ref num);
						for (int i = 0; i < num; i++)
						{
							WeaponAccuracyNode weaponAccuracyNode = array[i];
							if (weaponAccuracyNode.disabledComponent.enabled && weaponAccuracyNode.weaponActiveComponent.active)
							{
								UpdateAccuracy(weaponAccuracyNode, pressedFire, value.startFireTime);
							}
						}
					}
				}
				yield return null;
			}
		}
	}
}
