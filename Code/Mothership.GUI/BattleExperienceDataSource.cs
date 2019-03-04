using Authentication;
using Robocraft.GUI;
using SocialServiceLayer;
using SocialServiceLayer.Photon;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Utility;

namespace Mothership.GUI
{
	internal class BattleExperienceDataSource : IDataSource
	{
		public enum LongPlayWarningMessageType
		{
			NoWarning,
			Warning_50Percent,
			Warning_100Percent
		}

		private bool _isPremium;

		private ISocialRequestFactory _socialRequestFactory;

		private bool _hasExperience;

		private bool _tierIsShown;

		private int _battleExperience;

		private int _partyExperience;

		private int _tierExperience;

		private int _premiumExperience;

		private int _clanRobits;

		private float _longPlayReductionMultiplier;

		private int _clanAverageXp;

		private int _clanTotalXp;

		private int _seasonXp;

		private IServiceRequestFactory _serviceRequestFactory;

		private int _totalPlayerXp;

		private float _brawlXPMultiplier;

		private int _currentLevel;

		private int _newLevel;

		private IncomeScalesResponse _incomeScales;

		private TiersData _tiersData;

		private int _playerRobits;

		private int _playerPremiumRobits;

		public BattleExperienceDataSource(ISocialRequestFactory requestFactory, IServiceRequestFactory serviceRequestFactory)
		{
			_socialRequestFactory = requestFactory;
			_serviceRequestFactory = serviceRequestFactory;
		}

