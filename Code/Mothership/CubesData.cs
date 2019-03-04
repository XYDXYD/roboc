using Svelto.DataStructures;
using Svelto.IoC;
using Svelto.ServiceLayer;
using System;
using System.Collections.Generic;

namespace Mothership
{
	internal sealed class CubesData : IInitialize, ICubesData
	{
		internal class CubeForgeItemData
		{
			public ForgeItemAppearance ForgeItemAppearanceState;

			public bool IsLeagueBadge;

			public int UnlockingLeague;

			public bool IsCosmetic;

			public CubeForgeItemData()
			{
				ForgeItemAppearanceState = ForgeItemAppearance.NotShownInForge;
				IsLeagueBadge = false;
				UnlockingLeague = 0;
			}
		}

		private bool _catalogDataLoaded;

		private bool _hasEverPlayedLeague = true;

		private Dictionary<CubeTypeID, CubeForgeItemData> _cubeTypeInfo;

		[Inject]
		internal IServiceRequestFactory serviceFactory
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

		public bool isReady => _catalogDataLoaded;

		public bool IsLeagueBadge(CubeTypeID cubeTypeId)
		{
			return _cubeTypeInfo.ContainsKey(cubeTypeId) && _cubeTypeInfo[cubeTypeId].IsLeagueBadge;
		}

		public ForgeItemAppearance GetForgeItemTypeAppearance(CubeTypeID cubeTypeId)
		{
			return _cubeTypeInfo[cubeTypeId].ForgeItemAppearanceState;
		}

		public bool GetIsCosmetic(CubeTypeID cubeTypeId)
		{
			if (_cubeTypeInfo.ContainsKey(cubeTypeId))
			{
				return _cubeTypeInfo[cubeTypeId].IsCosmetic;
			}
			return false;
		}

		void IInitialize.OnDependenciesInjected()
		{
			_cubeTypeInfo = new Dictionary<CubeTypeID, CubeForgeItemData>();
			ILoadCubeListRequest loadCubeListRequest = serviceFactory.Create<ILoadCubeListRequest>();
			loadCubeListRequest.SetAnswer(new ServiceAnswer<ReadOnlyDictionary<CubeTypeID, CubeListData>>(OnLoadCatalogResponse));
			loadCubeListRequest.Execute();
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
					KeyValuePair<CubeTypeID, CubeListData> current = enumerator.get_Current();
					CubeTypeID key = current.Key;
					if (!_cubeTypeInfo.TryGetValue(key, out CubeForgeItemData value))
					{
						value = new CubeForgeItemData();
						_cubeTypeInfo[key] = value;
					}
					value.IsLeagueBadge = ((current.Value.specialCubeKind == SpecialCubesKind.LeagueBadge) ? true : false);
					value.UnlockingLeague = current.Value.leagueUnlockIndex;
					value.IsCosmetic = current.Value.isCosmetic;
				}
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
		}
	}
}
