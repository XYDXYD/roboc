using UnityEngine;

namespace Mothership.TechTree
{
	internal interface IGameObjectComponent
	{
		GameObject gameObject
		{
			get;
		}

		Transform transform
		{
			get;
		}
	}
}
