using Mothership;
using Svelto.Command;
using Svelto.Context;
using Svelto.Factories;
using Svelto.IoC;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

internal sealed class CubeInventory : ICubeInventory, IWaitForFrameworkInitialization
{
	private HashSet<uint> _newCubes = new HashSet<uint>();

	private Action _inventoryLoadedCallbacks = delegate
	{
	};

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	[Inject]
	public IGameObjectFactory gofactory
	{
		private get;
		set;
	}

	[Inject]
	internal ICubeList cubeList
	{
		private get;
		set;
	}

	[Inject]
	internal ICubesData cubesData
	{
		private get;
		set;
	}

	[Inject]
	internal ICommandFactory commandFactory
	{
		private get;
		set;
	}

	[Inject]
	internal LoadingIconPresenter loadingIconPresenter
	{
		private get;
		set;
	}

	public HashSet<uint> OwnedCubes
	{
		get;
		set;
	}

	public HashSet<uint> NewCubes => _newCubes;

	public void RegisterInventoryLoadedCallback(Action callback)
	{
		_inventoryLoadedCallbacks = (Action)Delegate.Combine(_inventoryLoadedCallbacks, callback);
	}

	public void DeRegisterInventoryLoadedCallback(Action callback)
	{
		_inventoryLoadedCallbacks = (Action)Delegate.Remove(_inventoryLoadedCallbacks, callback);
	}

	void IWaitForFrameworkInitialization.OnFrameworkInitialized()
	{
		TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadEverything);
	}

	public void RefreshAndForget()
	{
		TaskRunner.get_Instance().Run((Func<IEnumerator>)LoadEverything);
	}

	public IEnumerator RefreshAndWait()
	{
		yield return LoadEverything();
	}

	public bool IsCubeOwned(CubeTypeID type)
	{
		return OwnedCubes.Contains(type.ID) || NewCubes.Contains(type.ID);
	}

	private IEnumerator LoadEverything()
	{
		yield return LoadNewCubes();
		yield return LoadInventory();
		SafeEvent.SafeRaise(_inventoryLoadedCallbacks);
	}

	private IEnumerator LoadInventory()
	{
		EnableLoadingScreen();
		ILoadUserCubeInventoryRequest loadInventoryRequest = serviceFactory.Create<ILoadUserCubeInventoryRequest>();
		TaskService<Dictionary<uint, uint>> loadTask = new TaskService<Dictionary<uint, uint>>(loadInventoryRequest);
		yield return new HandleTaskServiceWithError(loadTask, EnableLoadingScreen, DisableLoadingScreen).GetEnumerator();
		DisableLoadingScreen();
		OwnedCubes = new HashSet<uint>();
		Dictionary<uint, uint> inventoryCounts = loadTask.result;
		foreach (KeyValuePair<uint, uint> item in inventoryCounts)
		{
			if (item.Value != 0)
			{
				OwnedCubes.Add(item.Key);
			}
		}
	}

	private IEnumerator LoadNewCubes()
	{
		EnableLoadingScreen();
		IGetNewInventoryCubesRequest loadNewCubesRequest = serviceFactory.Create<IGetNewInventoryCubesRequest>();
		TaskService<HashSet<uint>> loadTask = new TaskService<HashSet<uint>>(loadNewCubesRequest);
		yield return new HandleTaskServiceWithError(loadTask, EnableLoadingScreen, DisableLoadingScreen).GetEnumerator();
		DisableLoadingScreen();
		_newCubes = loadTask.result;
	}

	private void EnableLoadingScreen()
	{
		loadingIconPresenter.NotifyLoading("CubeInventory");
	}

	private void DisableLoadingScreen()
	{
		loadingIconPresenter.NotifyLoadingDone("CubeInventory");
	}
}
