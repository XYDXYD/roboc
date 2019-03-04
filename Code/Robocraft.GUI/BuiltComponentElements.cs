using UnityEngine;

namespace Robocraft.GUI
{
	internal struct BuiltComponentElements
	{
		public GameObject builtGameObject;

		public IGenericComponent componentController;

		public IGenericComponentView componentView;

		public BuiltComponentElements(GameObject builtGameObject_, IGenericComponent componentController_, IGenericComponentView componentView_)
		{
			builtGameObject = builtGameObject_;
			componentController = componentController_;
			componentView = componentView_;
		}
	}
}
