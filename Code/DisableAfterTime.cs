using UnityEngine;

internal sealed class DisableAfterTime : MonoBehaviour
{
	public float life;

	private float _creationTime;

	public DisableAfterTime()
		: this()
	{
	}

	private void OnEnable()
	{
		_creationTime = life;
	}

	private void Update()
	{
		_creationTime -= Time.get_deltaTime();
		if (_creationTime <= 0f)
		{
			this.get_gameObject().SetActive(false);
		}
	}
}
