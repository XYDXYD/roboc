using UnityEngine;

namespace Simulation.Hardware.Weapons.Chaingun
{
	internal class CrosshairChaingun : CrosshairBase
	{
		private ChaingunCrosshairInfo _parameters;

		internal CrosshairChaingun(ChaingunCrosshairInfo chaingunCrosshairInfo, CrosshairWidget crosshairWidget, CrosshairController crosshairController, float offsetAtMinAccuracy)
			: base(chaingunCrosshairInfo, null, crosshairController, crosshairWidget, offsetAtMinAccuracy)
		{
			_parameters = chaingunCrosshairInfo;
		}

		internal override void UpdateState()
		{
			base.UpdateState();
			UpdateSpinning();
		}

		private void UpdateSpinning()
		{
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00db: Unknown result type (might be due to invalid IL or missing references)
			float weaponSpinPower = _crosshairController.weaponSpinPower;
			_parameters.rotatingPart.Rotate(new Vector3(0f, 0f, weaponSpinPower * _parameters.rotationSpeed * Time.get_deltaTime()));
			Color color = _parameters.blurredSprite.get_color();
			if (_parameters.blurMaxThreshold != 0f && weaponSpinPower < _parameters.blurMaxThreshold)
			{
				color.a = weaponSpinPower / _parameters.blurMaxThreshold;
			}
			else
			{
				color.a = 1f;
			}
			_parameters.blurredSprite.set_color(color);
			Color color2 = _parameters.baseSprite.get_color();
			color2.a = 1f - color.a;
			_parameters.baseSprite.set_color(color2);
		}
	}
}
