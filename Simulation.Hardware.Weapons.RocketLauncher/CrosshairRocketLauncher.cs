using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware.Weapons.RocketLauncher
{
	internal class CrosshairRocketLauncher : CrosshairBase
	{
		private int _lockStage;

		private bool _enableLockTriangle;

		private List<GameObject> _locks;

		private GameObject _targetIndicator;

		private GameObject _targetAcquiringIndicator;

		private UISprite _targetTriangle;

		private UISprite _aquiringTargetTriangle;

		internal CrosshairRocketLauncher(RocketLauncherCrosshairInfo rocketLauncherCrosshairInfo, CrosshairWidget crosshairWidget, CrosshairController crosshairController, float offsetAtMinAccuracy, RocketLauncherCrosshairInfo crossHairInfo)
			: base(rocketLauncherCrosshairInfo, null, crosshairController, crosshairWidget, offsetAtMinAccuracy)
		{
			_locks = crossHairInfo.Locks;
			_targetIndicator = crossHairInfo.TargetIndicator;
			_targetTriangle = _targetIndicator.GetComponent<UISprite>();
			_targetAcquiringIndicator = crossHairInfo.AcquiringTargetIndicator;
			_aquiringTargetTriangle = _targetAcquiringIndicator.GetComponent<UISprite>();
			SetLockStage(_lockStage);
		}

		internal override void UpdateState()
		{
			base.UpdateState();
			UpdateLockStage();
			UpdateLockTraingle();
		}

		private void UpdateLockStage()
		{
			if (_crosshairController.lockStage != _lockStage)
			{
				SetLockStage(_crosshairController.lockStage);
			}
		}

		private void UpdateLockTraingle()
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0064: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Unknown result type (might be due to invalid IL or missing references)
			if (_enableLockTriangle)
			{
				Vector3 localPosition = Camera.get_main().WorldToScreenPoint(_crosshairController.lockTargetPosition);
				localPosition._002Ector(localPosition.x - (float)(Screen.get_width() / 2), localPosition.y - (float)(Screen.get_height() / 2), 0f);
				if (_lockStage == 3)
				{
					_targetTriangle.get_transform().set_localPosition(localPosition);
				}
				else
				{
					_aquiringTargetTriangle.get_transform().set_localPosition(localPosition);
				}
			}
		}

		private void SetLockStage(int stage)
		{
			_lockStage = stage;
			if (_locks.Count == 1)
			{
				_locks[0].SetActive(stage != 0);
			}
			else
			{
				for (int i = 0; i < _locks.Count; i++)
				{
					_locks[i].SetActive(i == stage);
				}
			}
			_enableLockTriangle = (stage != 0);
			_targetIndicator.SetActive(stage != 0 && stage == 3);
			_targetAcquiringIndicator.SetActive(stage != 0 && stage != 3);
		}
	}
}
