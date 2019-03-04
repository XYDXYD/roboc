using HutongGames.PlayMaker;
using UnityEngine;

namespace PlayMaker
{
	public abstract class PlayMakerCustomNodeBase : FsmStateAction
	{
		private PlayMakerScreenControllerRequestsSetup _requestsSetup;

		private PlayMakerScreenControllerCommandsSetup _commandsSetup;

		internal PlayMakerScreenControllerRequestsSetup RequestsHandler => _requestsSetup;

		internal PlayMakerScreenControllerCommandsSetup CommandsHandler => _commandsSetup;

		protected PlayMakerCustomNodeBase()
			: this()
		{
		}

		private void DiscoverRequestsSetupComponent()
		{
			GameObject gameObject = this.get_Fsm().get_GameObject();
			PlaymakerStateMachineIntegration component = gameObject.GetComponent<PlaymakerStateMachineIntegration>();
			_requestsSetup = component.GetRequestsSetup();
			_commandsSetup = component.GetCommandsSetup();
		}

		public abstract void OnBegin();

		public sealed override void OnEnter()
		{
			DiscoverRequestsSetupComponent();
			OnBegin();
		}
	}
}
