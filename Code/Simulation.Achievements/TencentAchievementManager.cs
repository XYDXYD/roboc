using Achievements;
using rail;
using RoboCraft.MiniJSON;
using Svelto.Context;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace Simulation.Achievements
{
	public class TencentAchievementManager : IAchievementManager, IWaitForFrameworkDestruction
	{
		private readonly Dictionary<ItemCategory, Achievement> KILL_ACHIEVEMENTS = new Dictionary<ItemCategory, Achievement>
		{
			{
				ItemCategory.Laser,
				new Achievement(ItemCategory.Laser, AchievementID.Stat_Kill_LaserCannon)
			},
			{
				ItemCategory.Plasma,
				new Achievement(ItemCategory.Plasma, AchievementID.Stat_Kill_PlasmaCannon)
			},
			{
				ItemCategory.Rail,
				new Achievement(ItemCategory.Rail, AchievementID.Stat_Kill_RailGun)
			},
			{
				ItemCategory.Tesla,
				new Achievement(ItemCategory.Tesla, AchievementID.Stat_Kill_TeslaRam)
			},
			{
				ItemCategory.Aeroflak,
				new Achievement(ItemCategory.Aeroflak, AchievementID.Stat_Kill_Aeroflak)
			},
			{
				ItemCategory.Ion,
				new Achievement(ItemCategory.Ion, AchievementID.Stat_Kill_Ion)
			},
			{
				ItemCategory.Seeker,
				new Achievement(ItemCategory.Seeker, AchievementID.Stat_Kill_Seeker)
			},
			{
				ItemCategory.Chaingun,
				new Achievement(ItemCategory.Chaingun, AchievementID.Stat_Kill_Chaingun)
			},
			{
				ItemCategory.Rotor,
				new Achievement(ItemCategory.Rotor, AchievementID.Stat_Kill_RotorBlade)
			},
			{
				ItemCategory.MechLeg,
				new Achievement(ItemCategory.MechLeg, AchievementID.Stat_Kill_MechLeg)
			},
			{
				ItemCategory.InsectLeg,
				new Achievement(ItemCategory.InsectLeg, AchievementID.Stat_Kill_InsectLeg)
			},
			{
				ItemCategory.Wing,
				new Achievement(ItemCategory.Wing, AchievementID.Stat_Kill_Wing)
			},
			{
				ItemCategory.Wheel,
				new Achievement(ItemCategory.Wheel, AchievementID.Stat_Kill_Wheel)
			},
			{
				ItemCategory.TankTrack,
				new Achievement(ItemCategory.TankTrack, AchievementID.Stat_Kill_TankTrack)
			},
			{
				ItemCategory.Hover,
				new Achievement(ItemCategory.Hover, AchievementID.Stat_Kill_HoverBlade)
			}
		};

		private readonly Dictionary<ItemCategory, Achievement> USE_MODULE_ACHIEVEMENTS = new Dictionary<ItemCategory, Achievement>
		{
			{
				ItemCategory.ShieldModule,
				new Achievement(ItemCategory.ShieldModule, AchievementID.Use_DiscShieldModule_5X, 5)
			},
			{
				ItemCategory.GhostModule,
				new Achievement(ItemCategory.GhostModule, AchievementID.Use_CloakModule_5X, 5)
			},
			{
				ItemCategory.EmpModule,
				new Achievement(ItemCategory.EmpModule, AchievementID.Use_EmpModule_5X, 5)
			},
			{
				ItemCategory.BlinkModule,
				new Achievement(ItemCategory.BlinkModule, AchievementID.Use_TeleportModule_5X, 5)
			},
			{
				ItemCategory.EnergyModule,
				new Achievement(ItemCategory.EnergyModule, AchievementID.Use_EnergyModule_5X, 5)
			},
			{
				ItemCategory.WindowmakerModule,
				new Achievement(ItemCategory.WindowmakerModule, AchievementID.Use_RadarModule_5X, 5)
			}
		};

		private readonly Dictionary<ItemCategory, Achievement> PLAY_ACHIEVEMENTS = new Dictionary<ItemCategory, Achievement>
		{
			{
				ItemCategory.Rotor,
				new Achievement(ItemCategory.Rotor, AchievementID.Stat_Play_RotorBlade)
			},
			{
				ItemCategory.MechLeg,
				new Achievement(ItemCategory.MechLeg, AchievementID.Stat_Play_MechLeg)
			},
			{
				ItemCategory.InsectLeg,
				new Achievement(ItemCategory.InsectLeg, AchievementID.Stat_Play_InsectLeg)
			},
			{
				ItemCategory.Wing,
				new Achievement(ItemCategory.Wing, AchievementID.Stat_Play_Wing)
			},
			{
				ItemCategory.Wheel,
				new Achievement(ItemCategory.Wheel, AchievementID.Stat_Play_Wheel)
			},
			{
				ItemCategory.TankTrack,
				new Achievement(ItemCategory.TankTrack, AchievementID.Stat_Play_TankTrack)
			},
			{
				ItemCategory.Hover,
				new Achievement(ItemCategory.Hover, AchievementID.Stat_Play_HoverBlade)
			}
		};

		private Dictionary<AchievementID, uint> _cachedAchievementStats = new Dictionary<AchievementID, uint>();

		private IRailPlayerAchievement _playerAchievement;

		private RailCallBackHelper _railCallbackHelper = new RailCallBackHelper();

		public TencentAchievementManager()
		{
			//IL_025e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0268: Expected O, but got Unknown
			RegisterRailEvent();
			TaskRunner.get_Instance().Run(Init());
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			UnRegisterRailEvent();
		}

		public void CompletedTutorial()
		{
			CompleteAchievement(AchievementID.Complete_Tutorial);
		}

		public void CompletedBattleWithFullParty()
		{
			AddAchievementStat(AchievementID.Stat_Win_FullParty);
		}

		public void CapturedPoint()
		{
			AddAchievementStat(AchievementID.Stat_CapturePoint);
		}

		public void CompletedHealFrom20To100()
		{
			AddAchievementStat(AchievementID.Stat_Heal_20_To_100);
		}

		public void CompletedKillWithTeslaAfterDecloaked()
		{
			CompleteAchievement(AchievementID.Kill_Tesla_After_2Sec_Decloak);
		}

		public void ReachedMastery10OnCRFBot()
		{
			CompleteAchievement(AchievementID.Earn_Mastery_10_CRF_Bot);
		}

		public void EarnRobitsFromCRF(int count)
		{
			AddAchievementStat(AchievementID.Stat_Earn_Robits_CRF, count);
		}

		public void ReachedRank(uint rank)
		{
			for (int i = 0; i <= rank; i++)
			{
				if (i == 0)
				{
					CompleteAchievement(AchievementID.Earn_BronzeLeague);
				}
				if (i == 1)
				{
					CompleteAchievement(AchievementID.Earn_SilverLeague);
				}
				if (i == 2)
				{
					CompleteAchievement(AchievementID.Earn_GoldLeague);
				}
				if (i == 3)
				{
					CompleteAchievement(AchievementID.Earn_DiamondLeague);
				}
				if (i == 4)
				{
					CompleteAchievement(AchievementID.Earn_ProtoniumLeague);
				}
			}
		}

		public void CompletedBattle(ItemCategory itemCategory)
		{
			if (PLAY_ACHIEVEMENTS.TryGetValue(itemCategory, out Achievement value))
			{
				AddAchievementStat(value.achievementID);
			}
		}

		public void ActivatedModule(ItemCategory moduleCategory, int count = 1)
		{
			if (USE_MODULE_ACHIEVEMENTS.TryGetValue(moduleCategory, out Achievement value) && count >= value.count)
			{
				CompleteAchievement(value.achievementID);
			}
		}

		public void MadeAKill(ItemCategory itemCategory)
		{
			if (KILL_ACHIEVEMENTS.TryGetValue(itemCategory, out Achievement value))
			{
				AddAchievementStat(value.achievementID);
			}
		}

		public void EarnFirstCampaign()
		{
			CompleteAchievement(AchievementID.Play_First_Ranked_Battle);
		}

		public void DeleteAllAchievements()
		{
			TaskRunner.get_Instance().Run(ResetAll());
		}

		private IEnumerator ResetAll()
		{
			yield return Init();
			_playerAchievement.ResetAllAchievements();
		}

		private bool CreatePlayerAchievementManager()
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			if (_playerAchievement == null)
			{
				try
				{
					IRailFactory val = rail_api.RailFactory();
					IRailAchievementHelper val2 = val.RailAchievementHelper();
					RailID val3 = new RailID();
					if (!ulong.TryParse(RailManager_Tencent.RailID, out val3.id_))
					{
						Console.LogError($"Error while parsing the railID {RailManager_Tencent.RailID} in initialise player achievement");
						return false;
					}
					_playerAchievement = val2.CreatePlayerAchievement(val3);
				}
				catch (Exception ex)
				{
					Console.LogError(ex.Message + " " + ex.StackTrace);
					return false;
				}
			}
			return true;
		}

		private void CompleteAchievement(AchievementID achievementID)
		{
			TaskRunner.get_Instance().Run(MakeAchievement(achievementID));
		}

		private IEnumerator MakeAchievement(AchievementID achievementID)
		{
			yield return Init();
			_playerAchievement.MakeAchievement(achievementID.ToString());
			_playerAchievement.AsyncStoreAchievement(string.Empty);
		}

		private void AddAchievementStat(AchievementID achievementID, int count = 1)
		{
			TaskRunner.get_Instance().Run(TriggerAchievementProgress(achievementID, count));
		}

		private IEnumerator TriggerAchievementProgress(AchievementID achievementID, int count = 1)
		{
			yield return Init();
			if (!_cachedAchievementStats.ContainsKey(achievementID))
			{
				string src = default(string);
				RailResult achievementInfo = _playerAchievement.GetAchievementInfo(achievementID.ToString(), ref src);
				if ((int)achievementInfo != 0)
				{
					Console.LogError("GetAchievementInfo failed. Result = " + ((object)achievementInfo).ToString());
					yield break;
				}
				string text = ConvertFromUtf8(src);
				Dictionary<string, object> dictionary = Json.Deserialize(text) as Dictionary<string, object>;
				if (!dictionary.ContainsKey("cur_value"))
				{
					Console.LogError("Error getting current value for the achievement: " + achievementID.ToString());
					yield break;
				}
				_cachedAchievementStats.Add(achievementID, Convert.ToUInt32(dictionary["cur_value"]));
			}
			Dictionary<AchievementID, uint> cachedAchievementStats;
			AchievementID key;
			(cachedAchievementStats = _cachedAchievementStats)[key = achievementID] = (uint)((int)cachedAchievementStats[key] + count);
			_playerAchievement.AsyncTriggerAchievementProgress(achievementID.ToString(), _cachedAchievementStats[achievementID]);
		}

		private IEnumerator Init()
		{
			if (!CreatePlayerAchievementManager())
			{
				Console.LogError("Player Achievement failed to initialise");
				yield break;
			}
			RailResult val = _playerAchievement.AsyncRequestAchievement(string.Empty);
			if ((int)val != 0)
			{
				Console.LogError("AsyncRequestAchievement failed. Result = " + ((object)val).ToString());
			}
		}

		private string ConvertFromUtf8(string src)
		{
			byte[] bytes = Encoding.Default.GetBytes(src);
			return Encoding.UTF8.GetString(bytes);
		}

		private unsafe void RegisterRailEvent()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			_railCallbackHelper.RegisterCallback(2101, new RailEventCallBackHandler((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private unsafe void UnRegisterRailEvent()
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Expected O, but got Unknown
			_railCallbackHelper.UnregisterCallback(2101, new RailEventCallBackHandler((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		private void OnRailEvent(RAILEventID id, EventBase data)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			RailResult result = data.result;
			if ((int)result != 0)
			{
				Console.LogError("Error OnRailEvent,id=" + ((object)id).ToString());
			}
		}
	}
}
