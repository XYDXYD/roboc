using System;
using System.Collections.Generic;
using UnityEngine;

namespace Simulation
{
	[Serializable]
	internal sealed class TrackScrollItem
	{
		public GameObject scrollingObject;

		public float scrollScale = 1f;

		public int numberFramesToAvg = 8;

		[NonSerialized]
		public float radius;

		[NonSerialized]
		public float scrollAmount;

		[NonSerialized]
		public Material scrollingMaterial;

		[NonSerialized]
		public Queue<float> pastRPM = new Queue<float>();

		private int numWheels;

		public void InitScrolling()
		{
			scrollingMaterial = scrollingObject.GetComponent<SkinnedMeshRenderer>().get_material();
		}

		public void AddWheelRadius(float wheelRadius)
		{
			radius *= numWheels;
			radius += wheelRadius;
			radius /= ++numWheels;
		}
	}
}
