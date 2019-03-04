using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Mothership
{
	internal sealed class RobotShopCommunityItemView : MonoBehaviour
	{
		public UIWidget submissionTimeRemainingPanel;

		public UILabel submissionTimeRemainingLabel;

		public UITexture thumbnail;

		public UILabel robotNameLabel;

		public UILabel sunkCostLabel;

		public UILabel tierLabel;

		public RobotShopRating Rating;

		public UILabel SortByLabel;

		public UILabel SortByValue;

		public UITable ratingTable;

		public GameObject FeaturedBackground;

		public GameObject ForgeableBackground;

		public GameObject MegabotStrip;

		public Color forgeColour;

		public Color getColour;

		public Color featuredColour;

		public UIButtonColor uiButtonColorForRobotNameLabel;

		[SerializeField]
		private UIButtonBroadcasterInt[] buttonBroadcasters;

		[SerializeField]
		private GameObject lockedStateGO;

		private ITaskRoutine _updateRemainingTimeTask;

		private bool _taskRunning;

		private Action _onRobotExpired;

		internal CRFItem crfItemData
		{
			get;
			private set;
		}

		public RobotShopCommunityItemView()
			: this()
		{
		}

		internal void SetItemData(CRFItem itemData, ItemSortMode sortMode, Transform listener, int index, Action onRobotExpired, bool inventoryShowsLockStateInsteadOfCount)
		{
			//IL_01a2: Unknown result type (might be due to invalid IL or missing references)
			//IL_01a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
			//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
			//IL_01dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
			//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
			//IL_020b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0210: Unknown result type (might be due to invalid IL or missing references)
			//IL_0217: Unknown result type (might be due to invalid IL or missing references)
			//IL_021d: Unknown result type (might be due to invalid IL or missing references)
			_onRobotExpired = onRobotExpired;
			crfItemData = itemData;
			for (int i = 0; i < buttonBroadcasters.Length; i++)
			{
				buttonBroadcasters[i].sendValue = index;
			}
			MegabotStrip.SetActive(!inventoryShowsLockStateInsteadOfCount && itemData.isMegabot);
			submissionTimeRemainingPanel.get_gameObject().SetActive(itemData.isMyRobot && !itemData.robotShopItem.featured);
			if (itemData.isMyRobot && !itemData.robotShopItem.featured)
			{
				if (_updateRemainingTimeTask == null)
				{
					_updateRemainingTimeTask = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)UpdateRemainingTime);
					_updateRemainingTimeTask.Start((Action<PausableTaskException>)null, (Action)null);
					_taskRunning = true;
				}
				if (!_taskRunning)
				{
					_updateRemainingTimeTask.Resume();
				}
			}
			else if (_updateRemainingTimeTask != null)
			{
				_updateRemainingTimeTask.Pause();
				_taskRunning = false;
			}
			FeaturedBackground.SetActive(itemData.robotShopItem.featured);
			ForgeableBackground.SetActive((!itemData.robotShopItem.featured && itemData.playerOwnAllCubes) || itemData.isMyRobot);
			robotNameLabel.set_text(crfItemData.robotShopItem.name);
			tierLabel.set_text(crfItemData.TierStr);
			if (itemData.robotShopItem.featured)
			{
				UILabel obj = robotNameLabel;
				Color val = featuredColour;
				uiButtonColorForRobotNameLabel.set_defaultColor(val);
				obj.set_color(val);
			}
			else if (itemData.playerOwnAllCubes || itemData.isMyRobot)
			{
				UILabel obj2 = robotNameLabel;
				Color val = forgeColour;
				uiButtonColorForRobotNameLabel.set_defaultColor(val);
				obj2.set_color(val);
			}
			else if (itemData.playerOwnAllCubes)
			{
				UILabel obj3 = robotNameLabel;
				Color val = getColour;
				uiButtonColorForRobotNameLabel.set_defaultColor(val);
				obj3.set_color(val);
			}
			Rating.get_gameObject().SetActive(false);
			SortByValue.get_gameObject().SetActive(false);
			sunkCostLabel.get_gameObject().SetActive(false);
			switch (sortMode)
			{
			case ItemSortMode.ADDED:
			{
				TimeSpan t = DateTime.UtcNow - itemData.robotShopItem.addedDate;
				if (t < TimeSpan.Zero)
				{
					t = TimeSpan.Zero;
				}
				int days = t.Days;
				if (365 < days)
				{
					SortByLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strYearsAgo"), (days / 365).ToString()));
					break;
				}
				if (30 < days)
				{
					int num = days / 30;
					if (num == 1)
					{
						SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("str1MonthAgo"));
					}
					else
					{
						SortByLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strMonthsAgo"), num.ToString()));
					}
					break;
				}
				if (1 < days)
				{
					SortByLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strDaysAgo"), days.ToString()));
					break;
				}
				if (days == 1)
				{
					SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("str1DayAgo"));
					break;
				}
				int hours = t.Hours;
				if (1 < hours)
				{
					SortByLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strHoursAgo"), hours.ToString()));
					break;
				}
				if (hours == 1)
				{
					SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("str1HourAgo"));
					break;
				}
				int minutes = t.Minutes;
				if (1 < minutes)
				{
					SortByLabel.set_text(string.Format(StringTableBase<StringTable>.Instance.GetString("strMinutesAgo"), minutes.ToString()));
				}
				else
				{
					SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("str1MinuteAgo"));
				}
				break;
			}
			case ItemSortMode.COMBAT_RATING:
				Rating.get_gameObject().SetActive(true);
				Rating.Set(itemData.robotShopItem.combatRating, setColours: false);
				SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strSortByCombat"));
				SortByValue.get_gameObject().SetActive(false);
				break;
			case ItemSortMode.COSMETIC_RATING:
				Rating.get_gameObject().SetActive(true);
				Rating.Set(itemData.robotShopItem.styleRating, setColours: false);
				SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strSortByStyle"));
				SortByValue.get_gameObject().SetActive(false);
				break;
			case ItemSortMode.CPU:
				SortByValue.get_gameObject().SetActive(true);
				SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strSortByCpu"));
				SortByValue.get_gameObject().SetActive(true);
				SortByValue.set_text(GameUtility.GetRobotRatingString((float)(double)itemData.robotCPUToPlayer));
				break;
			case ItemSortMode.SUGGESTED:
			case ItemSortMode.MOST_BOUGHT:
				SortByValue.get_gameObject().SetActive(true);
				SortByLabel.set_text(StringTableBase<StringTable>.Instance.GetString("strSortByMostBought"));
				SortByValue.set_text(itemData.robotShopItem.rentCount.ToString());
				break;
			}
			lockedStateGO.SetActive(itemData.LockedCubes.Count > 0);
			ratingTable.Reposition();
		}

		private IEnumerator UpdateRemainingTime()
		{
			while (true)
			{
				if (this.get_gameObject().get_activeInHierarchy())
				{
					DateTime submissionExpiryDate = crfItemData.robotShopItem.submissionExpiryDate;
					TimeSpan duration = submissionExpiryDate - DateTime.UtcNow;
					if (duration.TotalMilliseconds > 0.0)
					{
						UILabel val = submissionTimeRemainingLabel;
						val.set_text(FormatRemainingTime(duration));
					}
					else if (_onRobotExpired != null)
					{
						_onRobotExpired();
						_onRobotExpired = null;
					}
				}
				yield return (object)new WaitForSecondsEnumerator(1f);
			}
		}

		internal static string FormatRemainingTime(TimeSpan duration)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (duration < TimeSpan.Zero)
			{
				duration = TimeSpan.Zero;
			}
			if (duration.Days > 1)
			{
				stringBuilder.AppendFormat("{0:D2}:", duration.Days);
			}
			stringBuilder.AppendFormat("{0:D2}:{1:D2}:{2:D2}", duration.Hours, duration.Minutes, duration.Seconds);
			return stringBuilder.ToString();
		}

		internal void DisplayTexture(Texture2D texture)
		{
			thumbnail.set_mainTexture(texture);
		}
	}
}
