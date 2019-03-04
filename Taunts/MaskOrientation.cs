using UnityEngine;

namespace Taunts
{
	public struct MaskOrientation
	{
		private Quaternion _quaternion;

		public MaskOrientation(MaskOrientation other)
		{
			//IL_0003: Unknown result type (might be due to invalid IL or missing references)
			//IL_0008: Unknown result type (might be due to invalid IL or missing references)
			_quaternion = other._quaternion;
		}

		public MaskOrientation(byte lookupCode)
		{
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			_quaternion = CubeData.sQuatList[lookupCode];
		}

		public void AdjustByInverse(MaskOrientation other)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			_quaternion *= Quaternion.Inverse(other._quaternion);
		}

		public void Default()
		{
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			_quaternion = Quaternion.AngleAxis(0f, Vector3.get_forward());
		}

		public Quaternion ToQuaternion()
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return _quaternion;
		}
	}
}
