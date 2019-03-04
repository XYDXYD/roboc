using System;
using UnityEngine;

namespace Robocraft.GUI
{
	internal interface IComponentFactory
	{
		void RegisterCustomComponent(string typeName, Type view, Type controller);

		BuiltComponentElements BuildComponent(GUIComponentType componentToBuild, IDataSource dataSource, GameObject template, bool makeInstance = true);

		BuiltComponentElements BuildCustomComponent(string componentToBuild, IDataSource dataSource, GameObject template, bool makeInstance = true);
	}
}
