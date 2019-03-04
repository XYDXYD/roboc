using Svelto.Context;
using Svelto.IoC;

namespace Mothership
{
	internal sealed class MachineUpdater : IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		internal IMachineBuilder builder
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorGraphUpdater graphUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorBatcher batcher
		{
			private get;
			set;
		}

		[Inject]
		internal MachineColorUpdater colorUpdater
		{
			private get;
			set;
		}

		[Inject]
		internal MachineEditorBuilder machineBuilder
		{
			private get;
			set;
		}

		[Inject]
		internal MachineMover machineMover
		{
			private get;
			set;
		}

		[Inject]
		internal CPUExceededDisplay cpuExceeded
		{
			private get;
			set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			builder.OnPlaceCube += HandleOnPlaceCube;
			builder.OnDeleteCube += HandleOnDeleteCube;
			machineBuilder.OnMachineBuilt += HandleOnMachineBuilt;
			machineMover.OnMachineMoved += HandleOnMachineMoved;
		}

		private void HandleOnMachineMoved(Int3 obj)
		{
			graphUpdater.UpdateResources();
			batcher.StartBatching();
		}

		public void OnFrameworkDestroyed()
		{
			builder.OnPlaceCube -= HandleOnPlaceCube;
			builder.OnDeleteCube -= HandleOnDeleteCube;
			machineBuilder.OnMachineBuilt -= HandleOnMachineBuilt;
			machineMover.OnMachineMoved -= HandleOnMachineMoved;
		}

		private void HandleOnMachineBuilt(uint garageSlot)
		{
			graphUpdater.Initialize();
			batcher.StartBatching();
		}

		private void HandleOnDeleteCube(InstantiatedCube cube)
		{
			batcher.UpdateOnCubeDeleted(cube);
			colorUpdater.UpdateOnCubeDeleted(cube);
			graphUpdater.UpdateGraphOnCubeDeleted(cube);
			cpuExceeded.UpdateOnCubeDeleted(cube);
		}

		private void HandleOnPlaceCube(InstantiatedCube cube)
		{
			batcher.UpdateOnCubePlaced(cube);
			graphUpdater.UpdateGraphOnCubePlaced(cube);
			colorUpdater.UpdateOnCubePlaced(cube);
		}
	}
}
