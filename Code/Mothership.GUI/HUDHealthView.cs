using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal class HUDHealthView : MonoBehaviour, IInitialize
	{
		public float MaxHealthBoost = 1f;

		public float tweenDuration = 0.5f;

		public UILabel HealthLabel;

		[SerializeField]
		private UISprite HealthSlider;

		public UILabel HealthBoostLabel;

		[SerializeField]
		private UISprite HealthBoostSlider;

		[SerializeField]
		private UISprite MegabotHealthSlider;

		[Inject]
		internal HUDHealthPresenter presenter
		{
			private get;
			set;
		}

		internal float HealthSliderValue
		{
			get
			{
				return HealthSlider.get_fillAmount();
			}
			set
			{
				HealthSlider.set_fillAmount(value);
			}
		}

		internal float HealthBoostSliderValue
		{
			get
			{
				return HealthBoostSlider.get_fillAmount();
			}
			set
			{
				HealthBoostSlider.set_fillAmount(value);
			}
		}

		internal float MegabotHealthSliderValue
		{
			get
			{
				return MegabotHealthSlider.get_fillAmount();
			}
			set
			{
				MegabotHealthSlider.set_fillAmount(value);
			}
		}

		public HUDHealthView()
			: this()
		{
		}

		public void OnDependenciesInjected()
		{
			presenter.SetView(this);
		}
	}
}
