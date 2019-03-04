using Simulation.SinglePlayerCampaign.DataTypes;
using SinglePlayerCampaign.GUI.Mothership.DataTypes;
using SinglePlayerCampaign.GUI.Simulation.Components;
using Svelto.ECS;
using System;
using System.Text;
using UnityEngine;

namespace SinglePlayerCampaign.GUI.Mothership
{
	internal class SinglePlayerCampaignInfoScreen : MonoBehaviour, IWidgetCounterComponent
	{
		[SerializeField]
		private UILabel campaignNameUILabel;

		[SerializeField]
		private UILabel mapNameLabel;

		[SerializeField]
		private UILabel campaignInfoUILabel;

		[SerializeField]
		private UILabel rulesUILabel;

		[SerializeField]
		private UISprite goalSprite;

		[SerializeField]
		private UILabel goalLabel;

		[SerializeField]
		private UIButton backButt;

		[SerializeField]
		private UIButton startButt;

		[SerializeField]
		private UISprite[] playerProgressSprites;

		[SerializeField]
		private UIPopupList difficultyPopupList;

		[SerializeField]
		private UILabel[] playerCompletedWaveUILabels;

		[SerializeField]
		private UILabel numberOfLives;

		[SerializeField]
		private UILabel numberOfLivesHeader;

		[SerializeField]
		private UILabel waveNumber;

		[SerializeField]
		private WaveInfoView[] enemies;

		[SerializeField]
		private UIButton previousWave;

		[SerializeField]
		private UIButton nextWave;

		private const string WAVE_TAG = "strWave";

		private int _currentWave;

		private Color _disabledColor;

		public DispatchOnChange<bool> show
		{
			get;
			set;
		}

		public DispatchOnChange<Campaign?> campaignSet
		{
			get;
			set;
		}

		public DispatchOnChange<bool> backClicked
		{
			get;
			set;
		}

		public DispatchOnChange<SelectedCampaignToStart> startClicked
		{
			get;
			set;
		}

		public DispatchOnChange<int> difficultySelected
		{
			get;
			set;
		}

		public int WidgetCounterMaxValue
		{
			get;
			set;
		}

		public int WidgetCounterValue
		{
			set
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				playerProgressSprites[value].set_color(Color.get_white());
			}
		}

		public SinglePlayerCampaignInfoScreen()
			: this()
		{
		}

