using Fabric;
using Svelto.ES.Legacy;
using UnityEngine;

internal sealed class PlayAudioOnWorldSwitching : MonoBehaviour, IRunOnWorldSwitching, IComponent
{
	public bool fadeIn;

	public AudioFabricGameEvents soundEvent;

	public int priority = 2;

	public float duration;

	public bool FadeIn => fadeIn;

	public int Priority => priority;

	public float Duration => duration;

	public PlayAudioOnWorldSwitching()
		: this()
	{
	}

	public void Execute(WorldSwitchMode currentMode)
	{
		if (!FadeIn)
		{
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(soundEvent), 0, (object)null, null);
		}
		string text = (!fadeIn) ? "Pause" : "Normal";
		EventManager.get_Instance().PostEvent("Main", 28, (object)text);
	}
}
