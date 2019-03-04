using RCNetwork.Events;
using Svelto.ES.Legacy;
using Svelto.IoC;
using Svelto.Observer;
using System;
using UnityEngine;

namespace Simulation
{
	internal sealed class InputSenderClient : IHandleCharacterInput, IInitialize, IInputComponent, IComponent
	{
		private const float INPUT_SEND_INTERVAL = 0.125f;

		private const float MOUSE_INPUT_SEND_INTERVAL = 15f;

		private bool _lastJump;

		private bool _lastCrouch;

		private int _lastHorizontalInt;

		private int _lastVerticalInt;

		private bool _lastToggleLights;

		private bool _lastStrafeLeft;

		private bool _lastStrafeRight;

		private bool _inputSendPending;

		private bool _mouseInputPending;

		private float _lastSendTime;

		private bool _allowMovement;

		[Inject]
		public INetworkEventManagerClient networkEventManager
		{
			private get;
			set;
		}

		[Inject]
		public PlayerTeamsContainer playerTeamsContainer
		{
			private get;
			set;
		}

		[Inject]
		public AllowMovementObserver allowMovementObserver
		{
			private get;
			set;
		}

		public unsafe void OnDependenciesInjected()
		{
			allowMovementObserver.AddAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public unsafe void Dispose()
		{
			allowMovementObserver.RemoveAction(new ObserverAction<bool>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		}

		public void HandleCharacterInput(InputCharacterData data)
		{
			if (_allowMovement && playerTeamsContainer.OwnIdIsRegistered())
			{
				float num = data.data[0];
				float num2 = data.data[1];
				bool flag = data.data[2] != 0f;
				bool flag2 = data.data[3] != 0f;
				float num3 = data.data[4];
				float num4 = data.data[5];
				bool flag3 = data.data[6] != 0f;
				bool flag4 = data.data[7] != 0f;
				bool flag5 = data.data[28] != 0f;
				bool flag6 = data.data[29] != 0f;
				int num5 = 0;
				int num6 = 0;
				if (num > 0f)
				{
					num5 = 1;
				}
				else if (num < 0f)
				{
					num5 = -1;
				}
				if (num2 > 0f)
				{
					num6 = 1;
				}
				else if (num2 < 0f)
				{
					num6 = -1;
				}
				if (num5 != _lastHorizontalInt)
				{
					_inputSendPending = true;
				}
				else if (num6 != _lastVerticalInt)
				{
					_inputSendPending = true;
				}
				else if (flag != _lastJump)
				{
					_inputSendPending = true;
				}
				else if (flag2 != _lastCrouch)
				{
					_inputSendPending = true;
				}
				else if (flag5 != _lastStrafeLeft)
				{
					_inputSendPending = true;
				}
				else if (flag6 != _lastStrafeRight)
				{
					_inputSendPending = true;
				}
				if (num3 != 0f || num4 != 0f || flag3 || flag4)
				{
					_mouseInputPending = true;
				}
				if (_mouseInputPending && Time.get_time() - _lastSendTime > 15f)
				{
					_inputSendPending = true;
				}
				if (_inputSendPending && Time.get_time() - _lastSendTime > 0.125f)
				{
					_inputSendPending = false;
					_mouseInputPending = false;
					_lastSendTime = Time.get_time();
					PlayerInputChangedDependency dependency = new PlayerInputChangedDependency(playerTeamsContainer.localPlayerId, new LatestState(num, num2, flag, flag2, flag5, flag6));
					networkEventManager.SendEventToServerExperimental(NetworkEvent.OnPlayerInputChanged, dependency);
					_lastJump = flag;
					_lastCrouch = flag2;
					_lastHorizontalInt = num5;
					_lastVerticalInt = num6;
					_lastStrafeLeft = flag5;
					_lastStrafeRight = flag6;
				}
			}
		}

		private void AllowMovementChanged(ref bool allowMovement)
		{
			_allowMovement = allowMovement;
		}
	}
}
