using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Svelto.Tasks;
using System;
using System.Collections;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal sealed class HUDBattleArenaWidget : MonoBehaviour
	{
		public UILabel percentLabel;

		public UILabel extraInfoLabel;

		public UISprite slider;

		public GameObject sliderContainer;

		public UISprite sliderHighlight;

		public UISprite pulser;

		public UIWidget[] primaryColorWidgets;

		public UIWidget[] secondaryColorWidgets;

		public GameObject[] particleAnimations;

		public Transform animatedLabel;

		public Animation animation;

		public AnimatedAlpha gameEndSprite;

		public float sliderAnimationDuration = 0.2f;

		public float healthPercentStartLabelAnimation = 0.95f;

		public float[] basePulseAnimationSpeed;

		public float[] gameEndSpeed = new float[5]
		{
			0.4f,
			0.35f,
			0.3f,
			0.2f,
			0.2f
		};

		public float alphaEnd = 0.59f;

		public Vector3 startScaleValue = new Vector3(1.3f, 1.3f, 1.3f);

		public Vector3 endScaleValue = new Vector3(1f, 1f, 1f);

		public Vector3 bounceStartScaleValue = new Vector3(0.9f, 0.9f, 0.9f);

		public Vector3 startLabelAnimationScaleValue = new Vector3(3f, 3f, 3f);

		private Sequence _pulseSequence;

		private Sequence _colourChangeSequence;

		private Sequence _bounceSequence;

		private Sequence _disappearSequence;

		private Sequence _endSequence;

		private Action _onParticleAnimationEnd;

		private int _playingParticleAnimation = -1;

		private float _pulseSpeed;

		private float _currentPercent;

		private float _sliderAnimationSpeed;

		private HUDBattleArenaMarkerWidget[] _extraBehaviour;

		public HUDBattleArenaWidget()
			: this()
		{
		}//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)


		private void Awake()
		{
			InitHudWidget();
		}

		internal void InitHudWidget()
		{
			Console.Log("Late initialization of " + base.GetType() + " (" + this.get_gameObject().get_name() + "), design need to be fixed!");
			_extraBehaviour = this.GetComponentsInChildren<HUDBattleArenaMarkerWidget>(true);
			if (slider != null)
			{
				_currentPercent = slider.get_fillAmount();
			}
		}

		private void OnEnable()
		{
			if (slider != null)
			{
				slider.set_fillAmount(_currentPercent);
			}
		}

		internal void InitHUDWidget(Vector3 position, Vector3 size)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			InitHudWidget();
			for (int i = 0; i < _extraBehaviour.Length; i++)
			{
				_extraBehaviour[i].InitWidget(position, size);
			}
		}

		private void OnDestroy()
		{
			if (_pulseSequence != null)
			{
				TweenExtensions.Kill(_pulseSequence, false);
			}
			if (_colourChangeSequence != null)
			{
				TweenExtensions.Kill(_colourChangeSequence, false);
			}
			if (_bounceSequence != null)
			{
				TweenExtensions.Kill(_bounceSequence, false);
			}
			if (_disappearSequence != null)
			{
				TweenExtensions.Kill(_disappearSequence, false);
			}
			if (_endSequence != null)
			{
				TweenExtensions.Kill(_endSequence, false);
			}
		}

		private void LateUpdate()
		{
			if (slider != null)
			{
				UpdateSlider(slider, sliderHighlight, _sliderAnimationSpeed);
				for (int i = 0; i < _extraBehaviour.Length; i++)
				{
					_extraBehaviour[i].UpdateWidget();
				}
			}
			if (_currentPercent >= healthPercentStartLabelAnimation && gameEndSpeed.Length > 0)
			{
				int num = Mathf.FloorToInt((_currentPercent - healthPercentStartLabelAnimation) * 100f);
				if (num < 0)
				{
					num = 0;
				}
				float speed = (num <= gameEndSpeed.Length - 1) ? gameEndSpeed[num] : gameEndSpeed[gameEndSpeed.Length - 1];
				UpdateAnimatedLabel(animatedLabel, gameEndSprite, speed);
			}
			if (_onParticleAnimationEnd != null && _playingParticleAnimation != -1 && !particleAnimations[_playingParticleAnimation].get_activeSelf())
			{
				_onParticleAnimationEnd();
				_onParticleAnimationEnd = null;
			}
		}

		internal void Show(bool enabled)
		{
			this.get_gameObject().SetActive(enabled);
		}

		internal void SetHealthPercent(float percent, bool animate = true)
		{
			if (percent < 0f)
			{
				percent = 0f;
			}
			if (animate && sliderAnimationDuration > 0f)
			{
				_sliderAnimationSpeed = (percent - _currentPercent) / sliderAnimationDuration;
				_currentPercent = percent;
			}
			else
			{
				_currentPercent = percent;
				slider.set_fillAmount(percent);
			}
			if (percentLabel != null)
			{
				percentLabel.set_text(Mathf.FloorToInt(percent * 100f).ToString());
			}
		}

		internal void SetExtraInfoLabel(string str)
		{
			if (extraInfoLabel != null)
			{
				extraInfoLabel.set_text(str);
			}
		}

		internal void PlayAnimation()
		{
			if (animation != null)
			{
				animation.Play();
			}
		}

		internal void SetColors(Color primaryColor, Color secondaryColor)
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			for (int i = 0; i < primaryColorWidgets.Length; i++)
			{
				primaryColorWidgets[i].set_color(primaryColor);
			}
			for (int j = 0; j < secondaryColorWidgets.Length; j++)
			{
				secondaryColorWidgets[j].set_color(secondaryColor);
			}
		}

		internal unsafe void SetColorsWithAnimation(Color primaryColor, Color secondaryColor)
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			//IL_009b: Unknown result type (might be due to invalid IL or missing references)
			if (_colourChangeSequence != null)
			{
				TweenExtensions.Kill(_colourChangeSequence, false);
				this.get_transform().set_localScale(endScaleValue);
			}
			StopBounceAnimation();
			SetColors(primaryColor, secondaryColor);
			_colourChangeSequence = DOTween.Sequence();
			TweenSettingsExtensions.Append(_colourChangeSequence, TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(DOTween.To(new DOGetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), startScaleValue, 0.3f), 27));
			TweenSettingsExtensions.Append(_colourChangeSequence, TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(DOTween.To(new DOGetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), endScaleValue, 0.3f), 2));
			TweenExtensions.Play<Sequence>(_colourChangeSequence);
		}

		internal void SetPulseSpeed(int index)
		{
			if (basePulseAnimationSpeed.Length > 0)
			{
				_pulseSpeed = basePulseAnimationSpeed[index];
				if (_pulseSequence == null)
				{
					TaskRunner.get_Instance().Run((Func<IEnumerator>)PlayPulseEnumerator);
				}
			}
		}

		private unsafe IEnumerator PlayPulseEnumerator()
		{
			while (_pulseSpeed > 0f)
			{
				_pulseSequence = TweenSettingsExtensions.SetUpdate<Sequence>(TweenSettingsExtensions.SetRecyclable<Sequence>(DOTween.Sequence(), false), 1, true);
				TweenSettingsExtensions.Append(_pulseSequence, TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), _currentPercent, _pulseSpeed), 1));
				TweenSettingsExtensions.Join(_pulseSequence, TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), 0f, _pulseSpeed), 1));
				while (TweenExtensions.IsPlaying(_pulseSequence))
				{
					yield return null;
				}
				pulser.set_fillAmount(0f);
				pulser.set_alpha(1f);
			}
			_pulseSequence = null;
		}

		internal void PlayParticle(ParticleAnimation particleAnimation, Action onAnimationEnd = null)
		{
			if (particleAnimations != null && particleAnimations.Length > (int)particleAnimation && !particleAnimations[(int)particleAnimation].get_activeSelf() && _onParticleAnimationEnd == null)
			{
				_playingParticleAnimation = (int)particleAnimation;
				_onParticleAnimationEnd = onAnimationEnd;
				particleAnimations[(int)particleAnimation].SetActive(true);
			}
			else
			{
				onAnimationEnd?.Invoke();
			}
		}

		internal void Scale(Vector3 scale)
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			this.get_transform().set_localScale(scale);
		}

		internal unsafe void PlayBounceAnimation()
		{
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			if (_bounceSequence == null)
			{
				_bounceSequence = TweenSettingsExtensions.SetLoops<Sequence>(DOTween.Sequence(), -1);
				TweenSettingsExtensions.Append(_bounceSequence, TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(DOTween.To(new DOGetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), bounceStartScaleValue, 0.3f), 27));
				TweenSettingsExtensions.Append(_bounceSequence, TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(DOTween.To(new DOGetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Vector3>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), endScaleValue, 0.3f), 2));
			}
		}

		internal void StopBounceAnimation()
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			if (_bounceSequence != null)
			{
				TweenExtensions.Kill(_bounceSequence, false);
				_bounceSequence = null;
				this.get_transform().set_localScale(endScaleValue);
			}
		}

		private void UpdateSlider(UISprite sprite, UISprite spriteHighlight, float speed)
		{
			//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
			//IL_0100: Unknown result type (might be due to invalid IL or missing references)
			bool flag = speed > 0f;
			if (_currentPercent > 0f && ((flag && sprite.get_fillAmount() < _currentPercent) || (!flag && sprite.get_fillAmount() > _currentPercent)))
			{
				sprite.set_fillAmount((!flag) ? Mathf.Max(sprite.get_fillAmount() + speed * Time.get_deltaTime(), _currentPercent) : Mathf.Min(sprite.get_fillAmount() + speed * Time.get_deltaTime(), _currentPercent));
				if (spriteHighlight != null)
				{
					float fillAmount = sprite.get_fillAmount();
					Vector2 localSize = sprite.get_localSize();
					spriteHighlight.set_fillAmount(fillAmount + 1f / localSize.x);
				}
				return;
			}
			sprite.set_fillAmount(_currentPercent);
			if (spriteHighlight != null)
			{
				float fillAmount2;
				if (_currentPercent == 0f)
				{
					fillAmount2 = 0f;
				}
				else
				{
					float fillAmount3 = sprite.get_fillAmount();
					Vector2 localSize2 = sprite.get_localSize();
					fillAmount2 = fillAmount3 + 1f / localSize2.x;
				}
				spriteHighlight.set_fillAmount(fillAmount2);
			}
		}

		private unsafe void UpdateAnimatedLabel(Transform labelTransform, AnimatedAlpha animatedAlpha, float speed)
		{
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_008a: Unknown result type (might be due to invalid IL or missing references)
			if (labelTransform != null && (_endSequence == null || !TweenExtensions.IsPlaying(_endSequence)))
			{
				labelTransform.set_localScale(Vector3.get_one());
				animatedAlpha.alpha = 0f;
				_endSequence = DOTween.Sequence();
				_003CUpdateAnimatedLabel_003Ec__AnonStorey1 _003CUpdateAnimatedLabel_003Ec__AnonStorey;
				TweenSettingsExtensions.Append(_endSequence, TweenSettingsExtensions.SetEase<TweenerCore<Vector3, Vector3, VectorOptions>>(DOTween.To(new DOGetter<Vector3>((object)_003CUpdateAnimatedLabel_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Vector3>((object)_003CUpdateAnimatedLabel_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), startLabelAnimationScaleValue, speed), 34));
				TweenSettingsExtensions.Join(_endSequence, TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>((object)_003CUpdateAnimatedLabel_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)_003CUpdateAnimatedLabel_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), alphaEnd, speed), 34));
			}
		}
	}
}
