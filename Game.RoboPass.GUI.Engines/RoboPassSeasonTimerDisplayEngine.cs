using Game.ECS.GUI.Components;
using Game.RoboPass.Components;
using Game.RoboPass.EntityViews;
using Game.RoboPass.GUI.EntityViews;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.Tasks;
using Svelto.Tasks.Enumerators;
using System;
using System.Collections;
using System.Text;
using UnityEngine;

namespace Game.RoboPass.GUI.Engines
{
	internal class RoboPassSeasonTimerDisplayEngine : SingleEntityViewEngine<RoboPassSeasonDataEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkDestruction, IEngine
	{
		private const float SEC_IN_MIN = 60f;

		private const int HOURS_IN_DAY = 24;

		private const int HOURS_REMAINING_THRESHOLD = 36;

		private const string STR_KEY_DAYS_LEFT = "{DAYS_UNITY}";

		private const string STR_KEY_DAYS_LEFT_QUANTITY = "{DAYS}";

		private const string STR_KEY_HOURS_LEFT = "{HOURS_UNITY}";

		private const string STR_KEY_HOURS_LEFT_QUANTITY = "{HOURS}";

		private const string STR_KEY_LEFT = "{LEFT}";

		private const string STR_KEY_MINS_LEFT = "{MINS_UNITY}";

		private const string STR_KEY_MINS_LEFT_QUANTITY = "{MINS}";

		private const string STR_LOC_DAY = "strDay";

		private const string STR_LOC_DAYS = "strDays";

		private const string STR_LOC_DAYS_LEFT = "strDaysLeft";

		private const string STR_LOC_HOUR = "strHour";

		private const string STR_LOC_HOURS = "strHours";

		private const string STR_LOC_HOURS_MINUTES_LEFT = "strHoursMinutesLeft";

		private const string STR_LOC_LEFT_PLURAL = "strLeftPlural";

		private const string STR_LOC_LEFT_SINGULAR = "strLeftSingular";

		private const string STR_LOC_MIN = "strMin";

		private const string STR_LOC_MINS = "strMins";

		private readonly ITaskRoutine _timerHoursMinutesRoutine;

		private readonly StringBuilder _stringBuilder;

		private readonly string _strLocalizedDay;

		private readonly string _strLocalizedDays;

		private readonly string _strLocalizedDaysLeft;

		private readonly string _strLocalizedHour;

		private readonly string _strLocalizedHours;

		private readonly string _strLocalizedHoursMinutesLeft;

		private readonly string _strLocalizedLeftPlural;

		private readonly string _strLocalizedLeftSingular;

		private readonly string _strLocalizedMinute;

		private readonly string _strLocalizedMinutes;

		private bool _taskRoutineIsRunning;

		private int _remainingMinutes;

		private int _remainingTotalHours;

		public IEntityViewsDB entityViewsDB
		{
			private get;
			set;
		}

