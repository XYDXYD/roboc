using UnityEngine;

internal class PingIndicatorBehaviour : MonoBehaviour
{
	public float life;

	private float _timer;

	public PingIndicatorBehaviour()
		: this()
	{
	}

	private void Update()
	{
		_timer += Time.get_deltaTime();
		if (_timer >= life)
		{
			Object.Destroy(this.get_gameObject());
		}
	}
}
