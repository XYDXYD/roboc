using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.Demos
{
	[AddComponentMenu("")]
	public class ControlRemappingDemo1 : MonoBehaviour
	{
		private class ControllerSelection
		{
			private int _id;

			private int _idPrev;

			private ControllerType _type;

			private ControllerType _typePrev;

			public int id
			{
				get
				{
					return _id;
				}
				set
				{
					_idPrev = _id;
					_id = value;
				}
			}

			public ControllerType type
			{
				get
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					return _type;
				}
				set
				{
					//IL_0002: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Unknown result type (might be due to invalid IL or missing references)
					//IL_000d: Unknown result type (might be due to invalid IL or missing references)
					//IL_000e: Unknown result type (might be due to invalid IL or missing references)
					_typePrev = _type;
					_type = value;
				}
			}

			public int idPrev => _idPrev;

			public ControllerType typePrev => _typePrev;

			public bool hasSelection => _id >= 0;

			public ControllerSelection()
			{
				Clear();
			}

			public void Set(int id, ControllerType type)
			{
				//IL_0008: Unknown result type (might be due to invalid IL or missing references)
				this.id = id;
				this.type = type;
			}

			public void Clear()
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				_id = -1;
				_idPrev = -1;
				_type = 2;
				_typePrev = 2;
			}
		}

		private class DialogHelper
		{
			public enum DialogType
			{
				None = 0,
				JoystickConflict = 1,
				ElementConflict = 2,
				KeyConflict = 3,
				DeleteAssignmentConfirmation = 10,
				AssignElement = 11
			}

			private const float openBusyDelay = 0.25f;

			private const float closeBusyDelay = 0.1f;

			private DialogType _type;

			private bool _enabled;

			private float _busyTime;

			private bool _busyTimerRunning;

			private Action<int> drawWindowDelegate;

			private WindowFunction drawWindowFunction;

			private WindowProperties windowProperties;

			private int currentActionId;

			private Action<int, UserResponse> resultCallback;

			private float busyTimer
			{
				get
				{
					if (!_busyTimerRunning)
					{
						return 0f;
					}
					return _busyTime - Time.get_realtimeSinceStartup();
				}
			}

			public bool enabled
			{
				get
				{
					return _enabled;
				}
				set
				{
					if (value)
					{
						if (_type != 0)
						{
							StateChanged(0.25f);
						}
					}
					else
					{
						_enabled = value;
						_type = DialogType.None;
						StateChanged(0.1f);
					}
				}
			}

			public DialogType type
			{
				get
				{
					if (!_enabled)
					{
						return DialogType.None;
					}
					return _type;
				}
				set
				{
					if (value == DialogType.None)
					{
						_enabled = false;
						StateChanged(0.1f);
					}
					else
					{
						_enabled = true;
						StateChanged(0.25f);
					}
					_type = value;
				}
			}

			public bool busy => _busyTimerRunning;

			public unsafe DialogHelper()
			{
				//IL_0025: Unknown result type (might be due to invalid IL or missing references)
				//IL_002f: Expected O, but got Unknown
				drawWindowDelegate = DrawWindow;
				drawWindowFunction = new WindowFunction((object)drawWindowDelegate, (IntPtr)(void*)/*OpCode not supported: LdFtn*/);
			}

			public void StartModal(int queueActionId, DialogType type, WindowProperties windowProperties, Action<int, UserResponse> resultCallback)
			{
				StartModal(queueActionId, type, windowProperties, resultCallback, -1f);
			}

			public void StartModal(int queueActionId, DialogType type, WindowProperties windowProperties, Action<int, UserResponse> resultCallback, float openBusyDelay)
			{
				currentActionId = queueActionId;
				this.windowProperties = windowProperties;
				this.type = type;
				this.resultCallback = resultCallback;
				if (openBusyDelay >= 0f)
				{
					StateChanged(openBusyDelay);
				}
			}

			public void Update()
			{
				Draw();
				UpdateTimers();
			}

			public void Draw()
			{
				//IL_0029: Unknown result type (might be due to invalid IL or missing references)
				//IL_0045: Unknown result type (might be due to invalid IL or missing references)
				if (_enabled)
				{
					bool enabled = GUI.get_enabled();
					GUI.set_enabled(true);
					GUILayout.Window(windowProperties.windowId, windowProperties.rect, drawWindowFunction, windowProperties.title, (GUILayoutOption[])new GUILayoutOption[0]);
					GUI.FocusWindow(windowProperties.windowId);
					if (GUI.get_enabled() != enabled)
					{
						GUI.set_enabled(enabled);
					}
				}
			}

			public void DrawConfirmButton()
			{
				DrawConfirmButton("Confirm");
			}

			public void DrawConfirmButton(string title)
			{
				bool enabled = GUI.get_enabled();
				if (busy)
				{
					GUI.set_enabled(false);
				}
				if (GUILayout.Button(title, (GUILayoutOption[])new GUILayoutOption[0]))
				{
					Confirm(UserResponse.Confirm);
				}
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}

			public void DrawConfirmButton(UserResponse response)
			{
				DrawConfirmButton(response, "Confirm");
			}

			public void DrawConfirmButton(UserResponse response, string title)
			{
				bool enabled = GUI.get_enabled();
				if (busy)
				{
					GUI.set_enabled(false);
				}
				if (GUILayout.Button(title, (GUILayoutOption[])new GUILayoutOption[0]))
				{
					Confirm(response);
				}
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}

			public void DrawCancelButton()
			{
				DrawCancelButton("Cancel");
			}

			public void DrawCancelButton(string title)
			{
				bool enabled = GUI.get_enabled();
				if (busy)
				{
					GUI.set_enabled(false);
				}
				if (GUILayout.Button(title, (GUILayoutOption[])new GUILayoutOption[0]))
				{
					Cancel();
				}
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}

			public void Confirm()
			{
				Confirm(UserResponse.Confirm);
			}

			public void Confirm(UserResponse response)
			{
				resultCallback(currentActionId, response);
				Close();
			}

			public void Cancel()
			{
				resultCallback(currentActionId, UserResponse.Cancel);
				Close();
			}

			private void DrawWindow(int windowId)
			{
				windowProperties.windowDrawDelegate(windowProperties.title, windowProperties.message);
			}

			private void UpdateTimers()
			{
				if (_busyTimerRunning && busyTimer <= 0f)
				{
					_busyTimerRunning = false;
				}
			}

			private void StartBusyTimer(float time)
			{
				_busyTime = time + Time.get_realtimeSinceStartup();
				_busyTimerRunning = true;
			}

			private void Close()
			{
				Reset();
				StateChanged(0.1f);
			}

			private void StateChanged(float delay)
			{
				StartBusyTimer(delay);
			}

			private void Reset()
			{
				_enabled = false;
				_type = DialogType.None;
				currentActionId = -1;
				resultCallback = null;
			}

			private void ResetTimers()
			{
				_busyTimerRunning = false;
			}

			public void FullReset()
			{
				Reset();
				ResetTimers();
			}
		}

		private abstract class QueueEntry
		{
			public enum State
			{
				Waiting,
				Confirmed,
				Canceled
			}

			private static int uidCounter;

			public int id
			{
				get;
				protected set;
			}

			public QueueActionType queueActionType
			{
				get;
				protected set;
			}

			public State state
			{
				get;
				protected set;
			}

			public UserResponse response
			{
				get;
				protected set;
			}

			protected static int nextId
			{
				get
				{
					int result = uidCounter;
					uidCounter++;
					return result;
				}
			}

			public QueueEntry(QueueActionType queueActionType)
			{
				id = nextId;
				this.queueActionType = queueActionType;
			}

			public void Confirm(UserResponse response)
			{
				state = State.Confirmed;
				this.response = response;
			}

			public void Cancel()
			{
				state = State.Canceled;
			}
		}

		private class JoystickAssignmentChange : QueueEntry
		{
			public int playerId
			{
				get;
				private set;
			}

			public int joystickId
			{
				get;
				private set;
			}

			public bool assign
			{
				get;
				private set;
			}

			public JoystickAssignmentChange(int newPlayerId, int joystickId, bool assign)
				: base(QueueActionType.JoystickAssignment)
			{
				playerId = newPlayerId;
				this.joystickId = joystickId;
				this.assign = assign;
			}
		}

		private class ElementAssignmentChange : QueueEntry
		{
			public ElementAssignmentChangeType changeType
			{
				get;
				set;
			}

			public Context context
			{
				get;
				private set;
			}

			public ElementAssignmentChange(ElementAssignmentChangeType changeType, Context context)
				: base(QueueActionType.ElementAssignment)
			{
				this.changeType = changeType;
				this.context = context;
			}

			public ElementAssignmentChange(ElementAssignmentChange other)
				: this(other.changeType, other.context.Clone())
			{
			}
		}

		private class FallbackJoystickIdentification : QueueEntry
		{
			public int joystickId
			{
				get;
				private set;
			}

			public string joystickName
			{
				get;
				private set;
			}

			public FallbackJoystickIdentification(int joystickId, string joystickName)
				: base(QueueActionType.FallbackJoystickIdentification)
			{
				this.joystickId = joystickId;
				this.joystickName = joystickName;
			}
		}

		private class Calibration : QueueEntry
		{
			public int selectedElementIdentifierId;

			public bool recording;

			public Player player
			{
				get;
				private set;
			}

			public ControllerType controllerType
			{
				get;
				private set;
			}

			public Joystick joystick
			{
				get;
				private set;
			}

			public CalibrationMap calibrationMap
			{
				get;
				private set;
			}

			public Calibration(Player player, Joystick joystick, CalibrationMap calibrationMap)
				: base(QueueActionType.Calibrate)
			{
				this.player = player;
				this.joystick = joystick;
				this.calibrationMap = calibrationMap;
				selectedElementIdentifierId = -1;
			}
		}

		private struct WindowProperties
		{
			public int windowId;

			public Rect rect;

			public Action<string, string> windowDrawDelegate;

			public string title;

			public string message;
		}

		private enum QueueActionType
		{
			None,
			JoystickAssignment,
			ElementAssignment,
			FallbackJoystickIdentification,
			Calibrate
		}

		private enum ElementAssignmentChangeType
		{
			Add,
			Replace,
			Remove,
			ReassignOrRemove,
			ConflictCheck
		}

		public enum UserResponse
		{
			Confirm,
			Cancel,
			Custom1,
			Custom2
		}

		private const float defaultModalWidth = 250f;

		private const float defaultModalHeight = 200f;

		private const float assignmentTimeout = 5f;

		private DialogHelper dialog;

		private InputMapper inputMapper = new InputMapper();

		private ConflictFoundEventData conflictFoundEventData;

		private bool guiState;

		private bool busy;

		private bool pageGUIState;

		private Player selectedPlayer;

		private int selectedMapCategoryId;

		private ControllerSelection selectedController;

		private ControllerMap selectedMap;

		private bool showMenu;

		private bool startListening;

		private Vector2 actionScrollPos;

		private Vector2 calibrateScrollPos;

		private Queue<QueueEntry> actionQueue;

		private bool setupFinished;

		[NonSerialized]
		private bool initialized;

		private bool isCompiling;

		private GUIStyle style_wordWrap;

		private GUIStyle style_centeredBox;

		public ControlRemappingDemo1()
			: this()
		{
		}//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Expected O, but got Unknown


		private void Awake()
		{
			inputMapper.get_options().set_timeout(5f);
			inputMapper.get_options().set_ignoreMouseXAxis(true);
			inputMapper.get_options().set_ignoreMouseYAxis(true);
			Initialize();
		}

		private void OnEnable()
		{
			Subscribe();
		}

		private void OnDisable()
		{
			Unsubscribe();
		}

		private void Initialize()
		{
			dialog = new DialogHelper();
			actionQueue = new Queue<QueueEntry>();
			selectedController = new ControllerSelection();
			ReInput.add_ControllerConnectedEvent((Action<ControllerStatusChangedEventArgs>)JoystickConnected);
			ReInput.add_ControllerPreDisconnectEvent((Action<ControllerStatusChangedEventArgs>)JoystickPreDisconnect);
			ReInput.add_ControllerDisconnectedEvent((Action<ControllerStatusChangedEventArgs>)JoystickDisconnected);
			ResetAll();
			initialized = true;
			ReInput.get_userDataStore().Load();
			if (ReInput.get_unityJoystickIdentificationRequired())
			{
				IdentifyAllJoysticks();
			}
		}

		private void Setup()
		{
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Expected O, but got Unknown
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Expected O, but got Unknown
			if (!setupFinished)
			{
				style_wordWrap = new GUIStyle(GUI.get_skin().get_label());
				style_wordWrap.set_wordWrap(true);
				style_centeredBox = new GUIStyle(GUI.get_skin().get_box());
				style_centeredBox.set_alignment(4);
				setupFinished = true;
			}
		}

		private void Subscribe()
		{
			Unsubscribe();
			inputMapper.add_ConflictFoundEvent((Action<ConflictFoundEventData>)OnConflictFound);
			inputMapper.add_StoppedEvent((Action<StoppedEventData>)OnStopped);
		}

		private void Unsubscribe()
		{
			inputMapper.RemoveAllEventListeners();
		}

		public void OnGUI()
		{
			if (initialized)
			{
				Setup();
				HandleMenuControl();
				if (!showMenu)
				{
					DrawInitialScreen();
					return;
				}
				SetGUIStateStart();
				ProcessQueue();
				DrawPage();
				ShowDialog();
				SetGUIStateEnd();
				busy = false;
			}
		}

		private void HandleMenuControl()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Invalid comparison between Unknown and I4
			if (!dialog.enabled && (int)Event.get_current().get_type() == 8 && ReInput.get_players().GetSystemPlayer().GetButtonDown("Menu"))
			{
				if (showMenu)
				{
					ReInput.get_userDataStore().Save();
					Close();
				}
				else
				{
					Open();
				}
			}
		}

		private void Close()
		{
			ClearWorkingVars();
			showMenu = false;
		}

		private void Open()
		{
			showMenu = true;
		}

		private void DrawInitialScreen()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Expected O, but got Unknown
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			ActionElementMap firstElementMapWithAction = ReInput.get_players().GetSystemPlayer().controllers.maps.GetFirstElementMapWithAction("Menu", true);
			GUIContent val = (firstElementMapWithAction == null) ? new GUIContent("There is no element assigned to open the menu!") : new GUIContent("Press " + firstElementMapWithAction.get_elementIdentifierName() + " to open the menu.");
			GUILayout.BeginArea(GetScreenCenteredRect(300f, 50f));
			GUILayout.Box(val, style_centeredBox, (GUILayoutOption[])new GUILayoutOption[2]
			{
				GUILayout.ExpandHeight(true),
				GUILayout.ExpandWidth(true)
			});
			GUILayout.EndArea();
		}

		private void DrawPage()
		{
			//IL_006c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0098: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			if (GUI.get_enabled() != pageGUIState)
			{
				GUI.set_enabled(pageGUIState);
			}
			Rect val = default(Rect);
			val._002Ector(((float)Screen.get_width() - (float)Screen.get_width() * 0.9f) * 0.5f, ((float)Screen.get_height() - (float)Screen.get_height() * 0.9f) * 0.5f, (float)Screen.get_width() * 0.9f, (float)Screen.get_height() * 0.9f);
			GUILayout.BeginArea(val);
			DrawPlayerSelector();
			DrawJoystickSelector();
			DrawMouseAssignment();
			DrawControllerSelector();
			DrawCalibrateButton();
			DrawMapCategories();
			actionScrollPos = GUILayout.BeginScrollView(actionScrollPos, (GUILayoutOption[])new GUILayoutOption[0]);
			DrawCategoryActions();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
		}

		private void DrawPlayerSelector()
		{
			if (ReInput.get_players().get_allPlayerCount() == 0)
			{
				GUILayout.Label("There are no players.", (GUILayoutOption[])new GUILayoutOption[0]);
				return;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Players:", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			foreach (Player player in ReInput.get_players().GetPlayers(true))
			{
				if (selectedPlayer == null)
				{
					selectedPlayer = player;
				}
				bool flag = (player == selectedPlayer) ? true : false;
				bool flag2 = GUILayout.Toggle(flag, (!(player.get_descriptiveName() != string.Empty)) ? player.get_name() : player.get_descriptiveName(), GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag2 != flag && flag2)
				{
					selectedPlayer = player;
					selectedController.Clear();
					selectedMapCategoryId = -1;
				}
			}
			GUILayout.EndHorizontal();
		}

		private void DrawMouseAssignment()
		{
			bool enabled = GUI.get_enabled();
			if (selectedPlayer == null)
			{
				GUI.set_enabled(false);
			}
			GUILayout.Space(15f);
			GUILayout.Label("Assign Mouse:", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			bool flag = (selectedPlayer != null && selectedPlayer.controllers.get_hasMouse()) ? true : false;
			bool flag2 = GUILayout.Toggle(flag, "Assign Mouse", GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				if (flag2)
				{
					selectedPlayer.controllers.set_hasMouse(true);
					foreach (Player player in ReInput.get_players().get_Players())
					{
						if (player != selectedPlayer)
						{
							player.controllers.set_hasMouse(false);
						}
					}
				}
				else
				{
					selectedPlayer.controllers.set_hasMouse(false);
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.get_enabled() != enabled)
			{
				GUI.set_enabled(enabled);
			}
		}

		private void DrawJoystickSelector()
		{
			bool enabled = GUI.get_enabled();
			if (selectedPlayer == null)
			{
				GUI.set_enabled(false);
			}
			GUILayout.Space(15f);
			GUILayout.Label("Assign Joysticks:", (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			bool flag = (selectedPlayer == null || selectedPlayer.controllers.get_joystickCount() == 0) ? true : false;
			bool flag2 = GUILayout.Toggle(flag, "None", GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag2 != flag)
			{
				selectedPlayer.controllers.ClearControllersOfType(2);
				ControllerSelectionChanged();
			}
			if (selectedPlayer != null)
			{
				foreach (Joystick joystick in ReInput.get_controllers().get_Joysticks())
				{
					flag = selectedPlayer.controllers.ContainsController(joystick);
					flag2 = GUILayout.Toggle(flag, joystick.get_name(), GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
					{
						GUILayout.ExpandWidth(false)
					});
					if (flag2 != flag)
					{
						EnqueueAction(new JoystickAssignmentChange(selectedPlayer.get_id(), joystick.id, flag2));
					}
				}
			}
			GUILayout.EndHorizontal();
			if (GUI.get_enabled() != enabled)
			{
				GUI.set_enabled(enabled);
			}
		}

		private void DrawControllerSelector()
		{
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Invalid comparison between Unknown and I4
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cf: Invalid comparison between Unknown and I4
			//IL_014c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0152: Invalid comparison between Unknown and I4
			if (selectedPlayer != null)
			{
				bool enabled = GUI.get_enabled();
				GUILayout.Space(15f);
				GUILayout.Label("Controller to Map:", (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				if (!selectedController.hasSelection)
				{
					selectedController.Set(0, 0);
					ControllerSelectionChanged();
				}
				bool flag = (int)selectedController.type == 0;
				bool flag2 = GUILayout.Toggle(flag, "Keyboard", GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag2 != flag)
				{
					selectedController.Set(0, 0);
					ControllerSelectionChanged();
				}
				if (!selectedPlayer.controllers.get_hasMouse())
				{
					GUI.set_enabled(false);
				}
				flag = ((int)selectedController.type == 1);
				flag2 = GUILayout.Toggle(flag, "Mouse", GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag2 != flag)
				{
					selectedController.Set(0, 1);
					ControllerSelectionChanged();
				}
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
				foreach (Joystick joystick in selectedPlayer.controllers.get_Joysticks())
				{
					flag = ((int)selectedController.type == 2 && selectedController.id == joystick.id);
					flag2 = GUILayout.Toggle(flag, joystick.get_name(), GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
					{
						GUILayout.ExpandWidth(false)
					});
					if (flag2 != flag)
					{
						selectedController.Set(joystick.id, 2);
						ControllerSelectionChanged();
					}
				}
				GUILayout.EndHorizontal();
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}
		}

		private void DrawCalibrateButton()
		{
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Invalid comparison between Unknown and I4
			if (selectedPlayer == null)
			{
				return;
			}
			bool enabled = GUI.get_enabled();
			GUILayout.Space(10f);
			Controller val = (!selectedController.hasSelection) ? null : selectedPlayer.controllers.GetController(selectedController.type, selectedController.id);
			if (val == null || (int)selectedController.type != 2)
			{
				GUI.set_enabled(false);
				GUILayout.Button("Select a controller to calibrate", (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}
			else if (GUILayout.Button("Calibrate " + val.get_name(), (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				Joystick val2 = val as Joystick;
				if (val2 != null)
				{
					CalibrationMap calibrationMap = val2.get_calibrationMap();
					if (calibrationMap != null)
					{
						EnqueueAction(new Calibration(selectedPlayer, val2, calibrationMap));
					}
				}
			}
			if (GUI.get_enabled() != enabled)
			{
				GUI.set_enabled(enabled);
			}
		}

		private void DrawMapCategories()
		{
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
			//IL_0174: Unknown result type (might be due to invalid IL or missing references)
			if (selectedPlayer != null && selectedController.hasSelection)
			{
				bool enabled = GUI.get_enabled();
				GUILayout.Space(15f);
				GUILayout.Label("Categories:", (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				foreach (InputMapCategory userAssignableMapCategory in ReInput.get_mapping().get_UserAssignableMapCategories())
				{
					if (!selectedPlayer.controllers.maps.ContainsMapInCategory(selectedController.type, userAssignableMapCategory.get_id()))
					{
						GUI.set_enabled(false);
					}
					else if (selectedMapCategoryId < 0)
					{
						selectedMapCategoryId = userAssignableMapCategory.get_id();
						selectedMap = selectedPlayer.controllers.maps.GetFirstMapInCategory(selectedController.type, selectedController.id, userAssignableMapCategory.get_id());
					}
					bool flag = (userAssignableMapCategory.get_id() == selectedMapCategoryId) ? true : false;
					bool flag2 = GUILayout.Toggle(flag, (!(userAssignableMapCategory.get_descriptiveName() != string.Empty)) ? userAssignableMapCategory.get_name() : userAssignableMapCategory.get_descriptiveName(), GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
					{
						GUILayout.ExpandWidth(false)
					});
					if (flag2 != flag)
					{
						selectedMapCategoryId = userAssignableMapCategory.get_id();
						selectedMap = selectedPlayer.controllers.maps.GetFirstMapInCategory(selectedController.type, selectedController.id, userAssignableMapCategory.get_id());
					}
					if (GUI.get_enabled() != enabled)
					{
						GUI.set_enabled(enabled);
					}
				}
				GUILayout.EndHorizontal();
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}
		}

		private void DrawCategoryActions()
		{
			//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d3: Invalid comparison between Unknown and I4
			//IL_019f: Unknown result type (might be due to invalid IL or missing references)
			//IL_01af: Unknown result type (might be due to invalid IL or missing references)
			//IL_0234: Unknown result type (might be due to invalid IL or missing references)
			//IL_023a: Invalid comparison between Unknown and I4
			//IL_0246: Unknown result type (might be due to invalid IL or missing references)
			//IL_024c: Invalid comparison between Unknown and I4
			//IL_036f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0380: Unknown result type (might be due to invalid IL or missing references)
			//IL_0386: Invalid comparison between Unknown and I4
			//IL_0487: Unknown result type (might be due to invalid IL or missing references)
			//IL_048d: Invalid comparison between Unknown and I4
			//IL_0499: Unknown result type (might be due to invalid IL or missing references)
			//IL_049f: Invalid comparison between Unknown and I4
			if (selectedPlayer == null || selectedMapCategoryId < 0)
			{
				return;
			}
			bool enabled = GUI.get_enabled();
			if (selectedMap == null)
			{
				return;
			}
			GUILayout.Space(15f);
			GUILayout.Label("Actions:", (GUILayoutOption[])new GUILayoutOption[0]);
			InputMapCategory mapCategory = ReInput.get_mapping().GetMapCategory(selectedMapCategoryId);
			if (mapCategory == null)
			{
				return;
			}
			InputCategory actionCategory = ReInput.get_mapping().GetActionCategory(mapCategory.get_name());
			if (actionCategory != null)
			{
				float num = 150f;
				foreach (InputAction item in ReInput.get_mapping().ActionsInCategory(actionCategory.get_id()))
				{
					string text = (!(item.get_descriptiveName() != string.Empty)) ? item.get_name() : item.get_descriptiveName();
					if ((int)item.get_type() == 1)
					{
						GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
						GUILayout.Label(text, (GUILayoutOption[])new GUILayoutOption[1]
						{
							GUILayout.Width(num)
						});
						DrawAddActionMapButton(selectedPlayer.get_id(), item, 1, selectedController, selectedMap);
						foreach (ActionElementMap allMap in selectedMap.get_AllMaps())
						{
							if (allMap.get_actionId() == item.get_id())
							{
								DrawActionAssignmentButton(selectedPlayer.get_id(), item, 1, selectedController, selectedMap, allMap);
							}
						}
						GUILayout.EndHorizontal();
					}
					else if ((int)item.get_type() == 0)
					{
						if ((int)selectedController.type != 0)
						{
							GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
							GUILayout.Label(text, (GUILayoutOption[])new GUILayoutOption[1]
							{
								GUILayout.Width(num)
							});
							DrawAddActionMapButton(selectedPlayer.get_id(), item, 0, selectedController, selectedMap);
							foreach (ActionElementMap allMap2 in selectedMap.get_AllMaps())
							{
								if (allMap2.get_actionId() == item.get_id() && (int)allMap2.get_elementType() != 1 && (int)allMap2.get_axisType() != 2)
								{
									DrawActionAssignmentButton(selectedPlayer.get_id(), item, 0, selectedController, selectedMap, allMap2);
									DrawInvertButton(selectedPlayer.get_id(), item, 0, selectedController, selectedMap, allMap2);
								}
							}
							GUILayout.EndHorizontal();
						}
						string text2 = (!(item.get_positiveDescriptiveName() != string.Empty)) ? (item.get_descriptiveName() + " +") : item.get_positiveDescriptiveName();
						GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
						GUILayout.Label(text2, (GUILayoutOption[])new GUILayoutOption[1]
						{
							GUILayout.Width(num)
						});
						DrawAddActionMapButton(selectedPlayer.get_id(), item, 1, selectedController, selectedMap);
						foreach (ActionElementMap allMap3 in selectedMap.get_AllMaps())
						{
							if (allMap3.get_actionId() == item.get_id() && (int)allMap3.get_axisContribution() == 0 && (int)allMap3.get_axisType() != 1)
							{
								DrawActionAssignmentButton(selectedPlayer.get_id(), item, 1, selectedController, selectedMap, allMap3);
							}
						}
						GUILayout.EndHorizontal();
						string text3 = (!(item.get_negativeDescriptiveName() != string.Empty)) ? (item.get_descriptiveName() + " -") : item.get_negativeDescriptiveName();
						GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
						GUILayout.Label(text3, (GUILayoutOption[])new GUILayoutOption[1]
						{
							GUILayout.Width(num)
						});
						DrawAddActionMapButton(selectedPlayer.get_id(), item, 2, selectedController, selectedMap);
						foreach (ActionElementMap allMap4 in selectedMap.get_AllMaps())
						{
							if (allMap4.get_actionId() == item.get_id() && (int)allMap4.get_axisContribution() == 1 && (int)allMap4.get_axisType() != 1)
							{
								DrawActionAssignmentButton(selectedPlayer.get_id(), item, 2, selectedController, selectedMap, allMap4);
							}
						}
						GUILayout.EndHorizontal();
					}
				}
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}
		}

		private void DrawActionAssignmentButton(int playerId, InputAction action, AxisRange actionRange, ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap)
		{
			//IL_002d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Expected O, but got Unknown
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			if (GUILayout.Button(elementMap.get_elementIdentifierName(), (GUILayoutOption[])new GUILayoutOption[2]
			{
				GUILayout.ExpandWidth(false),
				GUILayout.MinWidth(30f)
			}))
			{
				Context val = new Context();
				val.set_actionId(action.get_id());
				val.set_actionRange(actionRange);
				val.set_controllerMap(controllerMap);
				val.set_actionElementMapToReplace(elementMap);
				Context context = val;
				EnqueueAction(new ElementAssignmentChange(ElementAssignmentChangeType.ReassignOrRemove, context));
				startListening = true;
			}
			GUILayout.Space(4f);
		}

		private void DrawInvertButton(int playerId, InputAction action, Pole actionAxisContribution, ControllerSelection controller, ControllerMap controllerMap, ActionElementMap elementMap)
		{
			bool invert = elementMap.get_invert();
			bool flag = GUILayout.Toggle(invert, "Invert", (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.ExpandWidth(false)
			});
			if (flag != invert)
			{
				elementMap.set_invert(flag);
			}
			GUILayout.Space(10f);
		}

		private void DrawAddActionMapButton(int playerId, InputAction action, AxisRange actionRange, ControllerSelection controller, ControllerMap controllerMap)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Expected O, but got Unknown
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			if (GUILayout.Button("Add...", (GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.ExpandWidth(false)
			}))
			{
				Context val = new Context();
				val.set_actionId(action.get_id());
				val.set_actionRange(actionRange);
				val.set_controllerMap(controllerMap);
				Context context = val;
				EnqueueAction(new ElementAssignmentChange(ElementAssignmentChangeType.Add, context));
				startListening = true;
			}
			GUILayout.Space(10f);
		}

		private void ShowDialog()
		{
			dialog.Update();
		}

		private void DrawModalWindow(string title, string message)
		{
			if (dialog.enabled)
			{
				GUILayout.Space(5f);
				GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				dialog.DrawConfirmButton("Okay");
				GUILayout.FlexibleSpace();
				dialog.DrawCancelButton();
				GUILayout.EndHorizontal();
			}
		}

		private void DrawModalWindow_OkayOnly(string title, string message)
		{
			if (dialog.enabled)
			{
				GUILayout.Space(5f);
				GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				dialog.DrawConfirmButton("Okay");
				GUILayout.EndHorizontal();
			}
		}

		private void DrawElementAssignmentWindow(string title, string message)
		{
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (!dialog.enabled)
			{
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			ElementAssignmentChange elementAssignmentChange = actionQueue.Peek() as ElementAssignmentChange;
			if (elementAssignmentChange == null)
			{
				dialog.Cancel();
				return;
			}
			float num;
			if (!dialog.busy)
			{
				if (startListening && (int)inputMapper.get_status() == 0)
				{
					inputMapper.Start(elementAssignmentChange.context);
					startListening = false;
				}
				if (conflictFoundEventData != null)
				{
					dialog.Confirm();
					return;
				}
				num = inputMapper.get_timeRemaining();
				if (num == 0f)
				{
					dialog.Cancel();
					return;
				}
			}
			else
			{
				num = inputMapper.get_options().get_timeout();
			}
			GUILayout.Label("Assignment will be canceled in " + ((int)Mathf.Ceil(num)).ToString() + "...", style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
		}

		private void DrawElementAssignmentProtectedConflictWindow(string title, string message)
		{
			if (dialog.enabled)
			{
				GUILayout.Space(5f);
				GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				ElementAssignmentChange elementAssignmentChange = actionQueue.Peek() as ElementAssignmentChange;
				if (elementAssignmentChange == null)
				{
					dialog.Cancel();
					return;
				}
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				dialog.DrawConfirmButton(UserResponse.Custom1, "Add");
				GUILayout.FlexibleSpace();
				dialog.DrawCancelButton();
				GUILayout.EndHorizontal();
			}
		}

		private void DrawElementAssignmentNormalConflictWindow(string title, string message)
		{
			if (dialog.enabled)
			{
				GUILayout.Space(5f);
				GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				ElementAssignmentChange elementAssignmentChange = actionQueue.Peek() as ElementAssignmentChange;
				if (elementAssignmentChange == null)
				{
					dialog.Cancel();
					return;
				}
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				dialog.DrawConfirmButton(UserResponse.Confirm, "Replace");
				GUILayout.FlexibleSpace();
				dialog.DrawConfirmButton(UserResponse.Custom1, "Add");
				GUILayout.FlexibleSpace();
				dialog.DrawCancelButton();
				GUILayout.EndHorizontal();
			}
		}

		private void DrawReassignOrRemoveElementAssignmentWindow(string title, string message)
		{
			if (dialog.enabled)
			{
				GUILayout.Space(5f);
				GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
				dialog.DrawConfirmButton("Reassign");
				GUILayout.FlexibleSpace();
				dialog.DrawCancelButton("Remove");
				GUILayout.EndHorizontal();
			}
		}

		private void DrawFallbackJoystickIdentificationWindow(string title, string message)
		{
			if (!dialog.enabled)
			{
				return;
			}
			FallbackJoystickIdentification fallbackJoystickIdentification = actionQueue.Peek() as FallbackJoystickIdentification;
			if (fallbackJoystickIdentification == null)
			{
				dialog.Cancel();
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Label("Press any button or axis on \"" + fallbackJoystickIdentification.joystickName + "\" now.", style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Skip", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				dialog.Cancel();
			}
			else if (!dialog.busy && ReInput.get_controllers().SetUnityJoystickIdFromAnyButtonOrAxisPress(fallbackJoystickIdentification.joystickId, 0.8f, false))
			{
				dialog.Confirm();
			}
		}

		private void DrawCalibrationWindow(string title, string message)
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			//IL_0090: Unknown result type (might be due to invalid IL or missing references)
			//IL_0095: Unknown result type (might be due to invalid IL or missing references)
			//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_04af: Unknown result type (might be due to invalid IL or missing references)
			if (!dialog.enabled)
			{
				return;
			}
			Calibration calibration = actionQueue.Peek() as Calibration;
			if (calibration == null)
			{
				dialog.Cancel();
				return;
			}
			GUILayout.Space(5f);
			GUILayout.Label(message, style_wordWrap, (GUILayoutOption[])new GUILayoutOption[0]);
			GUILayout.Space(20f);
			GUILayout.BeginHorizontal((GUILayoutOption[])new GUILayoutOption[0]);
			bool enabled = GUI.get_enabled();
			GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.Width(200f)
			});
			calibrateScrollPos = GUILayout.BeginScrollView(calibrateScrollPos, (GUILayoutOption[])new GUILayoutOption[0]);
			if (calibration.recording)
			{
				GUI.set_enabled(false);
			}
			IList<ControllerElementIdentifier> axisElementIdentifiers = calibration.joystick.get_AxisElementIdentifiers();
			for (int i = 0; i < axisElementIdentifiers.Count; i++)
			{
				ControllerElementIdentifier val = axisElementIdentifiers[i];
				bool flag = calibration.selectedElementIdentifierId == val.get_id();
				bool flag2 = GUILayout.Toggle(flag, val.get_name(), GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag != flag2)
				{
					calibration.selectedElementIdentifierId = val.get_id();
				}
			}
			if (GUI.get_enabled() != enabled)
			{
				GUI.set_enabled(enabled);
			}
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.BeginVertical((GUILayoutOption[])new GUILayoutOption[1]
			{
				GUILayout.Width(200f)
			});
			if (calibration.selectedElementIdentifierId >= 0)
			{
				float axisRawById = calibration.joystick.GetAxisRawById(calibration.selectedElementIdentifierId);
				GUILayout.Label("Raw Value: " + axisRawById.ToString(), (GUILayoutOption[])new GUILayoutOption[0]);
				int axisIndexById = calibration.joystick.GetAxisIndexById(calibration.selectedElementIdentifierId);
				AxisCalibration axis = calibration.calibrationMap.GetAxis(axisIndexById);
				GUILayout.Label("Calibrated Value: " + calibration.joystick.GetAxisById(calibration.selectedElementIdentifierId), (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Zero: " + axis.get_calibratedZero(), (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Min: " + axis.get_calibratedMin(), (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Max: " + axis.get_calibratedMax(), (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Label("Dead Zone: " + axis.get_deadZone(), (GUILayoutOption[])new GUILayoutOption[0]);
				GUILayout.Space(15f);
				bool flag3 = GUILayout.Toggle(axis.get_enabled(), "Enabled", GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (axis.get_enabled() != flag3)
				{
					axis.set_enabled(flag3);
				}
				GUILayout.Space(10f);
				bool flag4 = GUILayout.Toggle(calibration.recording, "Record Min/Max", GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (flag4 != calibration.recording)
				{
					if (flag4)
					{
						axis.set_calibratedMax(0f);
						axis.set_calibratedMin(0f);
					}
					calibration.recording = flag4;
				}
				if (calibration.recording)
				{
					axis.set_calibratedMin(Mathf.Min(new float[3]
					{
						axis.get_calibratedMin(),
						axisRawById,
						axis.get_calibratedMin()
					}));
					axis.set_calibratedMax(Mathf.Max(new float[3]
					{
						axis.get_calibratedMax(),
						axisRawById,
						axis.get_calibratedMax()
					}));
					GUI.set_enabled(false);
				}
				if (GUILayout.Button("Set Zero", (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.set_calibratedZero(axisRawById);
				}
				if (GUILayout.Button("Set Dead Zone", (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.set_deadZone(axisRawById);
				}
				bool flag5 = GUILayout.Toggle(axis.get_invert(), "Invert", GUIStyle.op_Implicit("Button"), (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				});
				if (axis.get_invert() != flag5)
				{
					axis.set_invert(flag5);
				}
				GUILayout.Space(10f);
				if (GUILayout.Button("Reset", (GUILayoutOption[])new GUILayoutOption[1]
				{
					GUILayout.ExpandWidth(false)
				}))
				{
					axis.Reset();
				}
				if (GUI.get_enabled() != enabled)
				{
					GUI.set_enabled(enabled);
				}
			}
			else
			{
				GUILayout.Label("Select an axis to begin.", (GUILayoutOption[])new GUILayoutOption[0]);
			}
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
			GUILayout.FlexibleSpace();
			if (calibration.recording)
			{
				GUI.set_enabled(false);
			}
			if (GUILayout.Button("Close", (GUILayoutOption[])new GUILayoutOption[0]))
			{
				calibrateScrollPos = default(Vector2);
				dialog.Confirm();
			}
			if (GUI.get_enabled() != enabled)
			{
				GUI.set_enabled(enabled);
			}
		}

		private void DialogResultCallback(int queueActionId, UserResponse response)
		{
			foreach (QueueEntry item in actionQueue)
			{
				if (item.id == queueActionId)
				{
					if (response != UserResponse.Cancel)
					{
						item.Confirm(response);
					}
					else
					{
						item.Cancel();
					}
					break;
				}
			}
		}

		private Rect GetScreenCenteredRect(float width, float height)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			return new Rect((float)Screen.get_width() * 0.5f - width * 0.5f, (float)((double)Screen.get_height() * 0.5 - (double)(height * 0.5f)), width, height);
		}

		private void EnqueueAction(QueueEntry entry)
		{
			if (entry != null)
			{
				busy = true;
				GUI.set_enabled(false);
				actionQueue.Enqueue(entry);
			}
		}

		private void ProcessQueue()
		{
			if (dialog.enabled || busy || actionQueue.Count == 0)
			{
				return;
			}
			while (actionQueue.Count > 0)
			{
				QueueEntry queueEntry = actionQueue.Peek();
				bool flag = false;
				switch (queueEntry.queueActionType)
				{
				case QueueActionType.JoystickAssignment:
					flag = ProcessJoystickAssignmentChange((JoystickAssignmentChange)queueEntry);
					break;
				case QueueActionType.ElementAssignment:
					flag = ProcessElementAssignmentChange((ElementAssignmentChange)queueEntry);
					break;
				case QueueActionType.FallbackJoystickIdentification:
					flag = ProcessFallbackJoystickIdentification((FallbackJoystickIdentification)queueEntry);
					break;
				case QueueActionType.Calibrate:
					flag = ProcessCalibration((Calibration)queueEntry);
					break;
				}
				if (!flag)
				{
					break;
				}
				actionQueue.Dequeue();
			}
		}

		private bool ProcessJoystickAssignmentChange(JoystickAssignmentChange entry)
		{
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			if (entry.state == QueueEntry.State.Canceled)
			{
				return true;
			}
			Player player = ReInput.get_players().GetPlayer(entry.playerId);
			if (player == null)
			{
				return true;
			}
			if (!entry.assign)
			{
				player.controllers.RemoveController(2, entry.joystickId);
				ControllerSelectionChanged();
				return true;
			}
			if (player.controllers.ContainsController(2, entry.joystickId))
			{
				return true;
			}
			if (!ReInput.get_controllers().IsJoystickAssigned(entry.joystickId) || entry.state == QueueEntry.State.Confirmed)
			{
				player.controllers.AddController(2, entry.joystickId, true);
				ControllerSelectionChanged();
				return true;
			}
			dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties
			{
				title = "Joystick Reassignment",
				message = "This joystick is already assigned to another player. Do you want to reassign this joystick to " + player.get_descriptiveName() + "?",
				rect = GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = DrawModalWindow
			}, DialogResultCallback);
			return false;
		}

		private bool ProcessElementAssignmentChange(ElementAssignmentChange entry)
		{
			switch (entry.changeType)
			{
			case ElementAssignmentChangeType.ReassignOrRemove:
				return ProcessRemoveOrReassignElementAssignment(entry);
			case ElementAssignmentChangeType.Remove:
				return ProcessRemoveElementAssignment(entry);
			case ElementAssignmentChangeType.Add:
			case ElementAssignmentChangeType.Replace:
				return ProcessAddOrReplaceElementAssignment(entry);
			case ElementAssignmentChangeType.ConflictCheck:
				return ProcessElementAssignmentConflictCheck(entry);
			default:
				throw new NotImplementedException();
			}
		}

		private bool ProcessRemoveOrReassignElementAssignment(ElementAssignmentChange entry)
		{
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			if (entry.context.get_controllerMap() == null)
			{
				return true;
			}
			if (entry.state == QueueEntry.State.Canceled)
			{
				ElementAssignmentChange elementAssignmentChange = new ElementAssignmentChange(entry);
				elementAssignmentChange.changeType = ElementAssignmentChangeType.Remove;
				actionQueue.Enqueue(elementAssignmentChange);
				return true;
			}
			if (entry.state == QueueEntry.State.Confirmed)
			{
				ElementAssignmentChange elementAssignmentChange2 = new ElementAssignmentChange(entry);
				elementAssignmentChange2.changeType = ElementAssignmentChangeType.Replace;
				actionQueue.Enqueue(elementAssignmentChange2);
				return true;
			}
			dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
			{
				title = "Reassign or Remove",
				message = "Do you want to reassign or remove this assignment?",
				rect = GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = DrawReassignOrRemoveElementAssignmentWindow
			}, DialogResultCallback);
			return false;
		}

		private bool ProcessRemoveElementAssignment(ElementAssignmentChange entry)
		{
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			//IL_008f: Unknown result type (might be due to invalid IL or missing references)
			if (entry.context.get_controllerMap() == null)
			{
				return true;
			}
			if (entry.state == QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == QueueEntry.State.Confirmed)
			{
				entry.context.get_controllerMap().DeleteElementMap(entry.context.get_actionElementMapToReplace().get_id());
				return true;
			}
			dialog.StartModal(entry.id, DialogHelper.DialogType.DeleteAssignmentConfirmation, new WindowProperties
			{
				title = "Remove Assignment",
				message = "Are you sure you want to remove this assignment?",
				rect = GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = DrawModalWindow
			}, DialogResultCallback);
			return false;
		}

		private bool ProcessAddOrReplaceElementAssignment(ElementAssignmentChange entry)
		{
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Invalid comparison between Unknown and I4
			//IL_0069: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Invalid comparison between Unknown and I4
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00cc: Invalid comparison between Unknown and I4
			//IL_0119: Unknown result type (might be due to invalid IL or missing references)
			//IL_011e: Unknown result type (might be due to invalid IL or missing references)
			if (entry.state == QueueEntry.State.Canceled)
			{
				inputMapper.Stop();
				return true;
			}
			if (entry.state == QueueEntry.State.Confirmed)
			{
				if ((int)Event.get_current().get_type() != 8)
				{
					return false;
				}
				if (conflictFoundEventData != null)
				{
					ElementAssignmentChange elementAssignmentChange = new ElementAssignmentChange(entry);
					elementAssignmentChange.changeType = ElementAssignmentChangeType.ConflictCheck;
					actionQueue.Enqueue(elementAssignmentChange);
				}
				return true;
			}
			string text;
			if ((int)entry.context.get_controllerMap().get_controllerType() != 0)
			{
				text = (((int)entry.context.get_controllerMap().get_controllerType() != 1) ? "Press any button or axis to assign it to this action." : "Press any mouse button or axis to assign it to this action.\n\nTo assign mouse movement axes, move the mouse quickly in the direction you want mapped to the action. Slow movements will be ignored.");
			}
			else
			{
				text = (((int)Application.get_platform() != 0 && (int)Application.get_platform() != 1) ? "Press any key to assign it to this action. You may also use the modifier keys Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second." : "Press any key to assign it to this action. You may also use the modifier keys Command, Control, Alt, and Shift. If you wish to assign a modifier key itself to this action, press and hold the key for 1 second.");
				if (Application.get_isEditor())
				{
					text += "\n\nNOTE: Some modifier key combinations will not work in the Unity Editor, but they will work in a game build.";
				}
			}
			dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
			{
				title = "Assign",
				message = text,
				rect = GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = DrawElementAssignmentWindow
			}, DialogResultCallback);
			return false;
		}

		private bool ProcessElementAssignmentConflictCheck(ElementAssignmentChange entry)
		{
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
			//IL_0177: Unknown result type (might be due to invalid IL or missing references)
			//IL_017c: Unknown result type (might be due to invalid IL or missing references)
			if (entry.context.get_controllerMap() == null)
			{
				return true;
			}
			if (entry.state == QueueEntry.State.Canceled)
			{
				inputMapper.Stop();
				return true;
			}
			if (conflictFoundEventData == null)
			{
				return true;
			}
			if (entry.state == QueueEntry.State.Confirmed)
			{
				if (entry.response == UserResponse.Confirm)
				{
					conflictFoundEventData.responseCallback(1);
				}
				else
				{
					if (entry.response != UserResponse.Custom1)
					{
						throw new NotImplementedException();
					}
					conflictFoundEventData.responseCallback(2);
				}
				return true;
			}
			if (conflictFoundEventData.isProtected)
			{
				string message = conflictFoundEventData.assignment.get_elementDisplayName() + " is already in use and is protected from reassignment. You cannot remove the protected assignment, but you can still assign the action to this element. If you do so, the element will trigger multiple actions when activated.";
				dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
				{
					title = "Assignment Conflict",
					message = message,
					rect = GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = DrawElementAssignmentProtectedConflictWindow
				}, DialogResultCallback);
			}
			else
			{
				string message2 = conflictFoundEventData.assignment.get_elementDisplayName() + " is already in use. You may replace the other conflicting assignments, add this assignment anyway which will leave multiple actions assigned to this element, or cancel this assignment.";
				dialog.StartModal(entry.id, DialogHelper.DialogType.AssignElement, new WindowProperties
				{
					title = "Assignment Conflict",
					message = message2,
					rect = GetScreenCenteredRect(250f, 200f),
					windowDrawDelegate = DrawElementAssignmentNormalConflictWindow
				}, DialogResultCallback);
			}
			return false;
		}

		private bool ProcessFallbackJoystickIdentification(FallbackJoystickIdentification entry)
		{
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (entry.state == QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == QueueEntry.State.Confirmed)
			{
				return true;
			}
			dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties
			{
				title = "Joystick Identification Required",
				message = "A joystick has been attached or removed. You will need to identify each joystick by pressing a button on the controller listed below:",
				rect = GetScreenCenteredRect(250f, 200f),
				windowDrawDelegate = DrawFallbackJoystickIdentificationWindow
			}, DialogResultCallback, 1f);
			return false;
		}

		private bool ProcessCalibration(Calibration entry)
		{
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			if (entry.state == QueueEntry.State.Canceled)
			{
				return true;
			}
			if (entry.state == QueueEntry.State.Confirmed)
			{
				return true;
			}
			dialog.StartModal(entry.id, DialogHelper.DialogType.JoystickConflict, new WindowProperties
			{
				title = "Calibrate Controller",
				message = "Select an axis to calibrate on the " + entry.joystick.get_name() + ".",
				rect = GetScreenCenteredRect(450f, 480f),
				windowDrawDelegate = DrawCalibrationWindow
			}, DialogResultCallback);
			return false;
		}

		private void PlayerSelectionChanged()
		{
			ClearControllerSelection();
		}

		private void ControllerSelectionChanged()
		{
			ClearMapSelection();
		}

		private void ClearControllerSelection()
		{
			selectedController.Clear();
			ClearMapSelection();
		}

		private void ClearMapSelection()
		{
			selectedMapCategoryId = -1;
			selectedMap = null;
		}

		private void ResetAll()
		{
			ClearWorkingVars();
			initialized = false;
			showMenu = false;
		}

		private void ClearWorkingVars()
		{
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0021: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			selectedPlayer = null;
			ClearMapSelection();
			selectedController.Clear();
			actionScrollPos = default(Vector2);
			dialog.FullReset();
			actionQueue.Clear();
			busy = false;
			startListening = false;
			conflictFoundEventData = null;
			inputMapper.Stop();
		}

		private void SetGUIStateStart()
		{
			guiState = true;
			if (busy)
			{
				guiState = false;
			}
			pageGUIState = (guiState && !busy && !dialog.enabled && !dialog.busy);
			if (GUI.get_enabled() != guiState)
			{
				GUI.set_enabled(guiState);
			}
		}

		private void SetGUIStateEnd()
		{
			guiState = true;
			if (!GUI.get_enabled())
			{
				GUI.set_enabled(guiState);
			}
		}

		private void JoystickConnected(ControllerStatusChangedEventArgs args)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_005f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0097: Unknown result type (might be due to invalid IL or missing references)
			if (ReInput.get_controllers().IsControllerAssigned(args.get_controllerType(), args.get_controllerId()))
			{
				foreach (Player allPlayer in ReInput.get_players().get_AllPlayers())
				{
					if (allPlayer.controllers.ContainsController(args.get_controllerType(), args.get_controllerId()))
					{
						ReInput.get_userDataStore().LoadControllerData(allPlayer.get_id(), args.get_controllerType(), args.get_controllerId());
					}
				}
			}
			else
			{
				ReInput.get_userDataStore().LoadControllerData(args.get_controllerType(), args.get_controllerId());
			}
			if (ReInput.get_unityJoystickIdentificationRequired())
			{
				IdentifyAllJoysticks();
			}
		}

		private void JoystickPreDisconnect(ControllerStatusChangedEventArgs args)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_008b: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
			if (selectedController.hasSelection && args.get_controllerType() == selectedController.type && args.get_controllerId() == selectedController.id)
			{
				ClearControllerSelection();
			}
			if (showMenu)
			{
				if (ReInput.get_controllers().IsControllerAssigned(args.get_controllerType(), args.get_controllerId()))
				{
					foreach (Player allPlayer in ReInput.get_players().get_AllPlayers())
					{
						if (allPlayer.controllers.ContainsController(args.get_controllerType(), args.get_controllerId()))
						{
							ReInput.get_userDataStore().SaveControllerData(allPlayer.get_id(), args.get_controllerType(), args.get_controllerId());
						}
					}
				}
				else
				{
					ReInput.get_userDataStore().SaveControllerData(args.get_controllerType(), args.get_controllerId());
				}
			}
		}

		private void JoystickDisconnected(ControllerStatusChangedEventArgs args)
		{
			if (showMenu)
			{
				ClearWorkingVars();
			}
			if (ReInput.get_unityJoystickIdentificationRequired())
			{
				IdentifyAllJoysticks();
			}
		}

		private void OnConflictFound(ConflictFoundEventData data)
		{
			conflictFoundEventData = data;
		}

		private void OnStopped(StoppedEventData data)
		{
			conflictFoundEventData = null;
		}

		public void IdentifyAllJoysticks()
		{
			if (ReInput.get_controllers().get_joystickCount() != 0)
			{
				ClearWorkingVars();
				Open();
				foreach (Joystick joystick in ReInput.get_controllers().get_Joysticks())
				{
					actionQueue.Enqueue(new FallbackJoystickIdentification(joystick.id, joystick.get_name()));
				}
			}
		}

		protected void CheckRecompile()
		{
		}

		private void RecompileWindow(int windowId)
		{
		}
	}
}
