using DG.Tweening;
using Fabric;
using Mothership;
using Svelto.IoC;
using System;
using UnityEngine;

internal sealed class DisplayCubePainter : MonoBehaviour
{
	public GameObject paintGunObject;

	public Transform colorWheelTransform;

	public GameObject premiumLockSprite;

	public float maxSingleSlotScrollTime = 0.1f;

	public float maxMultipleSlotScrollTime = 0.3f;

	public float maxWheelJiggleTime = 0.1f;

	private int _currentVisualSlot;

	private float _degreesPerColorSlot;

	private float _amountToScroll;

	private float _timeToScroll;

	private Sequence _jiggleSequence;

	private const float WHEEL_JIGGLE_DEGREES = 6f;

	private Renderer[] _paintGunRenderers;

	[Inject]
	internal ICubeHolder cubeHolder
	{
		private get;
		set;
	}

	[Inject]
	internal PaintToolPresenter paintToolPresenter
	{
		private get;
		set;
	}

	[Inject]
	internal PremiumMembership premiumMembership
	{
		private get;
		set;
	}

	public ColorEffects colorBeam
	{
		get;
		private set;
	}

	public DisplayCubePainter()
		: this()
	{
	}

	private void Start()
	{
		InitialisePaintEffects();
		paintToolPresenter.displayCubePainter = this;
		paintToolPresenter.PaintWheelScrolled += OnPaintWheelScrolled;
		paintToolPresenter.PaintWheelBlockedFromScrolling += OnPaintWheelBlockedFromScrolling;
		cubeHolder.OnColorChanged += OnColorChanged;
		premiumMembership.onSubscriptionActivated += OnPremiumActivated;
		premiumMembership.onSubscriptionExpired += OnPremiumExpired;
		ShowPremiumLockSprite(premiumMembership.hasSubscription);
		OnColorChanged(cubeHolder.currentColor);
		_degreesPerColorSlot = 360f / (float)paintToolPresenter.maxColours;
	}

	private void Destroy()
	{
		cubeHolder.OnColorChanged -= OnColorChanged;
		paintToolPresenter.PaintWheelScrolled -= OnPaintWheelScrolled;
		paintToolPresenter.PaintWheelBlockedFromScrolling -= OnPaintWheelBlockedFromScrolling;
		premiumMembership.onSubscriptionActivated -= OnPremiumActivated;
		premiumMembership.onSubscriptionExpired -= OnPremiumExpired;
	}

	private void Update()
	{
		if (_timeToScroll > 0f)
		{
			float num = Time.get_deltaTime();
			if (_timeToScroll - num < 0f)
			{
				num = _timeToScroll;
			}
			float num2 = _amountToScroll / _timeToScroll * num;
			colorWheelTransform.Rotate(0f, 0f, num2, 1);
			_amountToScroll -= num2;
			_timeToScroll -= num;
		}
	}

	public void PlayImpactEffect(Vector3 hitPoint)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		colorBeam.PlayImpactEffect(hitPoint);
	}

	public bool IsActive()
	{
		return this.get_gameObject().get_activeInHierarchy();
	}

	private void InitialisePaintEffects()
	{
		colorBeam = paintGunObject.GetComponentInChildren<ColorEffects>();
	}

	private void OnColorChanged(PaletteColor newColor)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		colorBeam.UpdateColor(newColor);
		if (_paintGunRenderers == null)
		{
			_paintGunRenderers = this.GetComponentsInChildren<Renderer>();
		}
		for (int i = 0; i < _paintGunRenderers.Length; i++)
		{
			for (int j = 0; j < _paintGunRenderers[i].get_materials().Length; j++)
			{
				Material val = _paintGunRenderers[i].get_materials()[j];
				if (val.HasProperty("_Colorable"))
				{
					val.set_color(Color32.op_Implicit(newColor.diffuse));
				}
			}
		}
	}

	private void OnPaintWheelScrolled(int newVisualSlotNum)
	{
		if (_jiggleSequence != null)
		{
			TweenExtensions.Complete(_jiggleSequence);
		}
		int num = newVisualSlotNum - _currentVisualSlot;
		_currentVisualSlot = newVisualSlotNum;
		if ((double)Mathf.Abs(num) > (double)paintToolPresenter.maxColours * 0.5)
		{
			if (num > 0)
			{
				num = -paintToolPresenter.maxColours + num;
			}
			else if (num < 0)
			{
				num = paintToolPresenter.maxColours + num;
			}
		}
		_amountToScroll += (float)num * _degreesPerColorSlot;
		float num2 = (float)Mathf.Abs(num) * maxSingleSlotScrollTime;
		_timeToScroll += num2;
		_timeToScroll = ((!(_timeToScroll < maxMultipleSlotScrollTime)) ? maxMultipleSlotScrollTime : _timeToScroll);
		EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Select));
	}

	private unsafe void OnPaintWheelBlockedFromScrolling(int visualSlotNum)
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Expected O, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		if (_timeToScroll <= 0f && _jiggleSequence == null)
		{
			Quaternion localRotation = colorWheelTransform.get_localRotation();
			Vector3 eulerAngles = localRotation.get_eulerAngles();
			Vector3 val = eulerAngles;
			if (visualSlotNum == 0)
			{
				val.z -= 6f;
			}
			else
			{
				val.z += 6f;
			}
			_jiggleSequence = DOTween.Sequence();
			TweenSettingsExtensions.OnComplete<Sequence>(_jiggleSequence, new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			TweenSettingsExtensions.Append(_jiggleSequence, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(colorWheelTransform.get_transform(), val, maxWheelJiggleTime * 0.5f, 0), 1));
			TweenSettingsExtensions.Append(_jiggleSequence, TweenSettingsExtensions.SetEase<Tweener>(ShortcutExtensions.DOLocalRotate(colorWheelTransform.get_transform(), eulerAngles, maxWheelJiggleTime * 0.5f, 0), 1));
			TweenSettingsExtensions.SetAutoKill<Sequence>(_jiggleSequence, true);
			TweenExtensions.Play<Sequence>(_jiggleSequence);
			EventManager.get_Instance().PostEvent(AudioFabricEvent.Name(AudioFabricGameEvents.Color_Locked));
		}
	}

	private void OnPremiumActivated(TimeSpan t)
	{
		premiumLockSprite.SetActive(false);
	}

	private void OnPremiumExpired()
	{
		ShowPremiumLockSprite(hasPremium: true);
	}

	private void ShowPremiumLockSprite(bool hasPremium)
	{
		premiumLockSprite.SetActive(!hasPremium);
	}
}
