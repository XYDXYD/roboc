using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDDamageBoostView : MonoBehaviour, IInitialize
	{
		public float tweenDuration = 0.5f;

		public UILabel DamageBoostHeaderLabel;

		public UILabel DamageBoostValueLabel;

		public UISprite DamageBoostSlider;

		[Inject]
		internal HUDDamageBoostPresenter presenter
		{
			private get;
			set;
		}

		internal float SliderValue
		{
			get
			{
				return DamageBoostSlider.get_fillAmount();
			}
			set
			{
				DamageBoostSlider.set_fillAmount(value);
			}
		}

		public HUDDamageBoostView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}

		public void SetHeaderLabel(string label)
		{
			DamageBoostHeaderLabel.set_text(label);
		}

		public void SetValueLabel(string label)
		{
			DamageBoostValueLabel.set_text(label);
		}
	}
}