		public unsafe void Initialise()
		{
			//IL_0088: Unknown result type (might be due to invalid IL or missing references)
			//IL_0092: Expected O, but got Unknown
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Expected O, but got Unknown
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b3: Expected O, but got Unknown
			//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b8: Expected O, but got Unknown
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d9: Expected O, but got Unknown
			//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Expected O, but got Unknown
			//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fa: Expected O, but got Unknown
			//IL_010c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0116: Expected O, but got Unknown
			//IL_0111: Unknown result type (might be due to invalid IL or missing references)
			//IL_011b: Expected O, but got Unknown
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_013a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0144: Expected O, but got Unknown
			//IL_0144: Unknown result type (might be due to invalid IL or missing references)
			//IL_014e: Expected O, but got Unknown
			int instanceID = this.get_gameObject().GetInstanceID();
			show = new DispatchOnChange<bool>(instanceID);
			show.NotifyOnValueSet((Action<int, bool>)delegate(int _, bool v)
			{
				this.get_gameObject().SetActive(v);
			});
			campaignSet = new DispatchOnChange<Campaign?>(instanceID);
			campaignSet.NotifyOnValueSet((Action<int, Campaign?>)delegate(int entityId, Campaign? campaign)
			{
				ShowInfo(campaign);
			});
			backClicked = new DispatchOnChange<bool>(instanceID);
			startClicked = new DispatchOnChange<SelectedCampaignToStart>(instanceID);
			difficultySelected = new DispatchOnChange<int>(instanceID);
			backButt.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			startButt.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			difficultyPopupList.onChange.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			previousWave.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			nextWave.onClick.Add(new EventDelegate(new Callback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/)));
			_disabledColor = playerProgressSprites[0].get_color();
			Localization.onLocalize = Delegate.Combine((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void OnDestroy()
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Expected O, but got Unknown
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Expected O, but got Unknown
			Localization.onLocalize = Delegate.Remove((Delegate)Localization.onLocalize, (Delegate)new OnLocalizeNotification((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void ReLocalize()
		{
			if (campaignSet.get_value().HasValue)
			{
				ShowInfo(campaignSet.get_value());
			}
		}

		private void ShowInfo(Campaign? campaign)
		{
			Campaign value = campaign.Value;
			campaignNameUILabel.set_text(StringTableBase<StringTable>.Instance.GetString(value.CampaignName));
			mapNameLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCustomGameMapName" + value.MapName));
			campaignInfoUILabel.set_text(StringTableBase<StringTable>.Instance.GetString(value.CampaignDesc));
			difficultyPopupList.set_value(difficultyPopupList.items[0]);
			UpdateRules();
			UpdateGoal();
			_currentWave = 0;
			UpdateWaveEnemies();
			UpdatePlayerCompletedWaves(value);
			UpdateBadges(value);
		}

		private void UpdateRules()
		{
			int num = difficultyPopupList.items.FindIndex((string x) => difficultyPopupList.get_value() == x);
			Campaign value = campaignSet.get_value().Value;
			CampaignDifficultySetting campaignDifficultySetting = value.DifficultySettings[num];
			PlayerSetting playerDifficultySetting = campaignDifficultySetting.PlayerDifficultySetting;
			int totalLives = playerDifficultySetting.TotalLives;
			numberOfLives.set_text(totalLives.ToString());
			numberOfLivesHeader.set_text(StringTableBase<StringTable>.Instance.GetString((totalLives <= 1) ? "strLife" : "strLives"));
			StringBuilder stringBuilder = new StringBuilder();
			string[] rules = value.Rules;
			string[][] parameters = value.Parameters;
			for (int i = 0; i < rules.Length; i++)
			{
				string text = StringTableBase<StringTable>.Instance.GetString(rules[i]);
				string[] array = parameters[i];
				int num2 = array.Length;
				if (num2 > 0)
				{
					object[] array2 = new object[num2];
					for (int j = 0; j < num2; j++)
					{
						string text2 = array[j];
						if (!text2.Contains("str"))
						{
							array2[j] = text2;
						}
						else
						{
							array2[j] = StringTableBase<StringTable>.Instance.GetString(text2);
						}
					}
					text = string.Format(text, array2);
				}
				stringBuilder.AppendLine(text);
			}
			rulesUILabel.set_text(stringBuilder.ToString().Trim());
			rulesUILabel.ResizeCollider();
		}

		private void UpdateGoal()
		{
			Campaign value = campaignSet.get_value().Value;
			switch (value.campaignType)
			{
			case CampaignType.TimedElimination:
				goalSprite.set_spriteName("CampaignMode_TimeElimination");
				goalLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCampaignTimedEliminationDescription"));
				break;
			case CampaignType.Survival:
				goalSprite.set_spriteName("CampaignMode_Survival");
				goalLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCampaignSurvivalDescription"));
				break;
			case CampaignType.Elimination:
				goalSprite.set_spriteName("CampaignMode_Elimination");
				goalLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strCampaignEliminationDescription"));
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}

		private void ShowNextWave()
		{
			_currentWave++;
			UpdateWaveEnemies();
		}

		private void ShowPreviousWave()
		{
			_currentWave--;
			UpdateWaveEnemies();
		}

		private void UpdateWaveEnemies()
		{
			waveNumber.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strWave"), _currentWave + 1));
			Campaign value = campaignSet.get_value().Value;
			WaveData[] campaignWaves = value.campaignWaves;
			WaveRobot[] waveRobots = campaignWaves[_currentWave].WaveRobots;
			int num = waveRobots.Length;
			int num2 = enemies.Length;
			for (int i = 0; i < num2; i++)
			{
				WaveInfoView waveInfoView = enemies[i];
				bool flag = i < num;
				if (flag)
				{
					WaveRobot waveRobot = waveRobots[i];
					waveInfoView.EnemyName = StringTableBase<StringTable>.Instance.GetString(waveRobot.robotName);
					waveInfoView.Weapon = StringTableBase<StringTable>.Instance.GetString(waveRobot.robotWeapon);
					waveInfoView.Movement = StringTableBase<StringTable>.Instance.GetString(waveRobot.robotMovementPart);
					waveInfoView.Rank = StringTableBase<StringTable>.Instance.GetString(waveRobot.robotRank);
				}
				waveInfoView.get_gameObject().SetActive(flag);
			}
			previousWave.set_isEnabled(_currentWave > 0);
			nextWave.set_isEnabled(_currentWave < campaignWaves.Length - 1);
		}

		private void UpdatePlayerCompletedWaves(Campaign campaign)
		{
			int[] playerCompletedWaves = campaign.PlayerCompletedWaves;
			int num = playerCompletedWaves.Length;
			string @string = StringTableBase<StringTable>.Instance.GetString("strCompletedWaveStat");
			int num2 = campaign.campaignWaves.Length;
			for (int i = 0; i < num; i++)
			{
				string text = string.Format(@string, playerCompletedWaves[i], num2);
				playerCompletedWaveUILabels[i].set_text(text.TrimEnd('\n'));
			}
		}

		private void UpdateBadges(Campaign campaign)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			bool[] difficultiesCompletedPerCampaign = campaign.difficultiesCompletedPerCampaign;
			int num2 = WidgetCounterMaxValue = (int)difficultiesCompletedPerCampaign.LongLength;
			for (int i = 0; i < num2; i++)
			{
				if (difficultiesCompletedPerCampaign[i])
				{
					WidgetCounterValue = i;
				}
				else
				{
					playerProgressSprites[i].set_color(_disabledColor);
				}
			}
		}

		private void BackClicked()
		{
			backClicked.set_value(true);
			backClicked.set_value(false);
		}

		private void StartClicked()
		{
			Campaign? value = campaignSet.get_value();
			int difficulty = difficultyPopupList.items.FindIndex((string x) => difficultyPopupList.get_value() == x);
			Campaign value2 = value.Value;
			startClicked.set_value(new SelectedCampaignToStart(value2.CampaignID, difficulty, value2.CampaignName, value2.MapName));
			startClicked.set_value((SelectedCampaignToStart)null);
		}
	}
}