		public RoboPassSeasonTimerDisplayEngine()
		{
			_stringBuilder = new StringBuilder();
			_strLocalizedDay = StringTableBase<StringTable>.Instance.GetString("strDay");
			_strLocalizedDays = StringTableBase<StringTable>.Instance.GetString("strDays");
			_strLocalizedDaysLeft = StringTableBase<StringTable>.Instance.GetString("strDaysLeft").ToUpper();
			_strLocalizedHour = StringTableBase<StringTable>.Instance.GetString("strHour").ToUpper();
			_strLocalizedHours = StringTableBase<StringTable>.Instance.GetString("strHours").ToUpper();
			_strLocalizedHoursMinutesLeft = StringTableBase<StringTable>.Instance.GetString("strHoursMinutesLeft").ToUpper();
			_strLocalizedLeftPlural = StringTableBase<StringTable>.Instance.GetString("strLeftPlural").ToUpper();
			_strLocalizedLeftSingular = StringTableBase<StringTable>.Instance.GetString("strLeftSingular").ToUpper();
			_strLocalizedMinute = StringTableBase<StringTable>.Instance.GetString("strMin").ToUpper();
			_strLocalizedMinutes = StringTableBase<StringTable>.Instance.GetString("strMins").ToUpper();
			_timerHoursMinutesRoutine = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)UpdateHoursMinutesTimer);
		}

		public void Ready()
		{
		}

		public void OnFrameworkDestroyed()
		{
			StopTimerTaskRoutine();
		}

		protected override void Add(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.dataUpdated.NotifyOnValueSet((Action<int, bool>)RefreshRoboPassTimerUi);
		}

		protected override void Remove(RoboPassSeasonDataEntityView entityView)
		{
			entityView.roboPassSeasonPlayerInfoComponent.dataUpdated.NotifyOnValueSet((Action<int, bool>)RefreshRoboPassTimerUi);
		}

		private void RefreshRoboPassTimerUi(int entityID, bool dataUpdated)
		{
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			if (!dataUpdated)
			{
				return;
			}
			RoboPassSeasonDataEntityView roboPassSeasonDataEntityView = entityViewsDB.QueryEntityView<RoboPassSeasonDataEntityView>(entityID);
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonInfoComponent;
			TimeSpan timeRemaining = roboPassSeasonInfoComponent.timeRemaining;
			int num = Mathf.RoundToInt((float)timeRemaining.TotalDays);
			_remainingTotalHours = timeRemaining.Days * 24 + timeRemaining.Hours;
			_remainingMinutes = timeRemaining.Minutes;
			RoboPassSeasonTimeUIEntityView roboPassSeasonTimeUIEntityView = entityViewsDB.QueryEntityViews<RoboPassSeasonTimeUIEntityView>().get_Item(0);
			ILabelUIComponent labelUIComponent = roboPassSeasonTimeUIEntityView.labelUIComponent;
			_stringBuilder.Length = 0;
			if (_remainingTotalHours > 36)
			{
				_stringBuilder.Append(_strLocalizedDaysLeft);
				if (num > 1)
				{
					_stringBuilder.Replace("{DAYS_UNITY}", _strLocalizedDays);
					_stringBuilder.Replace("{LEFT}", _strLocalizedLeftPlural);
				}
				else
				{
					_stringBuilder.Replace("{DAYS_UNITY}", _strLocalizedDay);
					_stringBuilder.Replace("{LEFT}", _strLocalizedLeftSingular);
				}
				_stringBuilder.Replace("{DAYS}", num.ToString());
				labelUIComponent.label = _stringBuilder.ToString();
				return;
			}
			bool flag = _remainingTotalHours <= 0 && _remainingMinutes <= 0;
			if (flag)
			{
				NotifySeasonEnded();
			}
			if (_taskRoutineIsRunning)
			{
				if (flag)
				{
					StopTimerTaskRoutine();
				}
			}
			else if (!flag)
			{
				_taskRoutineIsRunning = true;
				_timerHoursMinutesRoutine.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private IEnumerator UpdateHoursMinutesTimer()
		{
			while (_taskRoutineIsRunning)
			{
				_remainingMinutes--;
				if (_remainingMinutes < 0)
				{
					if (_remainingTotalHours > 0)
					{
						_remainingMinutes = 59;
						_remainingTotalHours--;
						if (_remainingTotalHours < 0)
						{
							_remainingTotalHours = 0;
						}
					}
					else
					{
						_remainingTotalHours = 0;
						_remainingMinutes = 0;
						_taskRoutineIsRunning = false;
					}
				}
				_stringBuilder.Length = 0;
				_stringBuilder.Append(_strLocalizedHoursMinutesLeft);
				_stringBuilder.Replace("{HOURS}", _remainingTotalHours.ToString());
				bool isPluralNoun2 = _remainingTotalHours > 1;
				_stringBuilder.Replace("{HOURS_UNITY}", (!isPluralNoun2) ? _strLocalizedHour : _strLocalizedHours);
				_stringBuilder.Replace("{MINS}", _remainingMinutes.ToString());
				isPluralNoun2 = (_remainingMinutes > 1);
				_stringBuilder.Replace("{MINS_UNITY}", (!isPluralNoun2) ? _strLocalizedMinute : _strLocalizedMinutes);
				_stringBuilder.Replace("{LEFT}", (!isPluralNoun2) ? _strLocalizedLeftSingular : _strLocalizedLeftPlural);
				FasterListEnumerator<RoboPassSeasonTimeUIEntityView> enumerator = entityViewsDB.QueryEntityViews<RoboPassSeasonTimeUIEntityView>().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						RoboPassSeasonTimeUIEntityView current = enumerator.get_Current();
						current.labelUIComponent.label = _stringBuilder.ToString();
					}
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				if (_remainingTotalHours <= 0 && _remainingMinutes <= 0)
				{
					NotifySeasonEnded();
				}
				yield return (object)new WaitForSecondsEnumerator(60f);
			}
		}

		private void NotifySeasonEnded()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			RoboPassSeasonDataEntityView roboPassSeasonDataEntityView = entityViewsDB.QueryEntityViews<RoboPassSeasonDataEntityView>().get_Item(0);
			IRoboPassSeasonInfoComponent roboPassSeasonInfoComponent = roboPassSeasonDataEntityView.roboPassSeasonInfoComponent;
			roboPassSeasonInfoComponent.isValidSeason = false;
			roboPassSeasonInfoComponent.timeRemaining = TimeSpan.Zero;
			roboPassSeasonInfoComponent.dataUpdated.set_value(true);
		}

		private void StopTimerTaskRoutine()
		{
			_timerHoursMinutesRoutine.Stop();
			_taskRoutineIsRunning = false;
		}
	}
}
