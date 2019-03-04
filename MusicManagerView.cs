using Fabric;
using Simulation;
using Svelto.IoC;
using UnityEngine;

internal sealed class MusicManagerView : MonoBehaviour, IInitialize
{
	[SerializeField]
	private string MusicEventName = "MUS_SpitzerDam";

	[SerializeField]
	private string MainLoopName = "Loop";

	[SerializeField]
	private string SecondaryLoopName = "Equaliser";

	[SerializeField]
	private string EndGameLoopName = "EndGame";

	[SerializeField]
	private string EndGameEventName = "GUI_95";

	[SerializeField]
	private string VotingScreenLoopName = "MUS_Tutorial_Intro";

	[Inject]
	internal IMusicManager musicManagerPresenter
	{
		private get;
		set;
	}

	public MusicManagerView()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		musicManagerPresenter.SetView(this);
	}

	internal void StartMusic()
	{
		EventManager.get_Instance().PostEvent(MusicEventName, 0, (object)null, this.get_gameObject());
	}

	internal void SwitchToMainLoop()
	{
		EventManager.get_Instance().PostEvent(MusicEventName, 6, (object)MainLoopName, this.get_gameObject());
	}

	internal void SwitchToSecondaryLoop()
	{
		EventManager.get_Instance().PostEvent(MusicEventName, 6, (object)SecondaryLoopName, this.get_gameObject());
	}

	internal void SwitchToEndGameLoop()
	{
		EventManager.get_Instance().PostEvent(MusicEventName, 6, (object)EndGameLoopName, this.get_gameObject());
		EventManager.get_Instance().PostEvent(EndGameEventName, 0, (object)null, this.get_gameObject());
	}

	internal void SwitchToVotingScreenLoop()
	{
		EventManager.get_Instance().PostEvent(MusicEventName, 21, this.get_gameObject());
		EventManager.get_Instance().PostEvent(VotingScreenLoopName, 0, (object)null, this.get_gameObject());
	}
}
