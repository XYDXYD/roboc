using Fabric;
using Svelto.IoC;
using UnityEngine;

internal sealed class TeamBaseAudioBehaviour : MonoBehaviour
{
	private int _sectionsCompleted;

	private TeamBaseBehaviour _teamBaseBehaviour;

	[Inject]
	internal TeamBaseProgressDispatcher teamBaseProgressDispatcher
	{
		private get;
		set;
	}

	public TeamBaseAudioBehaviour()
		: this()
	{
	}

	public void SetTeamBaseBehaviour(TeamBaseBehaviour teamBaseBehaviour)
	{
		_teamBaseBehaviour = teamBaseBehaviour;
		if (teamBaseProgressDispatcher != null)
		{
			teamBaseProgressDispatcher.RegisterCaptureStarted(_teamBaseBehaviour.teamId, OnCaptureStarted);
			teamBaseProgressDispatcher.RegisterCaptureStopped(_teamBaseBehaviour.teamId, OnCaptureStopped);
			teamBaseProgressDispatcher.RegisterSectionComplete(_teamBaseBehaviour.teamId, OnSectionComplete);
			teamBaseProgressDispatcher.RegisterFinalSectionComplete(_teamBaseBehaviour.teamId, OnFinalSectionComplete);
		}
	}

	private void OnDestroy()
	{
		if (teamBaseProgressDispatcher != null && _teamBaseBehaviour != null)
		{
			teamBaseProgressDispatcher.UnRegisterCaptureStarted(_teamBaseBehaviour.teamId, OnCaptureStarted);
			teamBaseProgressDispatcher.UnRegisterCaptureStopped(_teamBaseBehaviour.teamId, OnCaptureStopped);
			teamBaseProgressDispatcher.UnRegisterSectionComplete(_teamBaseBehaviour.teamId, OnSectionComplete);
			teamBaseProgressDispatcher.UnRegisterFinalSectionComplete(_teamBaseBehaviour.teamId, OnFinalSectionComplete);
		}
	}

	private void OnCaptureStarted()
	{
		switch (_sectionsCompleted)
		{
		case 0:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start01", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop01", 0, this.get_gameObject());
			break;
		case 1:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start02", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop02", 0, this.get_gameObject());
			break;
		case 2:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start03", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop03", 0, this.get_gameObject());
			break;
		case 3:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start04", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop04", 0, this.get_gameObject());
			break;
		}
	}

	private void OnCaptureStopped()
	{
		switch (_sectionsCompleted)
		{
		case 0:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_stop01", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop01", 1, this.get_gameObject());
			break;
		case 1:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_stop02", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop02", 1, this.get_gameObject());
			break;
		case 2:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_stop03", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop03", 1, this.get_gameObject());
			break;
		case 3:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_stop04", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop04", 1, this.get_gameObject());
			break;
		}
	}

	private void OnSectionComplete()
	{
		_sectionsCompleted++;
		switch (_sectionsCompleted)
		{
		case 0:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start01", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop01", 0, this.get_gameObject());
			break;
		case 1:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start02", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop02", 0, this.get_gameObject());
			break;
		case 2:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start03", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop03", 0, this.get_gameObject());
			break;
		case 3:
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start04", 0, this.get_gameObject());
			EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop04", 0, this.get_gameObject());
			break;
		}
	}

	private void OnFinalSectionComplete()
	{
		_sectionsCompleted++;
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_start05", 0, this.get_gameObject());
		EventManager.get_Instance().PostEvent("KUB_DEMO_fabric_Ingame_MiningBase_power_loop05", 0, this.get_gameObject());
	}
}
