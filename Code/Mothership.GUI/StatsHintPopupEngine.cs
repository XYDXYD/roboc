using Mothership.TechTree;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ServiceLayer;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Mothership.GUI
{
	internal sealed class StatsHintPopupEngine : MultiEntityViewsEngine<TechTreeItemDispatchableEntityView, StatsHintPopupAreaEntityView>, IQueryingEntityViewEngine, IWaitForFrameworkInitialization, IEngine
	{
		private ICubeInventoryData _cubeInventoryData;

		private ICubeList _cubeList;

		private IServiceRequestFactory _requestFactory;

		private IDictionary<int, MovementStatsData> _movementStats;

		private ITaskRoutine _tick;

		private readonly StringBuilder _stringBuilder;

		private readonly StringBuilder _replaceSB;

		private const string LOC_KEY_BASE_SPEED = "strBaseSpeedDS";

		private const string LOC_KEY_CPU_LOAD = "strInventoryDetailCpu";

		private const string LOC_KEY_HEALTH = "strInventoryDetailHealth";

		private const string LOC_KEY_HEALTH_BOOST = "strInventoryDetailBoostHealth";

		private const string LOC_KEY_KG = "strKilograms";

		private const string LOC_KEY_MASS = "strInventoryDetailMass";

		private const string LOC_KEY_MPH = "strMilesPerHour";

		private const string LOC_KEY_ROBOT_RANKING = "strRobotRanking";

		private const string LOC_KEY_PFLOP = "strPFlop";

		private const string LOC_KEY_PFLOPS = "strPFlops";

		private const string F1_STRING_FORMAT = "F1";

		private const string PERCENT_SYMBOL = "%";

		private const string SUFFIX_FORMAT = "[{0}]";

		private const string SUFFIX_STAT_REGEX = "\\[([^]]*)\\]";

		public IEntityViewsDB entityViewsDB
		{
			get;
			set;
		}

		public StatsHintPopupEngine(ICubeInventoryData cubeInventoryData, ICubeList cubeList, IServiceRequestFactory requestFactory, IGUIInputController guiInputController)
		{
			_cubeInventoryData = cubeInventoryData;
			_cubeList = cubeList;
			_requestFactory = requestFactory;
			_replaceSB = new StringBuilder();
			_stringBuilder = new StringBuilder();
			guiInputController.OnScreenStateChange += OnScreenStateChange;
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine().SetEnumeratorProvider((Func<IEnumerator>)Tick);
		}

		private void OnScreenStateChange()
		{
			StatsHintPopupEntityView popup = GetPopup();
			if (popup != null)
			{
				HidePopup();
			}
		}

		public void Ready()
		{
		}

		public void OnFrameworkInitialized()
		{
			TaskRunner.get_Instance().Run(LoadMovementStats());
		}

		private IEnumerator LoadMovementStats()
		{
			ILoadMovementStatsRequest request = _requestFactory.Create<ILoadMovementStatsRequest>();
			TaskService<MovementStats> task = new TaskService<MovementStats>(request);
			yield return task;
			if (task.succeeded)
			{
				_movementStats = task.result.data;
			}
			else
			{
				RemoteLogger.Error(new Exception("Unable to load movement stats for tooltips"));
			}
		}

		protected override void Add(TechTreeItemDispatchableEntityView entityView)
		{
			entityView.dispatcherComponent.IsHover.NotifyOnValueSet((Action<int, bool>)OnItemHover);
		}

		protected override void Remove(TechTreeItemDispatchableEntityView entityView)
		{
			entityView.dispatcherComponent.IsHover.StopNotify((Action<int, bool>)OnItemHover);
		}

		protected override void Add(StatsHintPopupAreaEntityView entityView)
		{
			entityView.mouseOverComponent.isMouseOver.NotifyOnValueSet((Action<int, bool>)OnItemHover);
		}

		protected override void Remove(StatsHintPopupAreaEntityView entityView)
		{
			entityView.mouseOverComponent.isMouseOver.StopNotify((Action<int, bool>)OnItemHover);
		}

		private void OnItemHover(int entityId, bool isMouseOver)
		{
			if (isMouseOver)
			{
				ShowPopup(entityId);
			}
			else
			{
				HidePopup();
			}
		}

		private void ShowPopup(int hoveredEntityId)
		{
			//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
			StatsHintPopupEntityView popup = GetPopup();
			StatsHintPopupAreaEntityView statsHintPopupAreaEntityView = default(StatsHintPopupAreaEntityView);
			CubeTypeID index;
			if (entityViewsDB.TryQueryEntityView<StatsHintPopupAreaEntityView>(hoveredEntityId, ref statsHintPopupAreaEntityView))
			{
				index = statsHintPopupAreaEntityView.cubeTypeIDComponent.cubeTypeId;
			}
			else
			{
				TechTreeItemInfoEntityView techTreeItemInfoEntityView = default(TechTreeItemInfoEntityView);
				if (!entityViewsDB.TryQueryEntityView<TechTreeItemInfoEntityView>(hoveredEntityId, ref techTreeItemInfoEntityView))
				{
					throw new Exception("Entity view not found for cube stats popup engine");
				}
				index = techTreeItemInfoEntityView.iDsComponent.CubeID;
			}
			CubeTypeData cubeTypeData = _cubeList.CubeTypeDataOf(index);
			PersistentCubeData cubeData = cubeTypeData.cubeData;
			popup.statsComponent.title = StringTableBase<StringTable>.Instance.GetString(cubeTypeData.nameStrKey);
			popup.statsComponent.description = StringTableBase<StringTable>.Instance.GetString(cubeData.descriptionStrKey);
			popup.statsComponent.cpuAndRRLines = (IList<ItemStat>)GetCPUAndRRStats(cubeTypeData);
			popup.statsComponent.statLines = (IList<ItemStat>)GetAllItemStats(cubeTypeData);
			popup.popupComponent.fadeIn = true;
			popup.popupComponent.screenPosition = Input.get_mousePosition();
			_tick.Start((Action<PausableTaskException>)null, (Action)null);
		}

		private void HidePopup()
		{
			StatsHintPopupEntityView popup = GetPopup();
			popup.popupComponent.fadeIn = false;
			_tick.Pause();
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				StatsHintPopupEntityView popup = GetPopup();
				popup.popupComponent.screenPosition = Input.get_mousePosition();
				yield return null;
			}
		}

		private FasterList<ItemStat> GetCPUAndRRStats(CubeTypeData cubeTypeData)
		{
			PersistentCubeData cubeData = cubeTypeData.cubeData;
			FasterList<ItemStat> val = new FasterList<ItemStat>();
			val.Add(new ItemStat
			{
				name = StringTableBase<StringTable>.Instance.GetString("strInventoryDetailCpu"),
				value = cubeData.cpuRating.ToString(),
				suffix = ((cubeData.cpuRating != 1) ? StringTableBase<StringTable>.Instance.GetString("strPFlop") : StringTableBase<StringTable>.Instance.GetString("strPFlops"))
			});
			val.Add(new ItemStat
			{
				name = StringTableBase<StringTable>.Instance.GetString("strRobotRanking"),
				value = GameUtility.CommaSeparate(cubeData.cubeRanking)
			});
			return val;
		}

		private FasterList<ItemStat> GetAllItemStats(CubeTypeData cubeTypeData)
		{
			FasterList<ItemStat> val = new FasterList<ItemStat>();
			PersistentCubeData cubeData = cubeTypeData.cubeData;
			val.Add(new ItemStat
			{
				name = StringTableBase<StringTable>.Instance.GetString("strInventoryDetailHealth"),
				value = GameUtility.CommaSeparate(cubeData.health)
			});
			if (cubeData.healthBoost > 0f)
			{
				val.Add(new ItemStat
				{
					name = StringTableBase<StringTable>.Instance.GetString("strInventoryDetailBoostHealth").ToUpperInvariant(),
					value = (cubeData.healthBoost * 100f).ToString() + "%"
				});
			}
			val.Add(new ItemStat
			{
				name = StringTableBase<StringTable>.Instance.GetString("strInventoryDetailMass"),
				value = cubeTypeData.prefab.GetComponent<CubeInstance>().DisplayedMass.ToString("F1"),
				suffix = StringTableBase<StringTable>.Instance.GetString("strKilograms")
			});
			if (cubeData.itemType == ItemType.Movement)
			{
				ItemDescriptor itemDescriptor = cubeData.itemDescriptor;
				MovementStatsData value;
				if (_movementStats != null && _movementStats.TryGetValue(itemDescriptor.GenerateKey(), out value) && value != null)
				{
					val.Add(new ItemStat
					{
						name = StringTableBase<StringTable>.Instance.GetString("strBaseSpeedDS").ToUpperInvariant(),
						value = value.horizontalTopSpeed.ToString(),
						suffix = StringTableBase<StringTable>.Instance.GetString("strMilesPerHour")
					});
				}
			}
			if (cubeData.displayStats == null)
			{
				return val;
			}
			foreach (KeyValuePair<string, object> displayStat in cubeData.displayStats)
			{
				val.Add(new ItemStat
				{
					name = StringTableBase<StringTable>.Instance.GetString(displayStat.Key),
					value = GetDisplayStatsValueSuffix(displayStat.Value.ToString())
				});
			}
			return val;
		}

		private string GetDisplayStatsValueSuffix(string value)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.Append(value);
			Match match = Regex.Match(value, "\\[([^]]*)\\]");
			while (match.Success)
			{
				string value2 = match.Groups[1].Value;
				_replaceSB.Length = 0;
				_replaceSB.AppendFormat("[{0}]", value2);
				_stringBuilder.Replace(_replaceSB.ToString(), StringTableBase<StringTable>.Instance.GetString(value2));
				match = match.NextMatch();
			}
			return _stringBuilder.ToString();
		}

		private StatsHintPopupEntityView GetPopup()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			return entityViewsDB.QueryEntityViews<StatsHintPopupEntityView>().get_Item(0);
		}
	}
}
