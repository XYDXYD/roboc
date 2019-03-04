using Svelto.Context;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System.Collections;
using System.Collections.Generic;
using Taunts;
using UnityEngine;

namespace Mothership
{
	public class TauntsMothershipController : IInitialize, IWaitForFrameworkDestruction
	{
		private TauntsMothershipView _view;

		[Inject]
		internal IMachineMap machineMap
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
		internal LoadingIconPresenter loadingPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IRobotShopController robotShopController
		{
			private get;
			set;
		}

		[Inject]
		internal ITauntMaskHelper tauntsMaskHelper
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

		public void OnDependenciesInjected()
		{
			RegisterEventListeners();
		}

		public void OnFrameworkDestroyed()
		{
			UnregisterEventListeners();
		}

		private void RegisterEventListeners()
		{
			machineMover.OnMachineMoved += HandleMachineMoved;
			machineMap.OnAddCubeAt += HandleOnCubePlacedByGarage;
			machineMap.OnCubeRemovedAt += HandleOnCubeActuallyRemoved;
			robotShopController.OnPreviewAddCubeAt += HandleOnCubePlacedByShop;
			robotShopController.OnPreviewCubeRemovedAt += HandleOnCubeActuallyRemoved;
		}

		private void UnregisterEventListeners()
		{
			machineMover.OnMachineMoved -= HandleMachineMoved;
			machineMap.OnAddCubeAt -= HandleOnCubePlacedByGarage;
			machineMap.OnCubeRemovedAt -= HandleOnCubeActuallyRemoved;
			robotShopController.OnPreviewAddCubeAt -= HandleOnCubePlacedByShop;
			robotShopController.OnPreviewCubeRemovedAt -= HandleOnCubeActuallyRemoved;
		}

		public void SetView(TauntsMothershipView view)
		{
			_view = view;
		}

		public IEnumerator Initialise()
		{
			loadingPresenter.NotifyLoading("TauntsData");
			IGetTauntsRequest request = serviceFactory.Create<IGetTauntsRequest>();
			TaskService<TauntsDeserialisedData> task = new TaskService<TauntsDeserialisedData>(request);
			yield return new HandleTaskServiceWithError(task, delegate
			{
				loadingPresenter.NotifyLoading("TauntsData");
			}, delegate
			{
				loadingPresenter.NotifyLoadingDone("TauntsData");
			}).GetEnumerator();
			loadingPresenter.NotifyLoadingDone("TauntsData");
			if (task.succeeded)
			{
				tauntsMaskHelper.Initialise(task.result);
			}
			else
			{
				tauntsMaskHelper.Initialise(new TauntsDeserialisedData(new Dictionary<string, object>()));
			}
		}

		private void HandleMachineMoved(Int3 displacement)
		{
			tauntsMaskHelper.MachineWasMoved(displacement);
			_view.MaskIdleAnimationsMoved(displacement);
		}

		private void HandleOnCubeActuallyRemoved(Byte3 location, uint cubeThatWasRemovedID)
		{
			tauntsMaskHelper.CubeRemoved(location, cubeThatWasRemovedID, HideIdleAnimationCallback);
		}

		private void HandleOnCubePlacedByShop(Byte3 location, uint cubeID, byte rotation)
		{
			tauntsMaskHelper.CubePlaced(location, cubeID, rotation, ShowIdleAnimationCallback);
		}

		private void HideIdleAnimationCallback(Byte3 gridLocation, string groupName)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			_view.HideTauntIdleAnimation(new Vector3((float)(int)gridLocation.x, (float)(int)gridLocation.y, (float)(int)gridLocation.z));
		}

		private void ShowIdleAnimationCallback(Byte3 gridLocation, MaskOrientation maskOrientation, string groupName)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			GameObject gameObject = MachineBoard.Instance.board.get_gameObject();
			_view.ShowTauntIdleAnimation(groupName, new Vector3((float)(int)gridLocation.x, (float)(int)gridLocation.y, (float)(int)gridLocation.z), maskOrientation.ToQuaternion(), gameObject);
		}

		private void HandleOnCubePlacedByGarage(Byte3 location, MachineCell cell)
		{
			tauntsMaskHelper.CubePlaced(location, cell.info.persistentCubeData.cubeType.ID, (byte)cell.info.rotationIndex, ShowIdleAnimationCallback);
		}
	}
}
