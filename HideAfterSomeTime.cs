using UnityEngine;

public class HideAfterSomeTime : MonoBehaviour
{
	[SerializeField]
	private float timeInSeconds;

	private float _timer;

	public HideAfterSomeTime()
		: this()
	{
	}

	private void Update()
	{
		_timer += Time.get_deltaTime();
		if (_timer >= timeInSeconds)
		{
			_timer = 0f;
			this.get_gameObject().SetActive(false);
		}
	}
}
