using Robocraft.GUI;
using SocialServiceLayer;
using Svelto.ServiceLayer;
using System;
using System.Collections;
using System.Text;
using Utility;

namespace Mothership
{
	internal class YourClanInfoDataSource : IDataSource
	{
		public enum YourClanStringEnum
		{
			ClanNameText,
			DescriptionText,
			InviteTypeText,
			SeasonExperience,
			SeasonRobits,
			ClanSize
		}

		public Action onDataChanged;

		private ISocialRequestFactory _socialRequestFactory;

		private bool _dataAvailable;

		private string _clanNameText;

		private string _descriptionText;

		private string _inviteTypeText;

		private string _seasonExperienceString;

		private string _seasonRobitsString;

		private string _clanSizeString;

		private ClanMember[] _clanMembers;

		private ClanViewMode _clanDataMode = ClanViewMode.NoClan;

		private string _searchCriteria = string.Empty;

		private bool _forceRefresh;

		private const int MAX_CLAN_SIZE = 50;

		private IServiceEventContainer _socialEventContainer;

		private float _robitsConversionFactor;

		public YourClanInfoDataSource(ISocialRequestFactory socialRequestFactory, IServiceEventContainer socialEventContainer)
		{
			_socialRequestFactory = socialRequestFactory;
			_socialEventContainer = socialEventContainer;
		}

