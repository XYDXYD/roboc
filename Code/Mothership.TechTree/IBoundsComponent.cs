using UnityEngine;

namespace Mothership.TechTree
{
	internal interface IBoundsComponent
	{
		Vector2 BoundsMin
		{
			get;
			set;
		}

		Vector2 BoundsMax
		{
			get;
			set;
		}
	}
}
