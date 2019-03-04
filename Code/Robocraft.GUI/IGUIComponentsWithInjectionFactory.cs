using System;
using UnityEngine;

namespace Robocraft.GUI
{
	internal interface IGUIComponentsWithInjectionFactory
	{
		IGenericComponent BuildComponent(Type t);

		IGenericComponentView BuildView(Type t, GameObject go);

		GameObject BuildListEntryView(string poolName, GameObject template);

		GameObject BuildListHeaderEntryView(string poolName, GameObject gameObjectToCopy);
	}
}
