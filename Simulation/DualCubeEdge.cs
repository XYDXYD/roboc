using System;
using System.Reflection;
using UnityEngine;

namespace Simulation
{
	internal sealed class DualCubeEdge : IComparable<DualCubeEdge>
	{
		private static int counter;

		private Bounds _bounds;

		private int _ID;

		private MachineBounds _machineBounds;

		public int ID => _ID;

		public double error
		{
			get;
			private set;
		}

		public IClusteredColliderNode A
		{
			get;
			private set;
		}

		public IClusteredColliderNode B
		{
			get;
			private set;
		}

		public Bounds mergedBounds => _bounds;

		public MachineBounds machineBounds => _machineBounds;

		public float boundsVolume
		{
			get;
			private set;
		}

		public float distanceCentersSQR
		{
			get;
			private set;
		}

		public DualCubeEdge(IClusteredColliderNode A, IClusteredColliderNode B, MachineBounds machinebounds)
		{
			_machineBounds = machinebounds;
			ChangeChildren(A, B);
			_ID = counter++;
		}

		public static double PowerA(double a, double b)
		{
			DoubleBits doubleBits = new DoubleBits(a);
			int num = (int)(doubleBits.Bits >> 32);
			int num2 = (int)(b * (double)(num - 1072632447) + 1072632447.0);
			doubleBits.Reuse((long)num2 << 32);
			return doubleBits.Value;
		}

		[Obfuscation(Feature = "inline", Exclude = false)]
		public static double EvaluateError(MachineBounds machineBounds, float mergedVolume, float boundsVolume, double distanceErrorSQR)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Unknown result type (might be due to invalid IL or missing references)
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			Bounds bounds = machineBounds.bounds;
			Vector3 val = bounds.get_max() - bounds.get_min();
			float sqrMagnitude = val.get_sqrMagnitude();
			Vector3 size = bounds.get_size();
			float x = size.x;
			Vector3 size2 = bounds.get_size();
			float num = x * size2.y;
			Vector3 size3 = bounds.get_size();
			float num2 = num * size3.z;
			double num3 = (boundsVolume - mergedVolume) / num2;
			if (num3 > double.Epsilon)
			{
				num3 = PowerA(num3, 0.33333333333333331);
			}
			double num4 = Math.Sqrt(distanceErrorSQR / (double)sqrMagnitude);
			return num4 * 10.0 + num3 * 5.0;
		}

		private void ComputeError()
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Unknown result type (might be due to invalid IL or missing references)
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Unknown result type (might be due to invalid IL or missing references)
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_009a: Unknown result type (might be due to invalid IL or missing references)
			//IL_009f: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
			Bounds bounds = A.bounds;
			Vector3 center = bounds.get_center();
			Bounds bounds2 = A.bounds;
			_bounds = new Bounds(center, bounds2.get_size());
			_bounds.Encapsulate(B.bounds);
			if (A.isSingularity || B.isSingularity)
			{
				error = 3.4028234663852886E+38;
				return;
			}
			float mergedVolume = A.mergedVolume + B.mergedVolume;
			Vector3 size = _bounds.get_size();
			float x = size.x;
			Vector3 size2 = _bounds.get_size();
			float num = x * size2.y;
			Vector3 size3 = _bounds.get_size();
			boundsVolume = num * size3.z;
			Vector3 val = _bounds.get_max() - _bounds.get_min();
			distanceCentersSQR = val.get_sqrMagnitude();
			error = EvaluateError(_machineBounds, mergedVolume, boundsVolume, distanceCentersSQR);
		}

		public void ChangeChildren(IClusteredColliderNode _A, IClusteredColliderNode _B)
		{
			A = _A;
			B = _B;
			ComputeError();
		}

		public int CompareTo(DualCubeEdge other)
		{
			if (error < other.error)
			{
				return -1;
			}
			if (error > other.error)
			{
				return 1;
			}
			if (_ID == other._ID)
			{
				return 0;
			}
			if (_ID > other._ID)
			{
				return 1;
			}
			return -1;
		}
	}
}
