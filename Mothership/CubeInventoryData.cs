using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class CubeInventoryData : IInitialize, ICubeInventoryData
	{
		internal class CubeInventoryItemData
		{
			public int UnlockTypeWithRobitsCost;

			public int UnlockTypeWithGCCost;

			public CubeInventoryItemData()
			{
				UnlockTypeWithRobitsCost = 0;
				UnlockTypeWithGCCost = 0;
			}
		}

		private Dictionary<CubeTypeID, CubeInventoryItemData> _cubeTypeInfo;

		private bool _catalogDataLoaded;

		[Inject]
		internal IServiceRequestFactory serviceFactory
		{
			private get;
			set;
		}

		public bool HasLoadedAllData => _catalogDataLoaded;

		void IInitialize.OnDependenciesInjected()
		{
			_cubeTypeInfo = new Dictionary<CubeTypeID, CubeInventoryItemData>();
			ILoadCubeListRequest loadCubeListRequest = serviceFactory.Create<ILoadCubeListRequest>();
			loadCubeListRequest.SetAnswer(new ServiceAnswer<ReadOnlyDictionary<CubeTypeID, CubeListData>>(OnLoadCatalogResponse, OnLoadFailed));
			loadCubeListRequest.Execute();
		}

		public int GetRobitsCostToUnlockType(CubeTypeID cubeTypeId)
		{
			if (_cubeTypeInfo.ContainsKey(cubeTypeId))
			{
				return _cubeTypeInfo[cubeTypeId].UnlockTypeWithRobitsCost;
			}
			return 0;
		}

		public int GetGCCostToUnlockType(CubeTypeID cubeTypeId)
		{
			if (_cubeTypeInfo.ContainsKey(cubeTypeId))
			{
				return _cubeTypeInfo[cubeTypeId].UnlockTypeWithGCCost;
			}
			return 0;
		}

		private void OnLoadCatalogResponse(ReadOnlyDictionary<CubeTypeID, CubeListData> result)
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			_catalogDataLoaded = true;
			DictionaryEnumerator<CubeTypeID, CubeListData> enumerator = result.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CubeTypeID key = enumerator.get_Current().Key;
					if (!_cubeTypeInfo.TryGetValue(key, out CubeInventoryItemData value))
					{
						value = new CubeInventoryItemData();
						_cubeTypeInfo[key] = value;
					}
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}

		private void OnLoadFailed(ServiceBehaviour behaviour)
		{
			ErrorWindow.ShowServiceErrorWindow(behaviour);
		}
	}
}
