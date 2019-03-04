using Svelto.IoC;
using UnityEngine;

internal sealed class GameTimer : MonoBehaviour, IInitialize
{
	public UILabel _timeLabel;

	[Inject]
	internal GameTimePresenter gameTimePresenter
	{
		private get;
		set;
	}

	public GameTimer()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		gameTimePresenter.SetView(this);
	}

	private void Start()
	{
		this.get_gameObject().SetActive(false);
	}

	public void SetTime(float seconds)
	{
		_timeLabel.set_text(FormatTime(seconds));
	}

	private string FormatTime(float seconds)
	{
		if (seconds > 0f)
		{
			int num = Mathf.FloorToInt(seconds / 60f);
			int num2 = Mathf.FloorToInt(seconds - (float)(num * 60));
			return num.ToString() + ":" + num2.ToString("D2");
		}
		return "0:00";
	}

	public void Show()
	{
		this.get_gameObject().SetActive(true);
	}

	public void Hide()
	{
		this.get_gameObject().SetActive(false);
	}
}
