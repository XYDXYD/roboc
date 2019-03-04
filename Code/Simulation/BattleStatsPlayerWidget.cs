using System.Globalization;
using System.Text;
using UnityEngine;

namespace Simulation
{
	internal class BattleStatsPlayerWidget : MonoBehaviour
	{
		public UILabel playerNameLabel;

		public UILabel killsLabel;

		public UILabel deathsLabel;

		public UILabel assistsLabel;

		public UILabel scoreLabel;

		public UILabel damageLabel;

		public UILabel healLabel;

		public UILabel currentStreakLabel;

		public UILabel pointsLabel;

		public UILabel objectivesLabel;

		public GameObject friendIcon;

		public UISprite bGSprite;

		public Color lightBlueBG = new Color(0f, 0f, 0f);

		public Color darkBlueBG = new Color(0f, 0f, 0f);

		public Color lightRedBG = new Color(0f, 0f, 0f);

		public Color darkRedBG = new Color(0f, 0f, 0f);

		public Color isFriendBG = new Color(0f, 0f, 0f);

		public Color addFriendBG = new Color(0f, 0f, 0f);

		public Color requestBG = new Color(0f, 0f, 0f);

		public UITexture AvatarTexture;

		public UITexture ClanAvatarTexture;

		public UIWidget uiWidget;

		private StringBuilder _stringBuilder = new StringBuilder();

		private string _playerName;

		public string PlayerName => _playerName;

		public BattleStatsPlayerWidget()
			: this()
		{
		}//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)


		public void SetBGColor(bool isMe, bool isMyTeam, int playerIndex)
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0081: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
			Color color = default(Color);
			color._002Ector(1f, 1f, 1f);
			Color color2 = default(Color);
			color2._002Ector(0f, 0f, 0f);
			if (!isMe)
			{
				color2._002Ector(1f, 1f, 1f);
				color = ((!isMyTeam) ? ((playerIndex % 2 != 0) ? darkRedBG : lightRedBG) : ((playerIndex % 2 != 0) ? darkBlueBG : lightBlueBG));
			}
			bGSprite.set_color(color);
			UILabel[] componentsInChildren = this.GetComponentsInChildren<UILabel>(true);
			foreach (UILabel val in componentsInChildren)
			{
				val.set_color(color2);
			}
		}

		public void SetPlayerNameAndClan(string playerName, string displayName, string clanTag)
		{
			_playerName = playerName;
			playerNameLabel.set_text($"{displayName} {clanTag}");
			playerNameLabel.UpdateNGUIText();
			string text = default(string);
			if (playerNameLabel.Wrap(displayName, ref text))
			{
				return;
			}
			_stringBuilder.Length = 0;
			if (text.Length - 3 > 0)
			{
				for (int i = 0; i < text.Length - 3; i++)
				{
					_stringBuilder.Append(text[i]);
				}
				_stringBuilder.Append("...");
				playerNameLabel.set_text(_stringBuilder.ToString());
			}
		}

		public virtual void SetStatsLabel(InGameStatId statId, uint amount)
		{
			switch (statId)
			{
			case InGameStatId.DestroyedCubesInProtection:
			case InGameStatId.DestroyedCubesDefendingTheBase:
			case InGameStatId.HealAssist:
			case InGameStatId.DestroyedProtoniumCubes:
			case InGameStatId.BaseCaptureClassicMode:
			case InGameStatId.HealthPercentageBonusClassicMode:
			case InGameStatId.BestKillStreak:
			case InGameStatId.CapturePointBattleArenaMode:
			case InGameStatId.EqualiserDestroyedBattleArenaMode:
				break;
			case InGameStatId.Kill:
				killsLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				break;
			case InGameStatId.RobotDestroyed:
				deathsLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				break;
			case InGameStatId.KillAssist:
				if (assistsLabel != null)
				{
					assistsLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				}
				break;
			case InGameStatId.DestroyedCubes:
				damageLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				break;
			case InGameStatId.HealCubes:
				if (healLabel != null)
				{
					healLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				}
				break;
			case InGameStatId.BattleArenaObjectives:
				if (objectivesLabel != null)
				{
					objectivesLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				}
				break;
			case InGameStatId.Score:
				if (scoreLabel != null)
				{
					scoreLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				}
				break;
			case InGameStatId.CurrentKillStreak:
				if (currentStreakLabel != null)
				{
					currentStreakLabel.set_text(PitUtils.TitleForStreak(amount));
				}
				break;
			case InGameStatId.Points:
				if (pointsLabel != null)
				{
					pointsLabel.set_text(amount.ToString("N0", CultureInfo.InvariantCulture));
				}
				break;
			}
		}

		public void SetFriendIcon(bool isMe, bool showAsFriend, bool showAsRequested, bool friendFunctionsEnabled, bool isSinglePlayer)
		{
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			friendIcon.SetActive(!isMe && !isSinglePlayer);
			UIButton[] componentsInChildren = friendIcon.GetComponentsInChildren<UIButton>(true);
			foreach (UIButton val in componentsInChildren)
			{
				val.set_isEnabled(friendFunctionsEnabled && !showAsFriend && !showAsRequested);
			}
			if (friendFunctionsEnabled)
			{
				friendIcon.GetComponent<UISprite>().set_color(showAsFriend ? isFriendBG : ((!showAsRequested) ? addFriendBG : requestBG));
			}
		}
	}
}
