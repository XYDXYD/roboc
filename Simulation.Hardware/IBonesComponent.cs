using System.Collections.Generic;
using UnityEngine;

namespace Simulation.Hardware
{
	internal interface IBonesComponent
	{
		List<Transform> bones
		{
			get;
		}

		int textureId
		{
			get;
			set;
		}
	}
}
