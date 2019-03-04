using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Simulation.Hardware.Weapons;
using Svelto.IoC;
using System;
using UnityEngine;

namespace Simulation
{
	internal class ZoomSurroundToggler : MonoBehaviour, IInitialize
	{
		public float fadeInTime = 0.2f;

		private UISprite[] _surroundSprites;

		private Tweener[] _tweens;

		[Inject]
		internal ZoomEngine zoom
		{
			private get;
			set;
		}

		public ZoomSurroundToggler()
			: this()
		{
		}

		void IInitialize.OnDependenciesInjected()
		{
			zoom.OnZoomModeChange += ZoomChanged;
			_surroundSprites = this.GetComponentsInChildren<UISprite>();
			_tweens = (Tweener[])new Tweener[_surroundSprites.Length];
			this.get_gameObject().SetActive(false);
		}

		private void OnDestroy()
		{
			zoom.OnZoomModeChange -= ZoomChanged;
			for (int i = 0; i < _tweens.Length; i++)
			{
				Tweener val = _tweens[i];
				if (val != null)
				{
					TweenExtensions.Kill(val, false);
				}
			}
		}

		private unsafe void ZoomChanged(ZoomType zoomType, float zoomAmount)
		{
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_004f: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			this.get_gameObject().SetActive(zoomType == ZoomType.Zoomed);
			if (zoomType == ZoomType.Zoomed)
			{
				for (int i = 0; i < _surroundSprites.Length; i++)
				{
					UISprite surroundSprite = _surroundSprites[i];
					Color color = surroundSprite.get_color();
					color.a = 0.5f;
					surroundSprite.set_color(color);
					_003CZoomChanged_003Ec__AnonStorey0 _003CZoomChanged_003Ec__AnonStorey;
					_tweens[i] = TweenSettingsExtensions.SetEase<TweenerCore<Color, Color, ColorOptions>>(DOTween.To(new DOGetter<Color>((object)_003CZoomChanged_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<Color>((object)_003CZoomChanged_003Ec__AnonStorey, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), surroundSprite.get_color(), fadeInTime), 1);
					TweenExtensions.Play<Tweener>(_tweens[i]);
				}
			}
		}
	}
}
