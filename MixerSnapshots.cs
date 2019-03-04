using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MixerSnapshots : MonoBehaviour
{
	public AudioMixerSnapshot Normal;

	public AudioMixerSnapshot LowPass;

	public float range = 10f;

	private bool lowPassIsActive;

	public MixerSnapshots()
		: this()
	{
	}

	private void Awake()
	{
		TaskRunner.get_Instance().Run((Func<IEnumerator>)Tick);
	}

	private IEnumerator Tick()
	{
		while (true)
		{
			yield return null;
			if (!lowPassIsActive && this.get_gameObject().get_activeInHierarchy() && Vector3.SqrMagnitude(this.get_transform().get_position() - Camera.get_main().get_transform().get_position()) <= range * range)
			{
				lowPassIsActive = true;
				LowPass.TransitionTo(0.5f);
			}
			else if (lowPassIsActive && (!this.get_gameObject().get_activeInHierarchy() || Vector3.SqrMagnitude(this.get_transform().get_position() - Camera.get_main().get_transform().get_position()) > range * range))
			{
				lowPassIsActive = false;
				Normal.TransitionTo(0.5f);
			}
		}
	}
}
