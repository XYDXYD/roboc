using Svelto.IoC;
using Svelto.Tasks.Enumerators;
using System.Collections;
using UnityEngine;

internal class DevMessageView : MonoBehaviour, IInitialize
{
	public UILabel message;

	private bool _isActive;

	private float _remainingTime;

	private float _startShowTime;

	[Inject]
	internal DevMessagePresenter devMessagePresenter
	{
		private get;
		set;
	}

	public DevMessageView()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		devMessagePresenter.view = this;
	}

	private void OnEnable()
	{
		if (_remainingTime > 0f)
		{
			this.StartCoroutine(ShowMessageAndHide(_remainingTime));
			_remainingTime = 0f;
		}
	}

	private void OnDisable()
	{
		if (_isActive)
		{
			_isActive = false;
			_remainingTime = Time.get_time() - _startShowTime;
		}
	}

	public void SetMessage(string text, float displayTime)
	{
		message.set_text(text);
		this.get_gameObject().SetActive(true);
		this.StartCoroutine(ShowMessageAndHide(displayTime));
	}

	private IEnumerator ShowMessageAndHide(float timeBeforeHide)
	{
		_isActive = true;
		_startShowTime = Time.get_time();
		yield return (object)new WaitForSecondsEnumerator(timeBeforeHide);
		this.get_gameObject().SetActive(false);
		_isActive = false;
	}
}
