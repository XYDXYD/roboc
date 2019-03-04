using Svelto.IoC;
using UnityEngine;

internal class PopupMessageView : MonoBehaviour
{
	public UILabel textLabelWarning;

	public UILabel textLabelTutorial;

	public string animationIn;

	public string animationOut;

	public GameObject warningGameobject;

	public GameObject tutorialGameObject;

	private Animation _animation;

	[Inject]
	internal PopupMessagePresenter presenter
	{
		private get;
		set;
	}

	public PopupMessageView()
		: this()
	{
	}

	private void Awake()
	{
		_animation = this.GetComponent<Animation>();
	}

	private void Start()
	{
		presenter.SetView(this);
	}

	private void OnDisable()
	{
		_animation.Stop();
	}

	internal void Show()
	{
		this.get_gameObject().SetActive(true);
	}

	internal void PlayAnimationIn()
	{
		_animation.Stop();
		_animation.Blend(animationIn);
	}

	internal void PlayAnimationOut()
	{
		_animation.Stop();
		_animation.Blend(animationOut);
	}

	internal void Hide()
	{
		this.get_gameObject().SetActive(false);
	}

	internal void SetupMessage(string text, PopupMessageCategory category)
	{
		if (category == PopupMessageCategory.Warning)
		{
			textLabelWarning.set_text(text);
		}
		else
		{
			textLabelTutorial.set_text(text);
		}
		warningGameobject.SetActive(category == PopupMessageCategory.Warning);
		tutorialGameObject.SetActive(category == PopupMessageCategory.Tutorial);
	}

	internal bool IsActive()
	{
		return this.get_gameObject().get_activeInHierarchy();
	}
}
