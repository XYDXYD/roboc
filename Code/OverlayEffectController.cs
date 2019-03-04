using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Simulation;
using Svelto.IoC;
using System;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

internal class OverlayEffectController : MonoBehaviour
{
	public ScreenOverlay effect;

	public float minIntensity = 0.1f;

	public float maxIntensity = 0.5f;

	public float fadeInTime = 0.2f;

	public float fadeOutTime = 1.8f;

	public float _maxDamage = 2000f;

	private float _intensity;

	private Sequence _activeTween;

	[Inject]
	internal DestructionReporter destructionReporter
	{
		private get;
		set;
	}

	public OverlayEffectController()
		: this()
	{
	}

	private void Start()
	{
		if (effect == null)
		{
			effect = this.GetComponent<ScreenOverlay>();
		}
		destructionReporter.OnPlayerDamageApplied += HandleOnPlayerDamageApplied;
	}

	private void OnDestroy()
	{
		destructionReporter.OnPlayerDamageApplied -= HandleOnPlayerDamageApplied;
		if (_activeTween != null)
		{
			TweenExtensions.Kill(_activeTween, false);
		}
	}

	private void HandleOnPlayerDamageApplied(DestructionData data)
	{
		if (data.targetIsMe)
		{
			float num = 0f;
			for (int i = 0; i < data.damagedCubes.get_Count(); i++)
			{
				num += (float)data.damagedCubes.get_Item(i).lastDamageApplied;
			}
			for (int j = 0; j < data.destroyedCubes.get_Count(); j++)
			{
				num += (float)data.destroyedCubes.get_Item(j).lastDamageApplied;
			}
			UpdateEffect(num);
		}
	}

	private void UpdateEffect(float damage)
	{
		damage /= _maxDamage;
		damage -= 1f - maxIntensity;
		_intensity = Mathf.Clamp(damage, minIntensity, maxIntensity);
		if (_intensity > effect.intensity)
		{
			PlayTween();
		}
	}

	private unsafe void PlayTween()
	{
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		if (_activeTween != null)
		{
			TweenExtensions.Kill(_activeTween, false);
		}
		effect.set_enabled(true);
		_activeTween = DOTween.Sequence();
		TweenSettingsExtensions.OnComplete<Sequence>(_activeTween, new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
		TweenSettingsExtensions.Append(_activeTween, TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), _intensity, fadeInTime), 3));
		TweenSettingsExtensions.Append(_activeTween, TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), 0f, fadeOutTime), 2));
	}
}
