using Svelto.IoC;
using UnityEngine;

namespace Simulation
{
	internal sealed class AlignmentRectifierTimerView : MonoBehaviour
	{
		public UISprite filledSprite;

		public UILabel timerLabel;

		private float _timerLength = 5f;

		private float _timer;

		[Inject]
		internal AlignmentRectifierEngine alignmentRectifierManager
		{
			private get;
			set;
		}

		public AlignmentRectifierTimerView()
			: this()
		{
		}

		private void Start()
		{
			alignmentRectifierManager.RegisterView(this);
		}

		private void OnDestroy()
		{
			alignmentRectifierManager.UnregisterView(this);
		}

		public void Show()
		{
			this.get_gameObject().SetActive(true);
		}

		public void Hide()
		{
			this.get_gameObject().SetActive(false);
		}

		public void SetTime(float t)
		{
			_timer = t;
		}

		public void SetDuration(float timerLength)
		{
			_timerLength = timerLength;
		}

		public void UpdateGui()
		{
			timerLabel.set_text($"{_timer:0.00}");
			filledSprite.set_fillAmount(_timer / _timerLength);
		}
	}
}
