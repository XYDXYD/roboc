using RCNetwork.Events;
using Rewired;
using Simulation.Hardware;
using Simulation.Hardware.Weapons;
using Svelto.Context;
using Svelto.DataStructures;
using Svelto.ECS;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	internal sealed class AlignmentRectifierEngine : SingleEntityViewEngine<MachineRectifierNode>, IInitialize, IWaitForFrameworkDestruction, IHandleCharacterInput, IEngine, IHudElement, IInputComponent, IComponent
	{
		private enum FlippingManagerState
		{
			NotInitialized,
			Ready,
			AlignmentActive,
			CoolingDown,
			Destroyed
		}

		public const float ALIGNMENT_DURATION = 3f;

		public const float COOLDOWN_DURATION = 1f;

		private Dictionary<int, ButtonActionMap> _buttonToActionMap = new Dictionary<int, ButtonActionMap>();

		private const float MOVEMENT_SPEED_THRESHOLD = 0.5f;

		private const float BOUNDING_SPHERE_START_ALIGNMENT_TOLLERANCE_FACTOR = 1.2f;

		private float _cooldownTimer;

		private float _alignmentTimer;

		private FlippingManagerState _state;

		private AlignmentRectifierBehaviour _alignmentRectifierBehaviour;

		private AlignmentRectifierTimerView _alignmentRectifierTimerView;

		private AlignmentRectifierCooldownView _alignmentRectifierCooldownView;

		private AlignmentRectifierHintNoviceView _alignmentRectifierHintNoviceView;

		private AlignmentRectifierData _alignmentRectifierData;

		private Rigidbody _rigidBody;

		private AlignmentRectifierEffectView _alignmentRectifierEffectView;

		private bool _alignmentInputKeyDown;

		private Vector3 _lastVelocity;

		private AIInputWrapper _aiInputWrapper;

		private ITaskRoutine _physicsTick;

		private ITaskRoutine _tick;

		private readonly FasterList<IAlignmentRectifierPausable> _componentsList;

		private LocalAlignmentRectifierActivatedObservable _localAlignmentRectifierActivatedObservable;

		private MachineRectifierNode _localMachineRectifierNode;

		[Inject]
		internal INetworkEventManagerClient eventManagerClient
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

		[Inject]
		internal PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		internal GameObjectPool gameobjectPool
		{
			private get;
			set;
		}

		[Inject]
		internal DestructionReporter destructionReporter
		{
			private get;
			set;
		}

		[Inject]
		internal MachineSpawnDispatcher spawnDispatcher
		{
			private get;
			set;
		}

		[Inject]
		internal PlayerMachinesContainer playerMachinesContainer
		{
			private get;
			set;
		}

		[Inject]
		internal RigidbodyDataContainer rigidbodyData
		{
			private get;
			set;
		}

		public AlignmentRectifierEngine(LocalAlignmentRectifierActivatedObservable localAlignmentRectifierActivatedObservable)
		{
			_localAlignmentRectifierActivatedObservable = localAlignmentRectifierActivatedObservable;
			_componentsList = new FasterList<IAlignmentRectifierPausable>();
			_physicsTick = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			_physicsTick.SetEnumeratorProvider((Func<IEnumerator>)PhysicsTick).SetScheduler(StandardSchedulers.get_physicScheduler());
			_tick = TaskRunner.get_Instance().AllocateNewTaskRoutine();
			_tick.SetEnumeratorProvider((Func<IEnumerator>)Tick).SetScheduler(StandardSchedulers.get_updateScheduler());
		}

		public void OnFrameworkDestroyed()
		{
			battleHudStyleController.RemoveHud(this);
			_alignmentRectifierBehaviour = null;
			_alignmentRectifierEffectView = null;
			spawnDispatcher.OnPlayerRegistered -= HandleOnPlayerRegistered;
			spawnDispatcher.OnPlayerSpawnedIn -= OnPlayerSpawnedIn;
			spawnDispatcher.OnPlayerRespawnedIn -= HandleonRespawnedIn;
			destructionReporter.OnMachineDestroyed -= HandleOnMachineDestroyed;
			destructionReporter.OnPlayerSelfDestructs -= HandleonPlayerSelfDestructs;
		}

		public void OnDependenciesInjected()
		{
			spawnDispatcher.OnPlayerRegistered += HandleOnPlayerRegistered;
			spawnDispatcher.OnPlayerSpawnedIn += OnPlayerSpawnedIn;
			spawnDispatcher.OnPlayerRespawnedIn += HandleonRespawnedIn;
			destructionReporter.OnMachineDestroyed += HandleOnMachineDestroyed;
			destructionReporter.OnPlayerSelfDestructs += HandleonPlayerSelfDestructs;
			ButtonRemapHelperRewired buttonRemapHelperRewired = new ButtonRemapHelperRewired();
			_buttonToActionMap = buttonRemapHelperRewired.GenerateButtonActionMaps(isShopAvailable: false);
		}

		private void HandleOnPlayerRegistered(SpawnInParametersPlayer data)
		{
			if (data.isMe)
			{
				_rigidBody = data.preloadedMachine.rbData;
				_alignmentRectifierBehaviour = new AlignmentRectifierBehaviour(_rigidBody, data.preloadedMachine.machineInfo);
				_state = FlippingManagerState.Ready;
			}
		}

		private void OnPlayerSpawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (spawnInParameters.isMe)
			{
				_tick.Start((Action<PausableTaskException>)null, (Action)null);
				_physicsTick.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		public string FetchKeyCodeForAction(string actionName)
		{
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0078: Unknown result type (might be due to invalid IL or missing references)
			//IL_007f: Unknown result type (might be due to invalid IL or missing references)
			foreach (KeyValuePair<int, ButtonActionMap> item in _buttonToActionMap)
			{
				int key = item.Key;
				ButtonActionMap value = item.Value;
				Dictionary<int, ButtonReassignData> map = value.map;
				for (int i = 0; i < map.Count; i += 3)
				{
					string text = map[i].ActionName(localised: false);
					if (map[i].IsSplit() && text.CompareTo(actionName) == 0)
					{
						KeyboardKeyCode key2 = map[i].KeyCode();
						return StringTableBase<StringTable>.Instance.GetKeyboardKeyString(key2);
					}
				}
			}
			return null;
		}

		private void HandleonRespawnedIn(SpawnInParametersPlayer spawnInParameters)
		{
			if (spawnInParameters.isMe)
			{
				if (_state == FlippingManagerState.Ready)
				{
					_tick.Stop();
					_physicsTick.Stop();
				}
				_state = FlippingManagerState.Ready;
				_alignmentRectifierCooldownView.Show();
				_tick.Start((Action<PausableTaskException>)null, (Action)null);
				_physicsTick.Start((Action<PausableTaskException>)null, (Action)null);
			}
		}

		private IEnumerator Tick()
		{
			while (true)
			{
				float deltaSec = Time.get_deltaTime();
				switch (_state)
				{
				case FlippingManagerState.AlignmentActive:
					_alignmentRectifierBehaviour.TimeTick(deltaSec);
					_alignmentTimer += deltaSec;
					_alignmentRectifierTimerView.SetTime(_alignmentTimer);
					_alignmentRectifierTimerView.UpdateGui();
					if (_alignmentTimer > 3f)
					{
						HandleOnAlignmentEnded();
						ChangeStateToCoolDown();
					}
					break;
				case FlippingManagerState.CoolingDown:
					_cooldownTimer -= deltaSec;
					if (_cooldownTimer <= 0f)
					{
						_cooldownTimer = 0f;
						_state = FlippingManagerState.Ready;
					}
					break;
				}
				yield return null;
			}
		}

		private IEnumerator PhysicsTick()
		{
			while (true)
			{
				if (_rigidBody == null)
				{
					yield return null;
					continue;
				}
				float deltaSec = Time.get_fixedDeltaTime();
				EnsureRobotIsActive();
				switch (_state)
				{
				case FlippingManagerState.Ready:
					if (Vector3.Dot(_rigidBody.get_transform().get_up(), Vector3.get_up()) < 0f)
					{
						Vector3 velocity = _rigidBody.get_velocity();
						if (velocity.get_sqrMagnitude() < 0.25f && CheckStartConditions())
						{
							_alignmentRectifierHintNoviceView.EnableHint(_alignmentRectifierData.audioHintEffect);
							goto IL_0126;
						}
					}
					_alignmentRectifierHintNoviceView.DisableHint();
					goto IL_0126;
				case FlippingManagerState.AlignmentActive:
					{
						_alignmentRectifierBehaviour.PhysicsUpdate(deltaSec);
						break;
					}
					IL_0126:
					_lastVelocity = _rigidBody.get_velocity();
					break;
				}
				yield return null;
			}
		}

		private void EnsureRobotIsActive()
		{
			if (_rigidBody != null && playerTeamsContainer.OwnIdIsRegistered() && !_rigidBody.get_gameObject().get_activeInHierarchy())
			{
				if (!_alignmentRectifierBehaviour.IsIdle())
				{
					_alignmentRectifierBehaviour.GoIdle();
				}
				if (_alignmentRectifierEffectView != null)
				{
					_alignmentRectifierEffectView.Stop();
				}
				if (_alignmentRectifierTimerView != null)
				{
					_alignmentRectifierTimerView.Hide();
				}
			}
		}

		public void UnregisterEffects(AlignmentRectifierData alignmentRectifierData)
		{
			_alignmentRectifierData = null;
		}

		public void RegisterEffects(AlignmentRectifierData alignmentRectifierData)
		{
			_alignmentRectifierData = alignmentRectifierData;
			gameobjectPool.Preallocate(_alignmentRectifierData.particleEffectTemplate.get_name(), 1, (Func<GameObject>)(() => GameObjectPool.CreateGameObjectFromPrefab(_alignmentRectifierData.particleEffectTemplate)));
		}

		public void RegisterView(AlignmentRectifierTimerView alignmentRectifierTimerView)
		{
			_alignmentRectifierTimerView = alignmentRectifierTimerView;
			_alignmentRectifierTimerView.Hide();
		}

		public void RegisterView(AlignmentRectifierCooldownView alignmentRectifierCooldownView)
		{
			_alignmentRectifierCooldownView = alignmentRectifierCooldownView;
			battleHudStyleController.AddHud(this);
		}

		public void RegisterView(AlignmentRectifierHintNoviceView alignmentRectifierHintNoviceView)
		{
			_alignmentRectifierHintNoviceView = alignmentRectifierHintNoviceView;
		}

		public void UnregisterView(AlignmentRectifierTimerView alignmentRectifierTimerView)
		{
			_alignmentRectifierTimerView = null;
		}

		public void UnregisterView(AlignmentRectifierCooldownView alignmentRectifierCooldownView)
		{
			_alignmentRectifierCooldownView = null;
		}

		public void UnregisterView(AlignmentRectifierHintNoviceView alignmentRectifierHintNoviceView)
		{
			_alignmentRectifierHintNoviceView = null;
		}

		private void HandleonPlayerSelfDestructs(int playerId)
		{
			if (playerTeamsContainer.IsMe(TargetType.Player, playerId) && _state != FlippingManagerState.Destroyed && _state != 0)
			{
				ChangeStateToDestroyed();
			}
		}

		private void HandleOnMachineDestroyed(int playerId, int machineId, bool isMe)
		{
			if (isMe && _state != FlippingManagerState.Destroyed)
			{
				ChangeStateToDestroyed();
			}
		}

		public void HandleCharacterInput(InputCharacterData data)
		{
			if (_state != 0)
			{
				if (data.data[9] > 0f)
				{
					if (!_alignmentInputKeyDown)
					{
						_alignmentInputKeyDown = true;
						HandleInputJustPressed();
					}
					HandleInputHeldDown();
				}
				else
				{
					_alignmentInputKeyDown = false;
				}
			}
			FlippingManagerState state = _state;
			if (state == FlippingManagerState.AlignmentActive)
			{
				_alignmentRectifierBehaviour.ResetInputSignal();
				if (data.data[1] > 0f)
				{
					_alignmentRectifierBehaviour.ForwardSignal();
				}
				if (data.data[1] < 0f)
				{
					_alignmentRectifierBehaviour.BackSignal();
				}
				if (data.data[0] > 0f)
				{
					_alignmentRectifierBehaviour.RightSignal();
				}
				if (data.data[0] < 0f)
				{
					_alignmentRectifierBehaviour.LeftSignal();
				}
			}
		}

		private bool CheckStartConditions()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0063: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_007b: Unknown result type (might be due to invalid IL or missing references)
			//IL_007c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0086: Unknown result type (might be due to invalid IL or missing references)
			Vector3 boundingsphereCentreLocal = _alignmentRectifierBehaviour.GetBoundingsphereCentreLocal();
			float boundingsphereRadius = _alignmentRectifierBehaviour.GetBoundingsphereRadius();
			Vector3 worldCenterOfMass = _rigidBody.get_worldCenterOfMass();
			Vector3 val = _rigidBody.get_transform().TransformPoint(boundingsphereCentreLocal);
			float num = boundingsphereRadius * 1.2f;
			Ray val2 = default(Ray);
			if (worldCenterOfMass.y > val.y)
			{
				val2._002Ector(worldCenterOfMass, Vector3.get_down());
				float num2 = num;
				Vector3 val3 = worldCenterOfMass - val;
				num = num2 + val3.get_magnitude();
			}
			else
			{
				val2._002Ector(val, Vector3.get_down());
			}
			return Physics.Raycast(val2, num, GameLayers.INTERACTIVE_ENVIRONMENT_LAYER_MASK);
		}

		private void HandleInputJustPressed()
		{
			switch (_state)
			{
			case FlippingManagerState.Ready:
				if (CheckStartConditions())
				{
					ChangeStateToAlignmentActive();
				}
				else
				{
					_alignmentRectifierCooldownView.PlayErrorCooldownActiveSound(_alignmentRectifierData.audioCooldownErrorEffect, _alignmentRectifierData.audioCooldownEndEffect);
				}
				break;
			case FlippingManagerState.CoolingDown:
				_alignmentRectifierCooldownView.PlayErrorCooldownActiveSound(_alignmentRectifierData.audioCooldownErrorEffect, _alignmentRectifierData.audioCooldownEndEffect);
				break;
			case FlippingManagerState.AlignmentActive:
				_alignmentRectifierCooldownView.PlayErrorCooldownActiveSound(_alignmentRectifierData.audioCooldownErrorEffect, _alignmentRectifierData.audioCooldownEndEffect);
				break;
			}
		}

		private void HandleInputHeldDown()
		{
			FlippingManagerState state = _state;
			if (state == FlippingManagerState.Ready)
			{
				if (CheckStartConditions())
				{
					ChangeStateToAlignmentActive();
				}
				else
				{
					_alignmentRectifierCooldownView.PlayErrorCooldownActiveSound(_alignmentRectifierData.audioCooldownErrorEffect, _alignmentRectifierData.audioCooldownEndEffect);
				}
			}
		}

		private void ChangeStateToDestroyed()
		{
			FlippingManagerState state = _state;
			if (state == FlippingManagerState.AlignmentActive)
			{
				if (!_alignmentRectifierBehaviour.IsIdle())
				{
					_alignmentRectifierBehaviour.GoIdle();
				}
				_alignmentRectifierEffectView.Stop();
				_alignmentRectifierEffectView = null;
				_alignmentRectifierTimerView.Hide();
				HandleOnAlignmentEnded();
			}
			_alignmentRectifierCooldownView.Hide();
			_state = FlippingManagerState.Destroyed;
			_tick.Stop();
			_physicsTick.Stop();
		}

		private void ChangeStateToCoolDown()
		{
			if (!_alignmentRectifierBehaviour.IsIdle())
			{
				_alignmentRectifierBehaviour.GoIdle();
			}
			_alignmentRectifierTimerView.Hide();
			_cooldownTimer = 1f;
			_state = FlippingManagerState.CoolingDown;
		}

		private void ChangeStateToAlignmentActive()
		{
			_state = FlippingManagerState.AlignmentActive;
			_alignmentRectifierBehaviour.Activate(3f);
			_alignmentRectifierBehaviour.OnAlignmentComplete += HandleOnAlignmentBehaviourComplete;
			_alignmentTimer = 0f;
			EnableViews();
			HandleOnAlignmentStarted();
			eventManagerClient.SendEventToServerUnreliable(NetworkEvent.AlignmentRectifierStarted, new NetworkDependency());
		}

		private void EnableViews()
		{
			//IL_0096: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			_alignmentRectifierTimerView.SetDuration(3f);
			_alignmentRectifierTimerView.SetTime(0f);
			_alignmentRectifierTimerView.Show();
			GameObject val = gameobjectPool.Use(_alignmentRectifierData.particleEffectTemplate.get_name(), (Func<GameObject>)(() => GameObjectPool.CreateGameObjectFromPrefab(_alignmentRectifierData.particleEffectTemplate)));
			val.SetActive(true);
			_alignmentRectifierEffectView = val.GetComponent<AlignmentRectifierEffectView>();
			_alignmentRectifierEffectView.get_gameObject().get_transform().set_parent(_rigidBody.get_transform());
			_alignmentRectifierEffectView.get_gameObject().get_transform().set_localRotation(Quaternion.get_identity());
			_alignmentRectifierEffectView.get_gameObject().get_transform().set_localPosition(_alignmentRectifierBehaviour.GetBoundingsphereCentreLocal());
			_alignmentRectifierEffectView.SetSize(_alignmentRectifierBehaviour.GetBoundingsphereRadius());
			_alignmentRectifierEffectView.Play(gameobjectPool, _alignmentRectifierData);
			_alignmentRectifierHintNoviceView.DisableHint();
		}

		private void HandleOnAlignmentBehaviourComplete(float elapsedTime)
		{
			_alignmentRectifierBehaviour.OnAlignmentComplete -= HandleOnAlignmentBehaviourComplete;
			_alignmentRectifierBehaviour.GoIdle();
		}

		public void SetStyle(HudStyle style)
		{
			_alignmentRectifierCooldownView.SetStyle(style);
		}

		public Type[] AcceptedComponents()
		{
			return new Type[1]
			{
				typeof(IAlignmentRectifierPausable)
			};
		}

		public void Add(IComponent obj)
		{
			if (obj is IAlignmentRectifierPausable)
			{
				_componentsList.Add(obj as IAlignmentRectifierPausable);
			}
		}

		public void Remove(IComponent obj)
		{
			if (obj is IAlignmentRectifierPausable)
			{
				_componentsList.Remove(obj as IAlignmentRectifierPausable);
			}
		}

		private void HandleOnAlignmentStarted()
		{
			_localMachineRectifierNode.machineFunctionalsComponent.functionalsEnabled = false;
			for (int i = 0; i < _componentsList.get_Count(); i++)
			{
				_componentsList.get_Item(i).AlignmentRectifierStarted();
			}
			bool flag = true;
			_localAlignmentRectifierActivatedObservable.Dispatch(ref flag);
		}

		private void HandleOnAlignmentEnded()
		{
			_localMachineRectifierNode.machineFunctionalsComponent.functionalsEnabled = true;
			for (int i = 0; i < _componentsList.get_Count(); i++)
			{
				_componentsList.get_Item(i).AlignmentRectifierEnded();
			}
			bool flag = false;
			_localAlignmentRectifierActivatedObservable.Dispatch(ref flag);
		}

		protected override void Add(MachineRectifierNode node)
		{
			if (node.ownerComponent.ownedByMe)
			{
				_localMachineRectifierNode = node;
			}
		}

		protected override void Remove(MachineRectifierNode node)
		{
		}
	}
}
