using Svelto.Ticker.Legacy;
using System.Collections.Generic;
using UnityEngine;
using Utility;

namespace Simulation
{
	internal class RigidbodyCOMTracker : ITickable, ILateTickable, ITickableBase
	{
		private Dictionary<Rigidbody, Vector3> _perRigidbodyCOM = new Dictionary<Rigidbody, Vector3>();

		internal static int frame;

		public void SetCOM(Rigidbody rb, Vector3 com)
		{
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			_perRigidbodyCOM[rb] = com;
		}

		public Vector3 GetCOM(Rigidbody rb)
		{
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			if (_perRigidbodyCOM.ContainsKey(rb))
			{
				return _perRigidbodyCOM[rb];
			}
			return Vector3.get_zero();
		}

		public void Tick(float deltaTime)
		{
			frame++;
			foreach (KeyValuePair<Rigidbody, Vector3> item in _perRigidbodyCOM)
			{
				VerifyCOM(item.Key, "Tick");
			}
		}

		public void LateTick(float deltaTime)
		{
			foreach (KeyValuePair<Rigidbody, Vector3> item in _perRigidbodyCOM)
			{
				VerifyCOM(item.Key, "LateTick");
			}
		}

		public void VerifyCOM(Rigidbody rb, string errorMsg)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0036: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			if (rb != null)
			{
				Vector3 val = _perRigidbodyCOM[rb];
				if (rb.get_centerOfMass() != val)
				{
					Vector3 val2 = rb.get_transform().get_rotation() * val;
					Vector3 val3 = rb.get_position() + val2;
					Console.LogWarning($"***COM offset {errorMsg}: {Vector3.Distance(rb.get_centerOfMass(), val3)}");
				}
			}
		}
	}
}