		public int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0 && _dataAvailable)
			{
				return Enum.GetNames(typeof(YourClanStringEnum)).Length;
			}
			return 0;
		}

		public void SetSearchTerm(ClanViewMode clanDataMode, string searchCriteria)
		{
			_clanDataMode = clanDataMode;
			_searchCriteria = searchCriteria;
		}

		private void RegisterToSocialEvent()
		{
			_socialEventContainer.ListenTo<IClanMemberXPChangedEventListener, ClanMemberXPChangedEventContent>(HandleOnClanMemberXPChanged);
			_socialEventContainer.ListenTo<IClanDataChangedEventListener, ClanInfo>(HandleOnClanDataChanged);
			_socialEventContainer.ListenTo<IClanMemberJoinedEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoinedOrLeft);
			_socialEventContainer.ListenTo<IClanMemberLeftEventListener, ClanMember[], ClanMember>(HandleOnClanMemberJoinedOrLeft);
		}

		private void HandleOnClanMemberJoinedOrLeft(ClanMember[] clanMembers, ClanMember member)
		{
			int num = 0;
			for (int i = 0; i < clanMembers.Length; i++)
			{
				if (clanMembers[i].ClanMemberState == ClanMemberState.Accepted)
				{
					num++;
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			stringBuilder.Append(num.ToString());
			stringBuilder.Append("/");
			stringBuilder.Append(50.ToString());
			stringBuilder.Append(")");
			_clanSizeString = stringBuilder.ToString();
			SafeEvent.SafeRaise(onDataChanged);
		}

		private void HandleOnClanDataChanged(ClanInfo clanInfo)
		{
			_clanNameText = clanInfo.ClanName;
			_descriptionText = clanInfo.ClanDescription;
			_inviteTypeText = ((clanInfo.ClanType != ClanType.Closed) ? StringTableBase<StringTable>.Instance.GetString("strClanTypeOpenAll") : StringTableBase<StringTable>.Instance.GetString("strClanTypeInviteOnly"));
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			stringBuilder.Append(clanInfo.ClanSize.ToString());
			stringBuilder.Append("/");
			stringBuilder.Append(50.ToString());
			stringBuilder.Append(")");
			_clanSizeString = stringBuilder.ToString();
			SafeEvent.SafeRaise(onDataChanged);
		}

		private void HandleOnClanMemberXPChanged(ClanMemberXPChangedEventContent data)
		{
			int num = 0;
			for (int i = 0; i < _clanMembers.Length; i++)
			{
				ClanMember clanMember = _clanMembers[i];
				if (data.memberName == clanMember.Name)
				{
					clanMember.SeasonXP = data.newXPValue;
				}
				if (clanMember.ClanMemberState == ClanMemberState.Accepted)
				{
					num += clanMember.SeasonXP;
				}
			}
			_seasonExperienceString = num.ToString();
			_seasonRobitsString = ((int)Math.Round((float)num * _robitsConversionFactor)).ToString();
			SafeEvent.SafeRaise(onDataChanged);
		}

		public T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			T result = default(T);
			if (uniqueIdentifier1 < 0 || uniqueIdentifier1 >= Enum.GetNames(typeof(YourClanStringEnum)).Length)
			{
				throw new InvalidDataIndexException(uniqueIdentifier1, uniqueIdentifier2, GetType().Name);
			}
			if (typeof(T) == typeof(string))
			{
				try
				{
					switch (uniqueIdentifier1)
					{
					case 0:
						return (T)Convert.ChangeType(_clanNameText, typeof(T));
					case 1:
						return (T)Convert.ChangeType(_descriptionText, typeof(T));
					case 2:
						return (T)Convert.ChangeType(_inviteTypeText, typeof(T));
					case 4:
						return (T)Convert.ChangeType(_seasonRobitsString, typeof(T));
					case 3:
						return (T)Convert.ChangeType(_seasonExperienceString, typeof(T));
					case 5:
						return (T)Convert.ChangeType(_clanSizeString, typeof(T));
					default:
						return result;
					}
				}
				catch
				{
					return default(T);
				}
			}
			return result;
		}

		public void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			_dataAvailable = false;
			if (_clanDataMode == ClanViewMode.YourClan)
			{
				IGetMyClanInfoAndMembersRequest getMyClanInfoAndMembersRequest = _socialRequestFactory.Create<IGetMyClanInfoAndMembersRequest>();
				getMyClanInfoAndMembersRequest.ForceRefresh = _forceRefresh;
				getMyClanInfoAndMembersRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(delegate(ClanInfoAndMembers successData)
				{
					OnSuccessResponse(successData, OnSuccess);
				}, delegate(ServiceBehaviour behaviour)
				{
					OnFailed(behaviour);
				})).Execute();
			}
			if (_clanDataMode == ClanViewMode.AnotherClan)
			{
				IGetClanInfoAndMembersRequest getClanInfoAndMembersRequest = _socialRequestFactory.Create<IGetClanInfoAndMembersRequest>();
				getClanInfoAndMembersRequest.Inject(_searchCriteria);
				getClanInfoAndMembersRequest.SetAnswer(new ServiceAnswer<ClanInfoAndMembers>(delegate(ClanInfoAndMembers successData)
				{
					OnSuccessResponse(successData, OnSuccess);
				}, delegate(ServiceBehaviour behaviour)
				{
					OnFailed(behaviour);
				})).Execute();
			}
			if (_clanDataMode == ClanViewMode.NoClan)
			{
				OnSuccess();
			}
		}

		private void OnSuccessResponse(ClanInfoAndMembers clanInfo, Action FinishedCallback)
		{
			_clanNameText = clanInfo.ClanInfo.ClanName;
			_descriptionText = clanInfo.ClanInfo.ClanDescription;
			_inviteTypeText = ((clanInfo.ClanInfo.ClanType != ClanType.Closed) ? StringTableBase<StringTable>.Instance.GetString("strClanTypeOpenAll") : StringTableBase<StringTable>.Instance.GetString("strClanTypeInviteOnly"));
			_clanMembers = clanInfo.ClanMembers;
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < _clanMembers.Length; i++)
			{
				ClanMember clanMember = _clanMembers[i];
				if (clanMember.ClanMemberState == ClanMemberState.Accepted)
				{
					num += clanMember.SeasonXP;
					num2++;
				}
			}
			_seasonExperienceString = num.ToString();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			stringBuilder.Append(num2.ToString());
			stringBuilder.Append("/");
			stringBuilder.Append(50.ToString());
			stringBuilder.Append(")");
			_clanSizeString = stringBuilder.ToString();
			_robitsConversionFactor = clanInfo.XPtoRobitsConversionFactor;
			_seasonRobitsString = ((int)Math.Round((float)num * _robitsConversionFactor)).ToString();
			_dataAvailable = true;
			RegisterToSocialEvent();
			FinishedCallback();
		}

		public IEnumerator RefreshData()
		{
			bool finished = false;
			RefreshData(delegate
			{
				finished = true;
			}, delegate
			{
				Console.Log("failed to execute task " + GetType().Name);
				finished = true;
			});
			while (!finished)
			{
				yield return null;
			}
		}

		public void SetForceRefresh(bool forceRefresh)
		{
			_forceRefresh = forceRefresh;
		}
	}
}
