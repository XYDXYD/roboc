using Svelto.Context;
using Svelto.IoC;
using UnityEngine;

namespace Mothership.GUI
{
	internal sealed class DragAndDropGUIFactory
	{
		[Inject]
		internal IContextNotifer contextNotifier
		{
			private get;
			set;
		}

		public void Build(GameObject rootGUINode, IContainer container)
		{
			DragAndDropView component = rootGUINode.GetComponent<DragAndDropView>();
			DragAndDropController dragAndDropController = container.Inject<DragAndDropController>(new DragAndDropController());
			dragAndDropController.Initialise(contextNotifier);
			dragAndDropController.SetView(component);
			component.InjectController(dragAndDropController);
		}
	}
}
