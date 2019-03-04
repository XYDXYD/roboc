using UnityEngine;

namespace Simulation.Hardware.Weapons.Nano
{
	internal class CrosshairNanoWeapon : CrosshairBase
	{
		private GameObject _lockIndicator;

		private GameObject _rangeIndicator;

		private bool _inRange;

		internal CrosshairNanoWeapon(NanoWeaponCrosshairInfo nanoWeaponCrosshairInfo, CrosshairWidget crosshairWidget, CrosshairController crosshairController, float offsetAtMinAccuracy)
			: base(nanoWeaponCrosshairInfo, null, crosshairController, crosshairWidget, offsetAtMinAccuracy)
		{
			_lockIndicator = nanoWeaponCrosshairInfo.Lock;
			_rangeIndicator = nanoWeaponCrosshairInfo.RangeIndicator;
		}

		internal override void UpdateState()
		{
			base.UpdateState();
			UpdateInRange();
			UpdateLockState();
		}

		private void UpdateInRange()
		{
			if (_crosshairController.inRange != _inRange)
			{
				_inRange = _crosshairController.inRange;
				_rangeIndicator.SetActive(_inRange);
			}
		}

		private void UpdateLockState()
		{
			_lockIndicator.SetActive(_crosshairController.lockStage > 0);
		}
	}
}
