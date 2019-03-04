using DG.Tweening;
using DG.Tweening.Core;
using Svelto.IoC;
using System;
using System.Text;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDSpeedView : MonoBehaviour, IInitialize
	{
		public UILabel speedLabel;

		public UISprite speedSlider;

		public UILabel boostLabel;

		public UISprite boostSlider;

		private StringBuilder _stringBuilder = new StringBuilder();

		private Sequence _baseSpeedSequence;

		private Sequence _speedBoostSequence;

		[Inject]
		internal HUDSpeedPreseneter speedPresenter
		{
			private get;
			set;
		}

		public HUDSpeedView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			speedPresenter.SetView(this);
		}

		internal void SetSpeed(string value, float percent, bool useDecimalSystem)
		{
			string key = (!useDecimalSystem) ? "strMilesPerHour" : "strKilometersPerHour";
			_stringBuilder.Length = 0;
			_stringBuilder.Append(value);
			_stringBuilder.Append(" ");
			_stringBuilder.Append(StringTableBase<StringTable>.Instance.GetString(key));
			speedLabel.set_text(_stringBuilder.ToString());
			AnimateBaseSpeedBar(percent);
		}

		internal void SetBoost(string value, float percent)
		{
			_stringBuilder.Length = 0;
			_stringBuilder.Append(value);
			_stringBuilder.Append(" %");
			boostLabel.set_text(_stringBuilder.ToString());
			AnimateSpeedBoostBar(percent);
		}

		private unsafe void AnimateBaseSpeedBar(float val)
		{
			TweenExtensions.Kill(_baseSpeedSequence, false);
			_baseSpeedSequence = TweenSettingsExtensions.SetRecyclable<Sequence>(DOTween.Sequence(), false);
			TweenSettingsExtensions.Append(_baseSpeedSequence, DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), val, 0.5f));
		}

		private unsafe void AnimateSpeedBoostBar(float val)
		{
			TweenExtensions.Kill(_speedBoostSequence, false);
			_speedBoostSequence = TweenSettingsExtensions.SetRecyclable<Sequence>(DOTween.Sequence(), false);
			TweenSettingsExtensions.Append(_speedBoostSequence, DOTween.To(new DOGetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), new DOSetter<float>((object)this, (IntPtr)(void*)/*OpCode not supported: LdFtn*/), val, 0.5f));
		}
	}
}