		public int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0)
			{
				return _hasExperience ? 1 : 0;
			}
			return 0;
		}

		public T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			if (typeof(T) == typeof(int))
			{
				return (T)Convert.ChangeType(GetInt((BattleExperienceFields)uniqueIdentifier1), typeof(T));
			}
			if (typeof(T) == typeof(bool) && uniqueIdentifier1 == 22)
			{
				return (T)Convert.ChangeType(_tierIsShown, typeof(T));
			}
			if (typeof(T) == typeof(LongPlayWarningMessageType) && uniqueIdentifier1 == 21)
			{
				if (_longPlayReductionMultiplier == 1f)
				{
					return (T)Convert.ChangeType(LongPlayWarningMessageType.NoWarning, typeof(T));
				}
				if (_longPlayReductionMultiplier >= 0.49999f && _longPlayReductionMultiplier <= 0.50001f)
				{
					return (T)Convert.ChangeType(LongPlayWarningMessageType.Warning_50Percent, typeof(T));
				}
				return (T)Convert.ChangeType(LongPlayWarningMessageType.Warning_100Percent, typeof(T));
			}
			if (typeof(T) == typeof(string))
			{
				switch (uniqueIdentifier1)
				{
				case 14:
					return (T)Convert.ChangeType(GetPartyXPCaption(), typeof(T));
				case 16:
					return (T)Convert.ChangeType(GetTierXPCaption(), typeof(T));
				case 15:
					return (T)Convert.ChangeType(GetPremiumXPCaption(), typeof(T));
				case 17:
					throw new Exception("Mastery references were removed from UI!");
				case 20:
					return (T)Convert.ChangeType(GetLongPlayCaption(), typeof(T));
				case 10:
					return (T)Convert.ChangeType(GetClanAverageXPString(), typeof(T));
				case 0:
					return (T)Convert.ChangeType(GetBattleXPString(), typeof(T));
				case 12:
					return (T)Convert.ChangeType(GetClanRobitsString(), typeof(T));
				default:
				{
					int @int = GetInt((BattleExperienceFields)uniqueIdentifier1);
					if (uniqueIdentifier1 == 7 || uniqueIdentifier1 == 8)
					{
						return (T)Convert.ChangeType(@int, typeof(T));
					}
					return (T)Convert.ChangeType(GetNumberWithCommas(@int), typeof(T));
				}
				}
			}
			Console.LogError("Cannot convert data field " + uniqueIdentifier1 + " to " + typeof(T));
			return default(T);
		}

		public IEnumerator RefreshData()
		{
			bool finished = false;
			RefreshData(delegate
			{
				finished = true;
			}, delegate(ServiceBehaviour behaviour)
			{
				ErrorWindow.ShowServiceErrorWindow(behaviour);
				finished = true;
			});
			while (!finished)
			{
				yield return null;
			}
		}

		public void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			TaskRunner.get_Instance().Run(RefreshDataAsTask(OnSuccess, OnFailed));
		}

		public void SetPremiumState(bool isPremium)
		{
			_isPremium = isPremium;
		}

		private int GetInt(BattleExperienceFields field)
		{
			switch (field)
			{
			case BattleExperienceFields.BATTLE_XP:
				return GetBattleXP();
			case BattleExperienceFields.PARTY_XP:
				return GetPartyXP();
			case BattleExperienceFields.LONG_PLAY_REDUCTION_ACTUAL_AMOUNT:
			{
				int totalGainedXPNotCountingLongPlayReduction2 = GetTotalGainedXPNotCountingLongPlayReduction();
				return Convert.ToInt32((float)totalGainedXPNotCountingLongPlayReduction2 * _longPlayReductionMultiplier);
			}
			case BattleExperienceFields.TIER_XP:
				return GetTierXP();
			case BattleExperienceFields.MASTERY_XP:
				throw new Exception("Mastery references were removed from UI!");
			case BattleExperienceFields.PREMIUM_XP:
				return GetPremiumXP();
			case BattleExperienceFields.TOTAL_GAINED_XP:
			{
				int totalGainedXPNotCountingLongPlayReduction = GetTotalGainedXPNotCountingLongPlayReduction();
				return Convert.ToInt32((float)totalGainedXPNotCountingLongPlayReduction * _longPlayReductionMultiplier);
			}
			case BattleExperienceFields.TOTAL_XP:
				return GetTotalPlayerXP();
			case BattleExperienceFields.LEVEL:
				return GetCurrentLevel();
			case BattleExperienceFields.LEVELS_GAINED:
				return GetLevelsGained();
			case BattleExperienceFields.SEASON_XP:
				return GetSeasonXP();
			case BattleExperienceFields.CLAN_TOTAL_XP:
				return GetClanTotalXP();
			case BattleExperienceFields.CLAN_AVERAGE_XP:
				return GetClanAverageXP();
			case BattleExperienceFields.CLAN_ROBITS:
				return GetClanRobits();
			case BattleExperienceFields.ROBITS:
				return GetPlayerRobits();
			case BattleExperienceFields.PREMIUM_ROBITS:
				return GetPlayerPremiumRobits();
			default:
				Console.LogError("Cannot convert data field " + field + " to int");
				return 0;
			}
		}

		private string GetClanRobitsString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strRobits"));
			stringBuilder.Append(" - ");
			stringBuilder.Append(GetClanRobits());
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private string GetClanAverageXPString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("(");
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strAverage"));
			stringBuilder.Append(" - ");
			stringBuilder.Append(GetNumberWithCommas(GetClanAverageXP()));
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private string GetTierXPCaption()
		{
			string @string = StringTableBase<StringTable>.Instance.GetString("strTierBonus");
			if (_tierIsShown)
			{
				return @string;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(@string);
			stringBuilder.Append(" (");
			stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strUpToPercentage", "{NUMBER}", GetMaxTierBonusPercent().ToString()));
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private int GetMaxTierBonusPercent()
		{
			return (int)Math.Floor((double)_tiersData.TiersBands.Length * _incomeScales.BonusPerTierMultiplier * 100.0);
		}

		private string GetLongPlayCaption()
		{
			string @string = StringTableBase<StringTable>.Instance.GetString("strLongPlayLabel");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(@string);
			stringBuilder.Append(" (-");
			int totalGainedXPNotCountingLongPlayReduction = GetTotalGainedXPNotCountingLongPlayReduction();
			int x = Convert.ToInt32((float)totalGainedXPNotCountingLongPlayReduction * (1f - _longPlayReductionMultiplier));
			stringBuilder.Append(GetNumberWithCommas(x));
			stringBuilder.Append(")");
			return stringBuilder.ToString();
		}

		private string GetPremiumXPCaption()
		{
			string @string = StringTableBase<StringTable>.Instance.GetString("strPremiumBonus");
			if (_isPremium)
			{
				return @string;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(@string);
			stringBuilder.Append(" (+");
			stringBuilder.Append(_incomeScales.PremiumXpBonusPercent.ToString());
			stringBuilder.Append("%)");
			return stringBuilder.ToString();
		}

		private string GetBattleXPString()
		{
			if (_brawlXPMultiplier != 1f)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(GetNumberWithCommas(GetBattleXP()));
				stringBuilder.Append(" ");
				stringBuilder.Append(" ( x ");
				stringBuilder.Append(Math.Round(_brawlXPMultiplier, 2, MidpointRounding.ToEven));
				stringBuilder.Append(" ");
				stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strBrawlBonus"));
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
			return GetNumberWithCommas(GetBattleXP());
		}

		private string GetPartyXPCaption()
		{
			string @string = StringTableBase<StringTable>.Instance.GetString("strPartyBonus");
			if (GetPartyXP() == 0)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(@string);
				stringBuilder.Append(" (");
				stringBuilder.Append(StringTableBase<StringTable>.Instance.GetReplaceString("strUpToPercentage", "{NUMBER}", GetMaxPartyBonusPercent().ToString()));
				stringBuilder.Append(")");
				return stringBuilder.ToString();
			}
			return @string;
		}

		private int GetMaxPartyBonusPercent()
		{
			return 5 * _incomeScales.PartyBonusPercentagePerPlayer;
		}

		private int GetBattleXP()
		{
			return _battleExperience;
		}

		private int GetPartyXP()
		{
			return _partyExperience;
		}

		private int GetTierXP()
		{
			return _tierExperience;
		}

		private int GetPremiumXP()
		{
			return _premiumExperience;
		}

		private float GetLongPlayReductionMultiplier()
		{
			return _longPlayReductionMultiplier;
		}

		private int GetClanRobits()
		{
			return _clanRobits;
		}

		private int GetClanAverageXP()
		{
			return _clanAverageXp;
		}

		private int GetClanTotalXP()
		{
			return _clanTotalXp;
		}

		private int GetSeasonXP()
		{
			return _seasonXp;
		}

		private int GetTotalPlayerXP()
		{
			return _totalPlayerXp;
		}

		private int GetCurrentLevel()
		{
			return _currentLevel;
		}

		private int GetLevelsGained()
		{
			return _newLevel - _currentLevel;
		}

		private int GetTotalGainedXPNotCountingLongPlayReduction()
		{
			int battleXP = GetBattleXP();
			battleXP += GetPartyXP();
			battleXP += GetTierXP();
			return battleXP + GetPremiumXP();
		}

		private int GetPlayerRobits()
		{
			return _playerRobits;
		}

		private int GetPlayerPremiumRobits()
		{
			return _playerPremiumRobits;
		}

		private IEnumerator RefreshDataAsTask(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			string username = User.Username;
			_hasExperience = false;
			IHasNewPreviousBattleRewardsRequest request = _socialRequestFactory.Create<IHasNewPreviousBattleRewardsRequest>();
			request.Inject(username);
			TaskService<bool> task = new TaskService<bool>(request);
			yield return task;
			if (!task.succeeded)
			{
				RemoteLogger.Error(task.behaviour.errorBody, string.Empty, Environment.StackTrace);
				OnFailed(task.behaviour);
				yield break;
			}
			_hasExperience = task.result;
			Console.Log("HasNewPreviousBattleRewardsRequest returned " + _hasExperience);
			if (!_hasExperience)
			{
				OnSuccess();
				yield break;
			}
			IGetNewPreviousBattleRewardsRequest getBattleRewardsRequest = _socialRequestFactory.Create<IGetNewPreviousBattleRewardsRequest>();
			ILoadTotalXPRequest getTotalXpRequest = _serviceRequestFactory.Create<ILoadTotalXPRequest>();
			ILoadPlayerLevelDataRequest getLevelDataRequest = _serviceRequestFactory.Create<ILoadPlayerLevelDataRequest>();
			TaskService<GetNewPreviousBattleRequestData> getBattleRewardsTask = new TaskService<GetNewPreviousBattleRequestData>(getBattleRewardsRequest);
			TaskService<uint[]> getTotalXpTask = new TaskService<uint[]>(getTotalXpRequest);
			TaskService<IDictionary<uint, uint>> getLevelDataTask = new TaskService<IDictionary<uint, uint>>(getLevelDataRequest);
			TaskService<IncomeScalesResponse> getIncomeScaleRequestTask = _serviceRequestFactory.Create<ILoadIncomeScalesPremiumFactorRequest>().AsTask();
			TaskService<TiersData> getTierBandingTask = _serviceRequestFactory.Create<ILoadTiersBandingRequest>().AsTask();
			getBattleRewardsRequest.Inject(username);
			ParallelTaskCollection parallel = new ParallelTaskCollection();
			parallel.Add(getBattleRewardsTask);
			parallel.Add(getTotalXpTask);
			parallel.Add(getLevelDataTask);
			parallel.Add(getIncomeScaleRequestTask);
			parallel.Add(getTierBandingTask);
			yield return parallel;
			if (!getBattleRewardsTask.succeeded)
			{
				OnFailed(getBattleRewardsTask.behaviour);
				yield break;
			}
			if (!getTotalXpTask.succeeded)
			{
				OnFailed(getTotalXpTask.behaviour);
				yield break;
			}
			if (!getLevelDataTask.succeeded)
			{
				OnFailed(getLevelDataTask.behaviour);
				yield break;
			}
			if (!getIncomeScaleRequestTask.succeeded)
			{
				OnFailed(getIncomeScaleRequestTask.behaviour);
				yield break;
			}
			if (!getTierBandingTask.succeeded)
			{
				OnFailed(getTierBandingTask.behaviour);
				yield break;
			}
			GetNewPreviousBattleRequestData data = getBattleRewardsTask.result;
			_tierIsShown = (data.xpAwardTierPart >= 0);
			_battleExperience = data.xpAwardBase;
			_clanRobits = data.robitsTotal;
			_clanTotalXp = data.ClanTotalXP;
			_clanAverageXp = data.averageSeasonXPForEveryone;
			_partyExperience = data.xpAwardPartyPart;
			_tierExperience = ((data.xpAwardTierPart >= 0) ? data.xpAwardTierPart : 0);
			_premiumExperience = data.xpAwardPremiumPart;
			_seasonXp = data.newSeasonXP;
			_brawlXPMultiplier = data.xpMultiplierForBrawl;
			_longPlayReductionMultiplier = data.longPlayMultiplier;
			_playerRobits = data.robits;
			_playerPremiumRobits = data.premiumRobits;
			_totalPlayerXp = (int)getTotalXpTask.result[0];
			int newPlayerXp = _totalPlayerXp + (int)((float)GetTotalGainedXPNotCountingLongPlayReduction() * _longPlayReductionMultiplier);
			_currentLevel = CalculateLevel(getLevelDataTask.result, _totalPlayerXp);
			_newLevel = CalculateLevel(getLevelDataTask.result, newPlayerXp);
			_incomeScales = getIncomeScaleRequestTask.result;
			_tiersData = getTierBandingTask.result;
			OnSuccess();
		}

		private static int CalculateLevel(IDictionary<uint, uint> levelData, int experience)
		{
			int num = 0;
			for (uint num2 = 1u; num2 < levelData.Count; num2++)
			{
				if (experience < levelData[num2])
				{
					return num;
				}
				num++;
			}
			return num;
		}

		private static string GetNumberWithCommas(int x)
		{
			return x.ToString("N0", CultureInfo.InvariantCulture);
		}
	}
}
