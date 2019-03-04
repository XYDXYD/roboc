using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ES.Legacy;
using Svelto.Factories;
using Svelto.IoC;
using System.Text;
using UnityEngine;

namespace Simulation
{
	internal class BattleEventStreamDisplay : MonoBehaviour, IInitialize, IPingMessageDisplayerComponent, IWaitForFrameworkDestruction, IHudElement, IComponent
	{
		protected struct BattleEventData
		{
			public string eventText;

			public float eventTime;

			public BattleEventData(string text, float time)
			{
				eventText = text;
				eventTime = time;
			}
		}

		public GameObject battleStreamWidget;

		public Vector3 startPos;

		public Vector3 offset;

		public float maxAlpha = 1f;

		public float minAlpha = 0.5f;

		[SerializeField]
		private string dangerColor = "CC00FF";

		[SerializeField]
		private string onMyWayColor = "00FF00";

		[SerializeField]
		private string goHereColor = "42C0FB";

		private UIAnchor _anchor;

		private float _lastTick;

		private FasterList<EventStreamWidget> eventStreamElements = new FasterList<EventStreamWidget>();

		protected FasterList<BattleEventData> battleEvents = new FasterList<BattleEventData>();

		private const uint MAX_BATTLE_STREAM_EVENTS = 4u;

		private const float BATTLE_EVENT_EXPIRE_TIME = 30f;

		private const float TICK_EVERY_X_SECONDS = 1f;

		protected const string localSelfColorHTML = "[28DC82FF]";

		protected const string sameTeamColorHTML = "[418CCAFF]";

		protected const string enemyColorHTML = "[FF3C3CFF]";

		private StringBuilder _sb = new StringBuilder();

		[Inject]
		internal IBattleEventStreamManager battleEventStreamManager
		{
			get;
			set;
		}

		[Inject]
		internal IGameObjectFactory gameObjectFactory
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerNamesContainer playerNamesContainer
		{
			get;
			set;
		}

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			get;
			set;
		}

		[Inject]
		internal LobbyGameStartPresenter lobbyPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IMinimapPresenter minimapPresenter
		{
			private get;
			set;
		}

		[Inject]
		internal IHudStyleController battleHudStyleController
		{
			private get;
			set;
		}

		public BattleEventStreamDisplay()
			: this()
		{
		}

		private void Awake()
		{
			_anchor = this.GetComponent<UIAnchor>();
		}

