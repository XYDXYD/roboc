using System;
using UnityEngine;

namespace PlayMaker
{
	[Serializable]
	internal struct PlayMakerFSMConfig
	{
		[Tooltip("an event fired into the state machine when the GUI system is ready to activate this state machine. if empty, no event is sent.")]
		public string GUIReadyEvent;

		[Tooltip("when the FSM reaches this state, the GUI will consider that it has finished, and can dismiss the screen it is contained in.")]
		public string EndState;
	}
}
