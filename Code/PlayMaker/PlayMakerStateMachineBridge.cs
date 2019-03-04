using Svelto.Context;
using Svelto.DataStructures;

namespace PlayMaker
{
	internal sealed class PlayMakerStateMachineBridge : IPlayMakerStateMachineBridge, IWaitForFrameworkDestruction
	{
		private struct PlayMakerStateMachineInfo
		{
			public PlayMakerFSM fsm;

			public PlayMakerFSMConfig config;
		}

		private FasterList<PlayMakerStateMachineInfo> _registeredStateMachines = new FasterList<PlayMakerStateMachineInfo>();

		void IWaitForFrameworkDestruction.OnFrameworkDestroyed()
		{
			_registeredStateMachines = null;
		}

		public void RegisterFiniteStateMachine(PlayMakerFSM stateMachine, PlayMakerFSMConfig configuration)
		{
			PlayMakerStateMachineInfo playMakerStateMachineInfo = default(PlayMakerStateMachineInfo);
			playMakerStateMachineInfo.fsm = stateMachine;
			playMakerStateMachineInfo.config = configuration;
			_registeredStateMachines.Add(playMakerStateMachineInfo);
		}

		public void UnregisterFiniteStateMachine(PlayMakerFSM stateMachine)
		{
			if (_registeredStateMachines == null)
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < _registeredStateMachines.get_Count())
				{
					PlayMakerStateMachineInfo playMakerStateMachineInfo = _registeredStateMachines.get_Item(num);
					if (playMakerStateMachineInfo.fsm == stateMachine)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			_registeredStateMachines.UnorderedRemoveAt(num);
		}

		public void SendPlaymakerEventToAllFSM(PlaymakerEventType eventType, string eventName)
		{
			string text = FastConcatUtility.FastConcat(eventType.ToString(), "_", eventName);
			for (int i = 0; i < _registeredStateMachines.get_Count(); i++)
			{
				PlayMakerStateMachineInfo playMakerStateMachineInfo = _registeredStateMachines.get_Item(i);
				playMakerStateMachineInfo.fsm.SendEvent(text);
			}
		}

		public void SetPlaymakerBoolVariable(string name, bool value)
		{
			for (int i = 0; i < _registeredStateMachines.get_Count(); i++)
			{
				PlayMakerStateMachineInfo playMakerStateMachineInfo = _registeredStateMachines.get_Item(i);
				playMakerStateMachineInfo.fsm.get_FsmVariables().GetFsmBool(name).set_Value(value);
			}
		}
	}
}
