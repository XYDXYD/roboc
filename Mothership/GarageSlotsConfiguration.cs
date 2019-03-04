using System;
using UnityEngine;

namespace Mothership
{
	[Serializable]
	internal struct GarageSlotsConfiguration
	{
		[SerializeField]
		private float _maxAspectRatio;

		[SerializeField]
		private int _numberOfSlots;

		public float maxAspectRatio => _maxAspectRatio;

		public int numberOfSlots => _numberOfSlots;
	}
}
