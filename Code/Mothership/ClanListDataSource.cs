using Avatars;
using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.Observer;
using Svelto.ServiceLayer;
using System;
using System.Collections;

namespace Mothership
{
	internal class ClanListDataSource : DataSourceBase, IDisposable
	{
		private const int DEFAULT_DAYS_ACTIVE = 50;

		private const int INITIAL_RESULTS_TO_FETCH = 11;

		private const int LIST_EXPANSION_AMOUNT = 20;

		public const int MAX_ENTRIES_TO_VIEW = 1000;

		private int _desiredHistoryToShow = 11;

		private bool _onlyOpenClans = true;

		private string _searchCriteria = string.Empty;

		private ClanPlusAvatarInfo[] _clansData;

		private ISocialRequestFactory _socialRequestFactory;

		private IMultiAvatarLoader _avatarLoader;

		private AvatarAvailableObserver _avatarAvailableObserver;

		private PresetAvatarMap _presetAvatarMap;

		public unsafe ClanListDataSource(ISocialRequestFactory socialRequestFactory, IMultiAvatarLoader avatarLoader, AvatarAvailableObserver avatarAvailableObserver, PresetAvatarMap presetAvatarMap)
		{
			_socialRequestFactory = socialRequestFactory;
			_avatarLoader = avatarLoader;
			_avatarAvailableObserver = avatarAvailableObserver;
			_avatarAvailableObserver.AddAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			_presetAvatarMap = presetAvatarMap;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return (_clansData != null) ? _clansData.GetLength(0) : 0;
			}
			return 0;
		}

		private void HandleOnAvatarAvailable(ref AvatarAvailableData avatarAvailableData)
		{
			if (avatarAvailableData.avatarType != AvatarType.ClanAvatar || NumberOfDataItemsAvailable(0) == 0)
			{
				return;
			}
			int num = 0;
			ClanPlusAvatarInfo[] clansData = _clansData;
			int num2 = 0;
			ClanPlusAvatarInfo clanPlusAvatarInfo;
			while (true)
			{
				if (num2 < clansData.Length)
				{
					clanPlusAvatarInfo = clansData[num2];
					if (clanPlusAvatarInfo.clanName == avatarAvailableData.avatarName)
					{
						break;
					}
					num++;
					num2++;
					continue;
				}
				return;
			}
			clanPlusAvatarInfo.avatarTexture = avatarAvailableData.texture;
			TriggerDataItemChanged(num, 0);
		}

		public void ExpandSearchHistory()
		{
			_desiredHistoryToShow += 20;
			if (_desiredHistoryToShow > 1000)
			{
				_desiredHistoryToShow = 1000;
			}
		}

		public void SetSearchTerm(string searchCriteria, bool onlyOpenClans)
		{
			_searchCriteria = searchCriteria;
			_onlyOpenClans = onlyOpenClans;
			_desiredHistoryToShow = 11;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (_clansData == null)
			{
				return default(T);
			}
			ValidateData(uniqueIdentifier1, uniqueIdentifier2);
			T result = default(T);
			if (typeof(T) == typeof(ClanPlusAvatarInfo))
			{
				ClanPlusAvatarInfo value = _clansData[uniqueIdentifier1];
				try
				{
					return (T)Convert.ChangeType(value, typeof(T));
				}
				catch
				{
					return default(T);
				}
			}
			return result;
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			IGetMyClanInfoRequest getMyClanInfoRequest = _socialRequestFactory.Create<IGetMyClanInfoRequest>();
			getMyClanInfoRequest.SetAnswer(new ServiceAnswer<ClanInfo>(delegate(ClanInfo successData)
			{
				OnMyClanSuccessResponse(successData, OnSuccess, OnFailed);
			}, delegate(ServiceBehaviour behaviour)
			{
				OnFailed(behaviour);
			})).Execute();
		}

		private void OnMyClanSuccessResponse(ClanInfo myClanData, Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			SearchClanDependency dependency = new SearchClanDependency(_searchCriteria, 50, 0, _desiredHistoryToShow, _onlyOpenClans ? new ClanType[1] : new ClanType[2]
			{
				ClanType.Open,
				ClanType.Closed
			});
			ISearchClansRequest searchClansRequest = _socialRequestFactory.Create<ISearchClansRequest>();
			searchClansRequest.SetAnswer(new ServiceAnswer<ClanInfo[]>(delegate(ClanInfo[] successData)
			{
				OnSuccessResponse(successData, OnSuccess, myClanData);
				TriggerAllDataChanged();
			}, delegate(ServiceBehaviour behaviour)
			{
				OnFailed(behaviour);
			}));
			searchClansRequest.Inject(dependency);
			searchClansRequest.Execute();
		}

		private void OnSuccessResponse(ClanInfo[] data, Action FinishedCallback, ClanInfo myClanData)
		{
			int num = -1;
			if (myClanData != null)
			{
				for (int i = 0; i < data.Length; i++)
				{
					if (data[i].ClanName == myClanData.ClanName)
					{
						num = i;
						break;
					}
				}
			}
			if (num > -1)
			{
				_clansData = new ClanPlusAvatarInfo[data.Length - 1];
				ClanInfo[] array = new ClanInfo[data.Length - 1];
				Array.Copy(data, 0, array, 0, num);
				Array.Copy(data, num + 1, array, num, array.Length - num);
			}
			else
			{
				_clansData = new ClanPlusAvatarInfo[data.Length];
			}
			for (int j = 0; j < _clansData.Length; j++)
			{
				_clansData[j] = new ClanPlusAvatarInfo();
				_clansData[j].clanName = data[j].ClanName;
				_clansData[j].clanSize = data[j].ClanSize;
				_clansData[j].ClanType = data[j].ClanType;
				int clanAvatarNumber = data[j].ClanAvatarNumber;
				_clansData[j].avatarTexture = _presetAvatarMap.GetPresetAvatar(clanAvatarNumber);
			}
			IComparer comparer = new ClanSizeComparer(doInverseComparison: true);
			Array.Sort(_clansData, comparer);
			FinishedCallback();
		}

		public unsafe void Dispose()
		{
			_avatarAvailableObserver.RemoveAction(new ObserverAction<AvatarAvailableData>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}
	}
}
