using UnityEngine;

internal sealed class PanelFlasher : MonoBehaviour
{
	public UILabel label;

	public float flashTime = 1f;

	private float _hideUserNotFoundTimer;

	private bool _persist;

	public PanelFlasher()
		: this()
	{
	}

	private void Start()
	{
		if (!_persist && _hideUserNotFoundTimer == 0f)
		{
			SetActive(active: false);
		}
	}

	private void Update()
	{
		if (!_persist && _hideUserNotFoundTimer > 0f)
		{
			_hideUserNotFoundTimer -= Time.get_deltaTime();
			if (_hideUserNotFoundTimer <= 0f)
			{
				SetActive(active: false);
			}
		}
	}

	public void Show(string s, bool persist = false)
	{
		if (label != null)
		{
			label.set_text(s);
		}
		SetActive(active: true);
		if (!persist)
		{
			_hideUserNotFoundTimer = flashTime;
		}
		_persist = persist;
	}

	private void SetActive(bool active)
	{
		this.set_enabled(active);
		this.get_gameObject().SetActive(active);
	}
}
