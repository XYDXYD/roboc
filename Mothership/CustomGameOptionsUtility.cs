using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Mothership
{
	internal static class CustomGameOptionsUtility
	{
		public struct SliderSettings
		{
			public int Low;

			public int High;

			public int Step;

			public SliderSettings(int low_, int high_, int step_)
			{
				Low = low_;
				High = high_;
				Step = step_;
			}
		}

		public struct CustomGameOptionsConfig
		{
			public string SettingString;

			public Type DataType;

			public SliderSettings? SliderSettings;

			public CustomGameOptionsConfig(string setting_, Type dataType_, SliderSettings? sliderSetting_)
			{
				SettingString = setting_;
				DataType = dataType_;
				SliderSettings = sliderSetting_;
			}
		}

		public static readonly IList<CustomGameOptionsConfig> customGameObjectTypes = new ReadOnlyCollection<CustomGameOptionsConfig>(new CustomGameOptionsConfig[22]
		{
			new CustomGameOptionsConfig("HealthRegen", typeof(bool), null),
			new CustomGameOptionsConfig("CaptureSegmentMemory", typeof(bool), null),
			new CustomGameOptionsConfig("BaseShieldsGoDown", typeof(bool), null),
			new CustomGameOptionsConfig("DamageMultiplier", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("HealthMultiplier", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("PowerMultiplier", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("GameTime", typeof(int), new SliderSettings(1, 50, 1)),
			new CustomGameOptionsConfig("CaptureSpeedElimination", typeof(int), new SliderSettings(3, 150, 3)),
			new CustomGameOptionsConfig("PointsKillStreakOnOff", typeof(bool), null),
			new CustomGameOptionsConfig("PointsTotalRequired", typeof(int), new SliderSettings(1, 50, 1)),
			new CustomGameOptionsConfig("NumberOfKillsToWin", typeof(int), new SliderSettings(1, 50, 1)),
			new CustomGameOptionsConfig("RespawnTimeBA", typeof(int), new SliderSettings(1, 50, 1)),
			new CustomGameOptionsConfig("RespawnTimeTDM", typeof(int), new SliderSettings(1, 50, 1)),
			new CustomGameOptionsConfig("RespawnTimePit", typeof(int), new SliderSettings(1, 50, 1)),
			new CustomGameOptionsConfig("CoreAppearFrequency", typeof(int), new SliderSettings(1, 50, 1)),
			new CustomGameOptionsConfig("CoreHealthMultiplier", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("CoreDestroyTimeValue", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("CaptureSpeedBA", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("ProtoniumHarvestBA", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("CeilingMultiplier", typeof(int), new SliderSettings(10, 500, 10)),
			new CustomGameOptionsConfig("MinCPU", typeof(int), new SliderSettings(200, 10000, 200)),
			new CustomGameOptionsConfig("MaxCPU", typeof(int), new SliderSettings(200, 10000, 200))
		});

		public static T FormatHintTipForCustomGameOption<T>(CustomGameOptionsDataSource.OptionEnum optionChoice, object dataToDisplay)
		{
			switch (optionChoice)
			{
			case CustomGameOptionsDataSource.OptionEnum.GameTimeValue:
				return (T)Convert.ChangeType(dataToDisplay.ToString() + ":00", typeof(T));
			case CustomGameOptionsDataSource.OptionEnum.CaptureEliminationTimeValue:
			case CustomGameOptionsDataSource.OptionEnum.RespawnTimeBattleArenaValue:
			case CustomGameOptionsDataSource.OptionEnum.RespawnTimeTeamDeathmatchValue:
			case CustomGameOptionsDataSource.OptionEnum.RespawnTimeThePitValue:
			case CustomGameOptionsDataSource.OptionEnum.CoreDestroyTimeValue:
			{
				int num = Convert.ToInt32(dataToDisplay);
				int num2 = num / 60;
				int num3 = num % 60;
				return (T)Convert.ChangeType(num2.ToString().PadLeft(2, '0') + ":" + num3.ToString().PadLeft(2, '0'), typeof(T));
			}
			case CustomGameOptionsDataSource.OptionEnum.PointsTotalRequiredValue:
			case CustomGameOptionsDataSource.OptionEnum.NumberOfKillsToWin:
			case CustomGameOptionsDataSource.OptionEnum.CoreAppearFrequencyValue:
			case CustomGameOptionsDataSource.OptionEnum.MinCPUValue:
			case CustomGameOptionsDataSource.OptionEnum.MaxCPUValue:
				return (T)Convert.ChangeType(dataToDisplay.ToString(), typeof(T));
			default:
				return (T)Convert.ChangeType(dataToDisplay.ToString() + "%", typeof(T));
			}
		}
	}
}
