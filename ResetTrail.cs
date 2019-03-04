using Svelto.Tasks;
using System.Collections;
using UnityEngine;

internal class ResetTrail : MonoBehaviour
{
	private TrailRenderer _trail;

	private float _trailTime;

	public ResetTrail()
		: this()
	{
	}

	private void Awake()
	{
		_trail = this.GetComponent<TrailRenderer>();
		_trailTime = _trail.get_time();
	}

	private void OnEnable()
	{
		TaskRunner.get_Instance().Run(Reset());
	}

	private IEnumerator Reset()
	{
		_trail.set_time(-1f);
		yield return null;
		_trail.set_time(_trailTime);
	}
}
