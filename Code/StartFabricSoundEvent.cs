using Fabric;
using UnityEngine;

internal class StartFabricSoundEvent : MonoBehaviour
{
	public string soundEventName;

	public string parameterName1 = string.Empty;

	public float parameterValue1;

	public string parameterName2 = string.Empty;

	public float parameterValue2;

	public bool repeatSound;

	public float repeatSoundEvery = 1f;

	private float lastPlayTime;

	public StartFabricSoundEvent()
		: this()
	{
	}

	private void Start()
	{
		PlaySound();
	}

	private void PlaySound()
	{
		EventManager.get_Instance().PostEvent(soundEventName, 1, (object)null, this.get_gameObject());
		EventManager.get_Instance().PostEvent(soundEventName, 0, (object)null, this.get_gameObject());
		if (parameterName1 != string.Empty)
		{
			EventManager.get_Instance().SetParameter(soundEventName, parameterName1, parameterValue1, this.get_gameObject());
		}
		if (parameterName2 != string.Empty)
		{
			EventManager.get_Instance().SetParameter(soundEventName, parameterName2, parameterValue2, this.get_gameObject());
		}
		lastPlayTime = Time.get_time();
	}

	private void Update()
	{
		if (repeatSound && Time.get_time() > lastPlayTime + repeatSoundEvery)
		{
			PlaySound();
		}
	}
}
