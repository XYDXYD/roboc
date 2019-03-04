using Simulation.Hardware.Weapons;
using Svelto.Command;
using Svelto.Command.Dispatcher;
using Svelto.DataStructures;
using Svelto.IoC;
using UnityEngine;

namespace Simulation.BattleArena.Equalizer
{
	internal sealed class SpawnEqualizerClientCommand : IDispatchableCommandWithDependency, IDispatchableCommand, ICommand
	{
		private GetEqualizerDependency _getEqDependency;

		[Inject]
		private IMinimapPresenter minimapPresenter
		{
			get;
			set;
		}

		[Inject]
		private LivePlayersContainer livePlayersContainer
		{
			get;
			set;
		}

		[Inject]
		private MachineClusterContainer machineClusterContainer
		{
			get;
			set;
		}

		[Inject]
		private MachineRootContainer machineRootContainer
		{
			get;
			set;
		}

		[Inject]
		private MachineSpawnDispatcher spawnDispatcher
		{
			get;
			set;
		}

		[Inject]
		private NetworkMachineManager machineManager
		{
			get;
			set;
		}

		[Inject]
		private PlayerMachinesContainer playerMachinesContainer
		{
			get;
			set;
		}

		[Inject]
		private PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		private RigidbodyDataContainer rigidbodyDataContainer
		{
			get;
			set;
		}

		[Inject]
		private TeamBasePreloader basePreloader
		{
			get;
			set;
		}

		public IDispatchableCommand Inject(object dependency)
		{
			_getEqDependency = (GetEqualizerDependency)dependency;
			return this;
		}

		public void Execute()
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			PreloadedMachine preloadedEqualizer = basePreloader.GetPreloadedEqualizer();
			SetAllCubesHealth(preloadedEqualizer.machineMap.GetAllInstantiatedCubes());
			minimapPresenter.RegisterEqualizerPosition(_getEqDependency.position);
			ApplyRigidbodyTransform(preloadedEqualizer, _getEqDependency.position, _getEqDependency.rotation);
			BattleArenaExtraData battleArenaExtraData = new BattleArenaExtraData();
			battleArenaExtraData.equalizerHealth = _getEqDependency.totalHealth;
			BattleArenaExtraData baseData = battleArenaExtraData;
			int num = -1;
			SpawnInParametersEntity spawnInParameters = new SpawnInParametersEntity(0, num, playerTeamsContainer.IsMyTeam(num), TargetType.EqualizerCrystal, preloadedEqualizer, baseData);
			spawnDispatcher.EntitySpawnedIn(spawnInParameters);
		}

		private void SetAllCubesHealth(FasterList<InstantiatedCube> allCubes)
		{
			for (int i = 0; i < allCubes.get_Count(); i++)
			{
				allCubes.get_Item(i).initialTotalHealth = _getEqDependency.totalHealth;
			}
		}

		private void ApplyRigidbodyTransform(PreloadedMachine preloadedMachine, Vector3 position, Quaternion rotation)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			Vector3 val = position - rotation * preloadedMachine.allCubeTransforms.get_Item(0).get_localPosition();
			Transform transform = preloadedMachine.machineBoard.get_transform();
			transform.SetPositionAndRotation(val, rotation);
		}
	}
}
