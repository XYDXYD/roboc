using PlayMaker;
using Svelto.IoC;
using UnityEngine;

[RequireComponent(typeof(PlayMakerFSM))]
internal sealed class PlaymakerStateMachineIntegration : MonoBehaviour, IInitialize
{
	[Tooltip("set to None if this state machine does not bind to a specific screen")]
	[SerializeField]
	public GuiScreens ScreenControllerTarget;

	[SerializeField]
	public PlayMakerFSMConfig StateMachineConfig;

	private PlayMakerScreenControllerCommandsSetup _screenControllerCommandsSetup;

	private PlayMakerScreenControllerRequestsSetup _screenControllerRequestsSetup;

	private PlayMakerFSM _stateMachine;

	[Inject]
	internal IPlayMakerStateMachineBridge playMakerStateMachineBridge
	{
		private get;
		set;
	}

	[Inject]
	internal IGUIInputController guiInputController
	{
		private get;
		set;
	}

	public PlaymakerStateMachineIntegration()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		_stateMachine = this.GetComponent<PlayMakerFSM>();
		if (ScreenControllerTarget != GuiScreens.Undefined)
		{
			_screenControllerRequestsSetup = new PlayMakerScreenControllerRequestsSetup();
			_screenControllerCommandsSetup = new PlayMakerScreenControllerCommandsSetup();
			IGUIDisplay controllerForScreen = guiInputController.GetControllerForScreen(ScreenControllerTarget);
			IPlaymakerDataProvider dataProvider = controllerForScreen as IPlaymakerDataProvider;
			IPlaymakerCommandProvider commandProvider = controllerForScreen as IPlaymakerCommandProvider;
			_screenControllerRequestsSetup.Initialise(dataProvider);
			_screenControllerCommandsSetup.Initialise(commandProvider);
		}
		playMakerStateMachineBridge.RegisterFiniteStateMachine(_stateMachine, StateMachineConfig);
	}

	public PlayMakerScreenControllerCommandsSetup GetCommandsSetup()
	{
		return _screenControllerCommandsSetup;
	}

	public PlayMakerScreenControllerRequestsSetup GetRequestsSetup()
	{
		return _screenControllerRequestsSetup;
	}

	public void BeginStateMachine()
	{
		this.get_gameObject().SetActive(true);
		_stateMachine.SendEvent(StateMachineConfig.GUIReadyEvent);
	}

	private void OnDestroy()
	{
		playMakerStateMachineBridge.UnregisterFiniteStateMachine(_stateMachine);
	}
}
