using Simulation.Hardware;
using Svelto.ECS;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class WheelColliderEngine : SingleEntityViewEngine<WheelColliderNode>, IInitialize
	{
		private Dictionary<GameObject, WheelColliderActivationPerMachine> _activators = new Dictionary<GameObject, WheelColliderActivationPerMachine>();

		[Inject]
		public MachineRootContainer machineRootContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerMachinesContainer playerMachineContainer
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColliderCollectionObservable machineColliderCollectionObservable
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			TaskRunner.get_Instance().RunOnSchedule(StandardSchedulers.get_physicScheduler(), (Func<IEnumerator>)PhysicsTick);
		}

		protected override void Add(WheelColliderNode node)
		{
			IWheelColliderComponent wheelColliderComponent = node.wheelColliderComponent;
			WheelColliderData[] wheelData = wheelColliderComponent.wheelData;
			if (wheelData.Length <= 0)
			{
				return;
			}
			Transform cubeRoot = wheelData[0].cubeRoot;
			if (NeedsWheelColliders(node.hardwareOwnerComponent))
			{
				GameObject machineBoard = GameUtility.GetMachineBoard(cubeRoot);
				if (!_activators.ContainsKey(machineBoard))
				{
					Rigidbody machineRigidbody = GameUtility.GetMachineRigidbody(cubeRoot);
					WheelColliderActivationPerMachine value = new WheelColliderActivationPerMachine(machineRigidbody, node.hardwareOwnerComponent.machineId, machineColliderCollectionObservable);
					_activators.Add(machineBoard, value);
				}
				_activators[machineBoard].AddCubes(wheelData);
			}
		}

		protected override void Remove(WheelColliderNode node)
		{
			IWheelColliderComponent wheelColliderComponent = node.wheelColliderComponent;
			WheelColliderData[] wheelData = wheelColliderComponent.wheelData;
			if (wheelData.Length > 0)
			{
				GameObject machineBoard = GameUtility.GetMachineBoard(wheelData[0].cubeRoot);
				if (_activators.ContainsKey(machineBoard) && _activators[machineBoard].RemoveCubes(wheelData) == 0)
				{
					_activators.Remove(machineBoard);
				}
			}
		}

		private IEnumerator PhysicsTick()
		{
			while (true)
			{
				Dictionary<GameObject, WheelColliderActivationPerMachine>.Enumerator enumerator = _activators.GetEnumerator();
				while (enumerator.MoveNext())
				{
					enumerator.Current.Value.PhysicsTick();
				}
				yield return null;
			}
		}

		private bool NeedsWheelColliders(IHardwareOwnerComponent hardwareOwnerComponent)
		{
			if (hardwareOwnerComponent.ownedByMe)
			{
				return true;
			}
			return hardwareOwnerComponent.ownedByAi;
		}
	}
}
