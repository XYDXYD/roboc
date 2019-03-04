using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Fabric;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal class HUDWaitTimeView : MonoBehaviour, IInitialize
	{
		public UILabel timeLabel;

		public UISprite sprite;

		public string countDouwnAudio = "KUB_DEMO_fabric_GUI_MarsCountdown_Loud";

		private Sequence _tween;

		[Inject]
		internal HUDWaitTimePresenter presenter
		{
			private get;
			set;
		}

		public HUDWaitTimeView()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			presenter.view = this;
		}

		internal void UpdateLabel(string time)
		{
			timeLabel.set_text(time);
		}

		internal void UpdateSprite(float percent)
		{
			sprite.set_fillAmount(percent);
		}

		internal void PlaySound()
		{
			if (this.get_gameObject().get_activeInHierarchy())
			{
				EventManager.get_Instance().PostEvent(countDouwnAudio, 0);
			}
		}

		internal unsafe void AnimateSprite(float value)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Expected O, but got Unknown
			_tween = TweenSettingsExtensions.OnComplete<Sequence>(DOTween.Sequence(), new TweenCallback((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/));
			TweenSettingsExtensions.Append(_tween, TweenSettingsExtensions.SetEase<TweenerCore<float, float, FloatOptions>>(DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), value, 0.5f), 3));
			TweenExtensions.Play<Sequence>(_tween);
		}

		private void OnDisable()
		{
			if (_tween != null)
			{
				TweenExtensions.Kill(_tween, false);
			}
		}
	}
}
