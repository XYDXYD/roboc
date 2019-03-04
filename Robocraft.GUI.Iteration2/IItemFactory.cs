using UnityEngine;

namespace Robocraft.GUI.Iteration2
{
	internal interface IItemFactory
	{
		IItemPresenter Build(GameObject template, Transform root);
	}
}
