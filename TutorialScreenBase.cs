using Svelto.Factories;
using Svelto.IoC;
using System;
using UnityEngine;

internal abstract class TutorialScreenBase : MonoBehaviour
{
	private PlaymakerStateMachineIntegration _stateMachineIntegration;

	private GameObject _playmakerFSM;

	private const string FSM_END_STATE = "EndState";

	private const string GUI_READY_EVENT = "GUI_Ready";

	[Inject]
	internal IGameObjectFactory gameObjectFactory
	{
		get;
		set;
	}

	[Inject]
	internal IMonoBehaviourFactory monoBehaviourFactory
	{
		get;
		set;
	}

	protected TutorialScreenBase()
		: this()
	{
	}

	public abstract GuiScreens QueryScreenType();

	public void SetupStateMachine()
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		if (_playmakerFSM == null)
		{
			_playmakerFSM = gameObjectFactory.Build("TutorialFSM");
			DynamicallyAddStateMachineIntegration();
			Transform val = _playmakerFSM.get_transform();
			while (val != null && val.get_transform().get_parent() != null)
			{
				val = val.get_transform().get_parent();
			}
			_playmakerFSM.get_transform().set_parent(val);
			_playmakerFSM.get_transform().set_localScale(new Vector3(1f, 1f, 1f));
			_stateMachineIntegration = _playmakerFSM.GetComponent<PlaymakerStateMachineIntegration>();
		}
	}

	private void DynamicallyAddStateMachineIntegration()
	{
		monoBehaviourFactory.Build<PlaymakerStateMachineIntegration>((Func<PlaymakerStateMachineIntegration>)delegate
		{
			PlaymakerStateMachineIntegration playmakerStateMachineIntegration = _playmakerFSM.AddComponent<PlaymakerStateMachineIntegration>();
			playmakerStateMachineIntegration.ScreenControllerTarget = QueryScreenType();
			playmakerStateMachineIntegration.StateMachineConfig.GUIReadyEvent = "GUI_Ready";
			playmakerStateMachineIntegration.StateMachineConfig.EndState = "EndState";
			return playmakerStateMachineIntegration;
		});
	}

	public void ShowScreenBase()
	{
		if (_stateMachineIntegration == null)
		{
			SetupStateMachine();
		}
		_stateMachineIntegration.BeginStateMachine();
	}

	public void HideScreenBase()
	{
		Object.Destroy(_stateMachineIntegration.get_gameObject());
	}
}
