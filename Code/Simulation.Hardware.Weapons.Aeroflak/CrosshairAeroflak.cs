using UnityEngine;

namespace Simulation.Hardware.Weapons.Aeroflak
{
	internal class CrosshairAeroflak : CrosshairBase
	{
		private GameObject _groundWarning;

		private UISprite[] _stacks;

		private float _stacksDuration;

		private int _currentPercent;

		public CrosshairAeroflak(AeroflakCrosshairInfo aeroflakCrosshair, CrosshairController crosshairController, CrosshairWidget crosshairWidget)
			: base(aeroflakCrosshair, aeroflakCrosshair.MoveablePart, crosshairController, crosshairWidget, aeroflakCrosshair.OffsetAtMinAccuracy)
		{
			_stacks = aeroflakCrosshair.Stacks.GetComponentsInChildren<UISprite>();
			_stacksDuration = aeroflakCrosshair.stacksDuration;
			_groundWarning = aeroflakCrosshair.GroundWarning;
		}

		internal override void UpdateState()
		{
			base.UpdateState();
			float num = Time.get_time() - (_hitTime + (_stacksDuration - _hitDuration));
			if (_currentPercent > 0 && num > 0f)
			{
				float num2 = Mathf.Clamp01(num / _hitDuration);
				float num3 = Mathf.Lerp(0f, (float)_currentPercent, 1f - num2);
				UpdateStackCount((int)num3);
			}
		}

		protected override void UpdateStackCount(int stackPercent)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0032: Unknown result type (might be due to invalid IL or missing references)
			_currentPercent = stackPercent;
			float x = (float)stackPercent / 100f;
			Vector3 one = Vector3.get_one();
			one.x = x;
			for (int i = 0; i < _stacks.Length; i++)
			{
				_stacks[i].get_transform().set_localScale(one);
			}
		}

		internal override void ActivateGroundWarning(bool active)
		{
			_groundWarning.SetActive(active);
		}
	}
}
