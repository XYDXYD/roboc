using Svelto.Observer.IntraNamespace;

namespace Mothership.GUI
{
	internal class DragAndDropGUIEventObserver : Observer<DragAndDropGUIMessage>
	{
		public DragAndDropGUIEventObserver(DragAndDropGUIEventObservable observable)
			: base(observable)
		{
		}
	}
}
