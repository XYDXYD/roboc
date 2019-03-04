using UnityEngine;

namespace Simulation.Hardware.Weapons
{
	internal class CrosshairMortar : CrosshairBase
	{
		private GameObject _groundWarning;

		private UILabel _angleLabel;

		private float _timeBeforeNextUpdate;

		private float _numberUpdateRate;

		public CrosshairMortar(MortarCrosshairInfo info, CrosshairController crosshairController, CrosshairWidget crosshairWidget)
			: base(info, null, crosshairController, crosshairWidget, 0f)
		{
			_groundWarning = info.GroundWarning;
			_angleLabel = info.horizonAngleLabel;
			_numberUpdateRate = info.numberUpdateRate;
		}

		internal override void UpdateState()
		{
			base.UpdateState();
			_timeBeforeNextUpdate -= Time.get_deltaTime();
			if (_timeBeforeNextUpdate < 0f)
			{
				_timeBeforeNextUpdate += _numberUpdateRate;
				_angleLabel.set_text(Mathf.RoundToInt(_crosshairController.angleToHorizonDeg) + "°");
			}
		}

		internal override void ActivateGroundWarning(bool active)
		{
			_groundWarning.SetActive(active);
		}
	}
}
