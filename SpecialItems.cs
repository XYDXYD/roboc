using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

internal sealed class SpecialItems : IInitialize
{
	private bool _initialized;

	private Action _onReady = delegate
	{
	};

	private IDictionary<uint, SpecialItemListData> _specialItems = new Dictionary<uint, SpecialItemListData>();

	[Inject]
	public IServiceRequestFactory serviceFactory
	{
		private get;
		set;
	}

	public List<uint> specialItemKeys
	{
		get;
		private set;
	}

	public void RunWhenSpecialItemListReady(Action onReady)
	{
		if (_initialized)
		{
			onReady();
		}
		else
		{
			_onReady = (Action)Delegate.Combine(_onReady, onReady);
		}
	}

	public bool IsItemValid(uint id)
	{
		return _specialItems.ContainsKey(id);
	}

	public uint GetMoterhshipSize(uint id)
	{
		return _specialItems[id].motherhsipSize;
	}

	public void GetGraphicData(uint id, out string name, out string spriteName)
	{
		SpecialItemListData specialItemListData = _specialItems[id];
		name = SpecialItemsStringTable.Instance.GetString(specialItemListData.keyName + "Name");
		spriteName = specialItemListData.spriteName;
	}

	internal SpecialItemListData SpecialItemDataOf(uint id)
	{
		if (_specialItems.ContainsKey(id))
		{
			return _specialItems[id];
		}
		return null;
	}

	void IInitialize.OnDependenciesInjected()
	{
		ILoadSpecialItemListRequest loadSpecialItemListRequest = serviceFactory.Create<ILoadSpecialItemListRequest>();
		loadSpecialItemListRequest.SetAnswer(new ServiceAnswer<ReadOnlyDictionary<uint, SpecialItemListData>>(OnSpecialItemsLoaded, OnFail));
		loadSpecialItemListRequest.Execute();
	}

	private void OnSpecialItemsLoaded(ReadOnlyDictionary<uint, SpecialItemListData> specialItemList)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		_specialItems = (IDictionary<uint, SpecialItemListData>)(object)specialItemList;
		specialItemKeys = new List<uint>(_specialItems.Keys);
		_initialized = true;
		_onReady();
		_onReady = delegate
		{
		};
	}

	private void OnFail(ServiceBehaviour behaviour)
	{
		ErrorWindow.ShowServiceErrorWindow(behaviour);
	}
}