		void IInitialize.OnDependenciesInjected()
		{
			minimapPresenter.OnMinimapZoom += HandleOnMinimapZoom;
		}

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			DeregisterListeners();
			battleHudStyleController.RemoveHud(this);
		}

		private void Start()
		{
			EventStreamWidget component = battleStreamWidget.GetComponent<EventStreamWidget>();
			_lastTick = Time.get_time();
			RegisterListeners();
			InitElements();
			battleHudStyleController.AddHud(this);
		}

		protected virtual void RegisterListeners()
		{
			battleEventStreamManager.OnPlayerSpawnedIn += OnPlayerSpawnedIn;
			battleEventStreamManager.OnPlayerSpawnedOut += OnPlayerSpawnedOut;
			battleEventStreamManager.OnPlayerWasKilledBy += OnPlayerWasKilledBy;
		}

		protected virtual void DeregisterListeners()
		{
			battleEventStreamManager.OnPlayerSpawnedIn -= OnPlayerSpawnedIn;
			battleEventStreamManager.OnPlayerSpawnedOut -= OnPlayerSpawnedOut;
			battleEventStreamManager.OnPlayerWasKilledBy -= OnPlayerWasKilledBy;
		}

		private void HandleOnMinimapZoom(float pixelOffset)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			_anchor.pixelOffset = new Vector2(0f, pixelOffset);
		}

		private void InitElements()
		{
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; (long)i < 4L; i++)
			{
				GameObject val = gameObjectFactory.Build(battleStreamWidget);
				val.get_transform().set_parent(this.get_transform());
				val.get_transform().set_localScale(Vector3.get_one());
				val.get_transform().set_localPosition(startPos + (float)i * offset);
				val.SetActive(true);
				EventStreamWidget component = val.GetComponent<EventStreamWidget>();
				component.SetAlpha(0f);
				eventStreamElements.Add(component);
			}
		}

		private void OnPlayerSpawnedIn(int player)
		{
			_sb.Length = 0;
			_sb.Append(StringTableBase<StringTable>.Instance.GetString("strTeleportedIn"));
			if (playerTeamsContainer.IsMe(TargetType.Player, player))
			{
				_sb.Replace("{PlayerColour}", "[28DC82FF]");
			}
			else
			{
				_sb.Replace("{PlayerColour}", "[FF3C3CFF]");
			}
			_sb.Replace("{PlayerName}", playerNamesContainer.GetDisplayName(player));
			BattleEventData battleEventData = new BattleEventData(_sb.ToString(), Time.get_time());
			battleEvents.Add(battleEventData);
			UpdateBattleStreamDisplay();
		}

		private void OnPlayerSpawnedOut(int playerId)
		{
			_sb.Length = 0;
			_sb.Append(StringTableBase<StringTable>.Instance.GetString("strReturnedToMothership"));
			if (playerTeamsContainer.IsMe(TargetType.Player, playerId))
			{
				_sb.Replace("{PlayerColour}", "[28DC82FF]");
			}
			else
			{
				_sb.Replace("{PlayerColour}", "[FF3C3CFF]");
			}
			_sb.Replace("{PlayerName}", playerNamesContainer.GetDisplayName(playerId));
			BattleEventData battleEventData = new BattleEventData(_sb.ToString(), Time.get_time());
			battleEvents.Add(battleEventData);
			UpdateBattleStreamDisplay();
		}

		private void OnPlayerWasKilledBy(int player, int shooter)
		{
			_sb.Length = 0;
			_sb.Append(StringTableBase<StringTable>.Instance.GetString("strPlayerDestroyed"));
			bool flag = player == shooter;
			if (!flag && playerTeamsContainer.IsMe(TargetType.Player, shooter))
			{
				_sb.Replace("{ShooterColour}", "[28DC82FF]");
			}
			else if (!flag && playerTeamsContainer.IsOnMyTeam(TargetType.Player, shooter))
			{
				_sb.Replace("{ShooterColour}", "[418CCAFF]");
			}
			else
			{
				_sb.Replace("{ShooterColour}", "[FF3C3CFF]");
			}
			if (flag)
			{
				_sb.Replace("{ShooterName}", StringTableBase<StringTable>.Instance.GetString("strEnemyBase"));
			}
			else
			{
				_sb.Replace("{ShooterName}", playerNamesContainer.GetDisplayName(shooter));
			}
			if (playerTeamsContainer.IsMe(TargetType.Player, player))
			{
				_sb.Replace("{PlayerColour}", "[28DC82FF]");
			}
			else if (playerTeamsContainer.IsOnMyTeam(TargetType.Player, player))
			{
				_sb.Replace("{PlayerColour}", "[418CCAFF]");
			}
			else
			{
				_sb.Replace("{PlayerColour}", "[FF3C3CFF]");
			}
			_sb.Replace("{PlayerName}", playerNamesContainer.GetDisplayName(player));
			BattleEventData battleEventData = new BattleEventData(_sb.ToString(), Time.get_time());
			battleEvents.Add(battleEventData);
			UpdateBattleStreamDisplay();
		}

		protected void UpdateBattleStreamDisplay()
		{
			if (!lobbyPresenter.hasBeenClosed)
			{
				return;
			}
			int num = battleEvents.get_Count() - 1;
			float num2 = (maxAlpha - minAlpha) / (float)battleEvents.get_Count();
			float num3 = maxAlpha;
			for (int i = 0; i < eventStreamElements.get_Count(); i++)
			{
				EventStreamWidget eventStreamWidget = eventStreamElements.get_Item(i);
				if (num >= 0)
				{
					EventStreamWidget eventStreamWidget2 = eventStreamWidget;
					BattleEventData battleEventData = battleEvents.get_Item(num);
					eventStreamWidget2.SetLabelText(battleEventData.eventText);
					eventStreamWidget.SetAlpha(num3);
				}
				else
				{
					eventStreamWidget.SetLabelText(" ");
					eventStreamWidget.SetAlpha(0f);
				}
				num--;
				num3 -= num2;
			}
		}

		private void Update()
		{
			float time = Time.get_time();
			if (!(time > _lastTick + 1f))
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < battleEvents.get_Count(); i++)
			{
				float num = time;
				BattleEventData battleEventData = battleEvents.get_Item(i);
				if (num > battleEventData.eventTime + 30f)
				{
					battleEvents.RemoveAt(i);
					i--;
					flag = true;
				}
			}
			if (flag)
			{
				UpdateBattleStreamDisplay();
			}
			_lastTick = time;
		}

		public void ShowPingMessage(string user, PingType type)
		{
			_sb.Length = 0;
			_sb.AppendFormat("[FFFFFF]{0}:", user);
			switch (type)
			{
			case PingType.DANGER:
				_sb.AppendFormat("[{0}]{1}", dangerColor, StringTableBase<StringTable>.Instance.GetString("strDangerHere"));
				break;
			case PingType.GOING_HERE:
				_sb.AppendFormat("[{0}]{1}", onMyWayColor, StringTableBase<StringTable>.Instance.GetString("strOnMyWay"));
				break;
			case PingType.MOVE_HERE:
				_sb.AppendFormat("[{0}]{1}", goHereColor, StringTableBase<StringTable>.Instance.GetString("strGoHere"));
				break;
			}
			BattleEventData battleEventData = new BattleEventData(_sb.ToString(), Time.get_time());
			battleEvents.Add(battleEventData);
			UpdateBattleStreamDisplay();
		}

		public void SetStyle(HudStyle style)
		{
			switch (style)
			{
			case HudStyle.HideAllButChat:
				this.get_gameObject().SetActive(false);
				break;
			case HudStyle.HideAll:
				this.get_gameObject().SetActive(false);
				break;
			case HudStyle.Full:
				this.get_gameObject().SetActive(true);
				break;
			}
		}
	}
}
