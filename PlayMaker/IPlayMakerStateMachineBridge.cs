namespace PlayMaker
{
	internal interface IPlayMakerStateMachineBridge
	{
		void SetPlaymakerBoolVariable(string name, bool value);

		void SendPlaymakerEventToAllFSM(PlaymakerEventType eventType, string message);

		void RegisterFiniteStateMachine(PlayMakerFSM stateMachine, PlayMakerFSMConfig stateMachineConfig);

		void UnregisterFiniteStateMachine(PlayMakerFSM stateMachine);
	}
}
