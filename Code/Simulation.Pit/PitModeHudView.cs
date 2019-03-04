using Svelto.IoC;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Simulation.Pit
{
	internal class PitModeHudView : MonoBehaviour
	{
		private struct Notification
		{
			public Color tint;

			public string text;

			public uint streakValue;
		}

		public List<PitHudPlayerStats> playerStats;

		public PitHudPlayerStats localPlayerStats;

		public GameObject localPlayerCrown;

		public UILabel streakTitle;

		public GameObject notifierContainer;

		private Animation notifierAnimation;

		public UISprite positionBacking1;

		public UISprite positionBacking2;

		public UISprite positionBacking3;

		public Color currentPlayerEffectColour;

		public Color enemyPlayerEffectColour;

		public Color localPlayerBacking;

		public Color enemyPlayerBacking;

		public GameObject centerAnchor;

		public GameObject trAnchor;

		private Queue<Notification> _notificationQueue = new Queue<Notification>();

		private bool _isShowingNotification;

		private StringBuilder _stringBuilder = new StringBuilder();

		[Inject]
		internal PitModeHudPresenter pitModHudPresenter
		{
			private get;
			set;
		}

		public PitModeHudView()
			: this()
		{
		}

		private void Start()
		{
			if (pitModHudPresenter != null)
			{
				pitModHudPresenter.RegisterView(this);
				notifierAnimation = notifierContainer.GetComponent<Animation>();
				this.GetComponent<Animation>().set_wrapMode(1);
				ClearStats();
				centerAnchor.SetActive(false);
				trAnchor.SetActive(false);
				streakTitle.set_text(string.Empty);
			}
		}

		internal void DisplayLeader(PitStatData firstPlayer, bool isLocalPlayer)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			SetPitStats(1, firstPlayer, playerStats[0]);
			positionBacking1.set_color((!isLocalPlayer) ? enemyPlayerBacking : localPlayerBacking);
		}

		internal void DisplaySideLeaderboardSlot1(PitStatData secondPlayer, bool isLocalPlayer)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			SetPitStats(2, secondPlayer, playerStats[1]);
			positionBacking2.set_color((!isLocalPlayer) ? enemyPlayerBacking : localPlayerBacking);
		}

		internal void DisplaySideLeaderboardSlot2(PitStatData thirdPlayer, bool isLocalPlayer)
		{
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			SetPitStats(3, thirdPlayer, playerStats[2]);
			positionBacking3.set_color((!isLocalPlayer) ? enemyPlayerBacking : localPlayerBacking);
		}

		internal void ResetSideLeaderboardSlot1()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			positionBacking2.set_color(enemyPlayerBacking);
		}

		internal void ResetSideLeaderboardSlot2()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			positionBacking3.set_color(enemyPlayerBacking);
		}

		internal void DisplayOwnStats(bool isLeader, int position, PitStatData thirdPlayer)
		{
			localPlayerCrown.SetActive(isLeader);
			SetPitStats(position, thirdPlayer, localPlayerStats);
		}

		internal void ShowHud()
		{
			centerAnchor.SetActive(true);
			trAnchor.SetActive(true);
		}

		internal void ShowStreakAnim(uint streak)
		{
			if (streak > 1)
			{
				ShowStreakTextAnim(streak);
			}
		}

		internal void ShowStreakTextAnim(uint streak, bool isOwnPlayer = true, string playerName = null)
		{
			_stringBuilder.Length = 0;
			if (playerName == null)
			{
				playerName = StringTableBase<StringTable>.Instance.GetString("strYou");
			}
			if (isOwnPlayer)
			{
				_stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString("strYouAre"));
			}
			else
			{
				_stringBuilder.AppendFormat("{0} {1}", playerName.ToUpper(), StringTableBase<StringTable>.Instance.GetString("strIs"));
			}
			_stringBuilder.AppendFormat(" {0}!", PitUtils.TitleForStreak(streak).ToUpper());
			ShowNotification(_stringBuilder.ToString(), streak, isOwnPlayer);
		}

		internal void ShowNotification(string text, uint streak, bool isOwnPlayer = true)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			Notification item = default(Notification);
			item.tint = ((!isOwnPlayer) ? enemyPlayerEffectColour : currentPlayerEffectColour);
			item.text = text;
			item.streakValue = streak;
			_notificationQueue.Enqueue(item);
			if (!_isShowingNotification)
			{
				_isShowingNotification = true;
				TaskRunner.get_Instance().Run((Func<IEnumerator>)ShowNotificationQueue);
			}
		}

		private IEnumerator ShowNotificationQueue()
		{
			if (!notifierAnimation.get_isPlaying() && _notificationQueue.Count > 0)
			{
				Notification notification = _notificationQueue.Dequeue();
				streakTitle.set_text(notification.text);
				streakTitle.set_color(notification.tint);
				notifierAnimation.Play("HUD_PIT_NOTIFIER");
			}
			if (notifierAnimation.get_isPlaying())
			{
				yield return (object)new WaitForSecondsEnumerator(notifierAnimation.GetComponent<Animation>().get_clip().get_length());
				TaskRunner.get_Instance().Run((Func<IEnumerator>)ShowNotificationQueue);
			}
			else
			{
				_isShowingNotification = false;
				yield return null;
			}
		}

		internal void StreakLost()
		{
		}

		private void SetPitStats(int position, PitStatData data, PitHudPlayerStats hudStats)
		{
			hudStats.Position.set_text(position.ToString());
			hudStats.Streak.set_text(PitUtils.TitleForStreak(data.Streak));
			hudStats.PlayerName.set_text(data.Name);
			hudStats.Score.set_text(data.Score.ToString());
			hudStats.ptsLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strPointsShort"));
		}

		internal void ClearStats()
		{
			foreach (PitHudPlayerStats playerStat in playerStats)
			{
				playerStat.Position.set_text(string.Empty);
				playerStat.Streak.set_text(string.Empty);
				playerStat.PlayerName.set_text(string.Empty);
				playerStat.Score.set_text(string.Empty);
				playerStat.ptsLabel.set_text(string.Empty);
			}
		}

		internal void ShowStreakIcon(uint streak, Color colour)
		{
		}
	}
}
