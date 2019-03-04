using System;
using UnityEngine;

namespace Simulation
{
	internal class AlignmentRectifierCollisionDetection : MonoBehaviour
	{
		public event Action OnCollisionDetected = delegate
		{
		};

		public AlignmentRectifierCollisionDetection()
			: this()
		{
		}

		private void OnCollisionEnter(Collision collision)
		{
			this.OnCollisionDetected();
		}

		private void OnTriggerEnter(Collider other)
		{
			this.OnCollisionDetected();
		}
	}
}
