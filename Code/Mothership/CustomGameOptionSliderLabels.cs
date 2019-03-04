using UnityEngine;

namespace Mothership
{
	internal class CustomGameOptionSliderLabels : MonoBehaviour
	{
		[SerializeField]
		private UILabel SliderValueLabel;

		[SerializeField]
		private UILabel SliderDescriptionLabel;

		[SerializeField]
		private UILocalize SliderUILocalize;

		public UILabel ValueLabel => SliderValueLabel;

		public UILabel DescriptionLabel => SliderDescriptionLabel;

		public UILocalize DescriptionUILocalize => SliderUILocalize;

		public CustomGameOptionSliderLabels()
			: this()
		{
		}

		public void Start()
		{
		}
	}
}
