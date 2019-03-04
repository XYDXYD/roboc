using Robocraft.GUI;
using Svelto.IoC;

namespace Mothership
{
	internal class FriendComponentFactory : IInitialize
	{
		public static string FriendList = "FriendList";

		[Inject]
		internal IComponentFactory GenericComponentFactory
		{
			get;
			private set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			GenericComponentFactory.RegisterCustomComponent(FriendList, typeof(FriendListComponentView), typeof(FriendListComponent));
		}
	}
}
