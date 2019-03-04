using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.Observer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation.TeamBuff
{
	internal sealed class PlayerCubesBuffManager : IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		internal BuffTeamObserver buffTeamObserver
		{
			private get;
			set;
		}

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
		internal ConnectedPlayersContainer connectedPlayerContainer
		{
			private get;
			set;
		}

		[Inject]
		internal NetworkMachineManager networkMachineManager
		{
			get;
			private set;
		}

		[Inject]
		internal PlayerCubesBuffedObservable playerCubesBuffedObservable
		{
			get;
			private set;
		}

		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			get;
			private set;
		}

		unsafe void IInitialize.OnDependenciesInjected()
		{
			buffTeamObserver.AddAction(new ObserverAction<TeamBuffDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		unsafe void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			buffTeamObserver.RemoveAction(new ObserverAction<TeamBuffDependency>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void BuffTeamPlayersCubes(ref TeamBuffDependency dependency)
		{
			ICollection<int> connectedPlayerIds = connectedPlayerContainer.GetConnectedPlayerIds();
			foreach (int item in connectedPlayerIds)
			{
				int playerTeam = playerTeamsContainer.GetPlayerTeam(TargetType.Player, item);
				float num = dependency.teamBuffs[playerTeam];
				if (num > 0f)
				{
					int activeMachine = playerMachinesContainer.GetActiveMachine(TargetType.Player, item);
					IMachineMap machineMap = networkMachineManager.GetMachineMap(TargetType.Player, activeMachine);
					int num2 = 0;
					int num3 = 0;
					FasterList<InstantiatedCube> allInstantiatedCubes = machineMap.GetAllInstantiatedCubes();
					InstantiatedCube[] array = allInstantiatedCubes.ToArrayFast();
					for (int num4 = allInstantiatedCubes.get_Count() - 1; num4 >= 0; num4--)
					{
						InstantiatedCube instantiatedCube = array[num4];
						if ((double)instantiatedCube.cubeBuff > 1.0)
						{
							instantiatedCube.health = Mathf.FloorToInt((float)instantiatedCube.health / instantiatedCube.cubeBuff);
						}
						instantiatedCube.cubeBuff = num;
						instantiatedCube.health = Mathf.CeilToInt((float)instantiatedCube.health * num);
						num2 += instantiatedCube.totalHealth;
						num3 += instantiatedCube.health;
					}
					PlayerCubesBuffedDependency playerCubesBuffedDependency = new PlayerCubesBuffedDependency(activeMachine, num3, num2, num);
					playerCubesBuffedObservable.Dispatch(ref playerCubesBuffedDependency);
				}
			}
		}
	}
}
