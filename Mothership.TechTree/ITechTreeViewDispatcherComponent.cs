using Svelto.ECS;

namespace Mothership.TechTree
{
	internal interface ITechTreeViewDispatcherComponent
	{
		DispatchOnChange<bool> IsActive
		{
			get;
		}

		DispatchOnSet<bool> InputLocked
		{
			get;
		}
	}
}
