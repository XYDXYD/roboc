using Mothership;
using Mothership.GUI;
using Svelto.IoC;
using Svelto.Tasks.Enumerators;
using Svelto.UI.Comms.SignalChain;
using System;
using System.Collections;
using UnityEngine;
using Utility;

internal sealed class AwardedItemsScreen : MonoBehaviour, IInitialize, IChainListener
{
	private bool _isActive;

	public GameObject showCollectButton;

	public UILabel cubeCountLabel;

	public UILabel cubeNameLabel;

	public UISprite cubeSprite;

	public GameObject cubeContainer;

	public StatsHintPopupAreaImplementor hintPopupArea;

	public Animation animation;

	public AnimationClip screenAppearAnim;

	public AnimationClip screenDisappearAnim;

	public AnimationClip cubeAppearAnim;

	public AnimationClip cubeDisAppearAnim;

	public float cubeStayTime = 3f;

	[Inject]
	internal AwardedItemsController awardedItemsController
	{
		private get;
		set;
	}

	public AwardedItemsScreen()
		: this()
	{
	}

	void IInitialize.OnDependenciesInjected()
	{
		awardedItemsController.SetScreen(this);
	}

	private void Start()
	{
		this.get_gameObject().SetActive(false);
	}

	public void Show()
	{
		this.get_gameObject().SetActive(true);
		cubeContainer.SetActive(false);
		showCollectButton.SetActive(false);
		_isActive = true;
	}

	public bool Hide()
	{
		this.get_gameObject().SetActive(false);
		_isActive = false;
		return true;
	}

	public bool IsActive()
	{
		return _isActive;
	}

	public void ShowCollectButton()
	{
		showCollectButton.SetActive(true);
	}

	public void PlayCubeAppearAnimation(CubeTypeID cubeType, string cubeNameKey, string spriteName, uint cubeCount, Action HandleAnimationEndCallback)
	{
		cubeContainer.SetActive(true);
		SetSpriteAndLabelsForCubeType(cubeType, cubeNameKey, spriteName, cubeCount);
		StopAndPlayAnimation(cubeAppearAnim, HandleAnimationEndCallback, cubeStayTime);
	}

	public void PlayCubeDisappearAnimation(Action HandleAnimationEndCallback)
	{
		StopAndPlayAnimation(cubeDisAppearAnim, HandleAnimationEndCallback);
	}

	public void PlayScreenAppearAnimation(Action HandleAnimationEndCallback)
	{
		StopAndPlayAnimation(screenAppearAnim, HandleAnimationEndCallback);
	}

	private void StopAndPlayAnimation(AnimationClip clip, Action HandleAnimationEndCallback, float extraWaitTime = 0f)
	{
		animation.Stop();
		animation.Play(clip.get_name());
		this.StartCoroutine(WaitForAnimToFinish(clip.get_length() + extraWaitTime, HandleAnimationEndCallback));
	}

	private void SetSpriteAndLabelsForCubeType(CubeTypeID cubeType, string cubeNameKey, string spriteName, uint cubeCount)
	{
		hintPopupArea.cubeTypeId = cubeType;
		cubeSprite.set_spriteName(spriteName);
		cubeCountLabel.set_text($"x {cubeCount}");
		cubeNameLabel.set_text(StringTableBase<StringTable>.Instance.GetString(cubeNameKey));
	}

	private IEnumerator WaitForAnimToFinish(float waitTime, Action onAnimationEndCallback)
	{
		yield return (object)new WaitForSecondsEnumerator(waitTime);
		if (animation.get_isPlaying())
		{
			yield return null;
		}
		onAnimationEndCallback();
	}

	public void Listen(object message)
	{
		if (message is ButtonType && (ButtonType)message == ButtonType.Close)
		{
			StopAndPlayAnimation(screenDisappearAnim, OnScreenDisappearAnimComplete);
		}
	}

	private void OnScreenDisappearAnimComplete()
	{
		awardedItemsController.ScreenDisappearAnimationEnd();
		Console.Log("Awarded items controller: exit animation complete, screen will close now");
	}
}
