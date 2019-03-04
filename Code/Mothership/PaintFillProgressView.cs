using Svelto.IoC;
using UnityEngine;

namespace Mothership
{
	internal class PaintFillProgressView : MonoBehaviour
	{
		public UISlider fillBarSlider;

		private int _totalSteps;

		private int _stepsTaken;

		[Inject]
		internal PaintFillController fillController
		{
			private get;
			set;
		}

		public PaintFillProgressView()
			: this()
		{
		}

		private void Start()
		{
			fillController.paintFillProgressView = this;
			SetActive(active: false);
		}

		public void StepForward()
		{
			_stepsTaken++;
			UpdateSlider();
		}

		public void StepBackward()
		{
			_stepsTaken--;
			UpdateSlider();
		}

		private void UpdateSlider()
		{
			float value = (float)_stepsTaken / (float)_totalSteps;
			fillBarSlider.set_value(value);
		}

		public void SetActive(bool active)
		{
			this.get_gameObject().SetActive(active);
		}

		public void ShowProgressBar(int numSteps)
		{
			SetActive(active: true);
			_totalSteps = numSteps;
			_stepsTaken = 0;
			UpdateSlider();
		}

		public void ClearProgressBar()
		{
			SetActive(active: false);
		}
	}
}
