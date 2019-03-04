using Robocraft.GUI;
using Services.Web.Photon;
using Svelto.ServiceLayer;
using System;

namespace Mothership
{
	internal class CustomGameOptionsDataSource : DataSourceBase
	{
		public enum OptionEnum
		{
			HealthRegenYesNo,
			CaptureSegmentMemoryYesNo,
			BaseShieldsGoDownYesNo,
			DamageMultiplier,
			HealthMultiplier,
			PowerMultiplier,
			GameTimeValue,
			CaptureEliminationTimeValue,
			PointsMultipliedByKillStreakOnOff,
			PointsTotalRequiredValue,
			NumberOfKillsToWin,
			RespawnTimeBattleArenaValue,
			RespawnTimeTeamDeathmatchValue,
			RespawnTimeThePitValue,
			CoreAppearFrequencyValue,
			CoreHealthMultiplier,
			CoreDestroyTimeValue,
			CaptureSpeedBattleArenaMultiplier,
			BattleArenaProtoniumHarvestRateMultiplier,
			CeilingMultiplier,
			MinCPUValue,
			MaxCPUValue
		}

		public Action onDataChanged;

		private IServiceRequestFactory _serviceRequestFactory;

		private bool _dataAvailable;

		private object[] _allOptionSettings;

		private Action _successCallBack;

		public CustomGameOptionsDataSource(IServiceRequestFactory serviceFactory)
		{
			_serviceRequestFactory = serviceFactory;
			_allOptionSettings = new object[Enum.GetNames(typeof(OptionEnum)).Length];
			_dataAvailable = false;
		}

		public override int NumberOfDataItemsAvailable(int dimension)
		{
			if (dimension == 0 && _dataAvailable)
			{
				return Enum.GetNames(typeof(OptionEnum)).Length;
			}
			return 0;
		}

		public override T QueryData<T>(int uniqueIdentifier1, int uniqueIdentifier2)
		{
			base.ValidateData(uniqueIdentifier1, uniqueIdentifier2);
			T val = default(T);
			if (uniqueIdentifier1 < 0 || uniqueIdentifier1 >= Enum.GetNames(typeof(OptionEnum)).Length)
			{
				throw new InvalidDataIndexException(uniqueIdentifier1, uniqueIdentifier2, GetType().Name);
			}
			if (typeof(T) == typeof(bool))
			{
				try
				{
					bool flag = Convert.ToBoolean(_allOptionSettings[uniqueIdentifier1]);
					return (T)Convert.ChangeType(flag, typeof(T));
				}
				catch
				{
					val = default(T);
				}
			}
			if (typeof(T) == typeof(float))
			{
				try
				{
					float num = NormaliseSettingValueFromMultiplier(Convert.ToInt32(_allOptionSettings[uniqueIdentifier1]), (OptionEnum)uniqueIdentifier1);
					return (T)Convert.ChangeType(num, typeof(T));
				}
				catch
				{
					val = default(T);
				}
			}
			if (typeof(T) == typeof(string))
			{
				return CustomGameOptionsUtility.FormatHintTipForCustomGameOption<T>((OptionEnum)uniqueIdentifier1, _allOptionSettings[uniqueIdentifier1]);
			}
			return default(T);
		}

		public override void RefreshData(Action OnSuccess, Action<ServiceBehaviour> OnFailed)
		{
			IRetrieveCustomGameSessionRequest retrieveCustomGameSessionRequest = _serviceRequestFactory.Create<IRetrieveCustomGameSessionRequest>();
			retrieveCustomGameSessionRequest.ClearCache();
			retrieveCustomGameSessionRequest.SetAnswer(new ServiceAnswer<RetrieveCustomGameSessionRequestData>(delegate(RetrieveCustomGameSessionRequestData successData)
			{
				OnSuccessResponse(successData, OnSuccess);
			}, delegate(ServiceBehaviour behaviour)
			{
				OnFailed(behaviour);
			})).Execute();
		}

		private void OnSuccessResponse(RetrieveCustomGameSessionRequestData data, Action FinishedCallback)
		{
			if (data.Data != null)
			{
				_dataAvailable = true;
				for (int i = 0; i < CustomGameOptionsUtility.customGameObjectTypes.Count; i++)
				{
					CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig = CustomGameOptionsUtility.customGameObjectTypes[i];
					string settingString = customGameOptionsConfig.SettingString;
					string value = data.Data.Config[settingString];
					CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig2 = CustomGameOptionsUtility.customGameObjectTypes[i];
					if (customGameOptionsConfig2.DataType == typeof(int))
					{
						_allOptionSettings[i] = Convert.ToInt32(value);
						continue;
					}
					CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig3 = CustomGameOptionsUtility.customGameObjectTypes[i];
					if (customGameOptionsConfig3.DataType == typeof(bool))
					{
						_allOptionSettings[i] = Convert.ToBoolean(value);
					}
				}
				TriggerAllDataChanged();
			}
			FinishedCallback();
		}

		private float NormaliseSettingValueFromMultiplier(int multiplierValue, OptionEnum option)
		{
			int num = 0;
			int num2 = 1;
			int num3 = 1;
			CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig = CustomGameOptionsUtility.customGameObjectTypes[(int)option];
			CustomGameOptionsUtility.SliderSettings? sliderSettings = customGameOptionsConfig.SliderSettings;
			if (sliderSettings.HasValue)
			{
				CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig2 = CustomGameOptionsUtility.customGameObjectTypes[(int)option];
				CustomGameOptionsUtility.SliderSettings value = customGameOptionsConfig2.SliderSettings.Value;
				num2 = value.High;
				num = value.Low;
				num3 = value.Step;
				int num4 = num2 - num;
				return (float)(multiplierValue - num) / (float)num4;
			}
			return 0f;
		}

		public int TranslateNormalisedValueToPercentageMultiplier(OptionEnum option, float sliderNormalisedValue)
		{
			int num = 0;
			int num2 = 1;
			int num3 = 1;
			CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig = CustomGameOptionsUtility.customGameObjectTypes[(int)option];
			CustomGameOptionsUtility.SliderSettings? sliderSettings = customGameOptionsConfig.SliderSettings;
			if (sliderSettings.HasValue)
			{
				CustomGameOptionsUtility.CustomGameOptionsConfig customGameOptionsConfig2 = CustomGameOptionsUtility.customGameObjectTypes[(int)option];
				CustomGameOptionsUtility.SliderSettings value = customGameOptionsConfig2.SliderSettings.Value;
				num2 = value.High;
				num = value.Low;
				num3 = value.Step;
				int num4 = num2 - num;
				float num5 = sliderNormalisedValue * (float)num4 + (float)num;
				return (int)num5 / num3 * num3;
			}
			return 0;
		}
	}
}
