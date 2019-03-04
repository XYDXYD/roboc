using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI.Social
{
	internal interface IGUIElementFactory
	{
		void Build(GameObject guiElementRoot, IContainer container);
	}
}
