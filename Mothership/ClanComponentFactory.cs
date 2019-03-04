using Robocraft.GUI;
using Svelto.IoC;

namespace Mothership
{
	internal class ClanComponentFactory : IInitialize
	{
		[Inject]
		internal IComponentFactory GenericComponentFactory
		{
			get;
			private set;
		}

		void IInitialize.OnDependenciesInjected()
		{
			GenericComponentFactory.RegisterCustomComponent("ClanSearchList", typeof(ClanSearchListComponentView), typeof(ClanSearchListComponent));
			GenericComponentFactory.RegisterCustomComponent("ClanPlayerList", typeof(ClanPlayersListComponentView), typeof(ClanPlayersListComponent));
			GenericComponentFactory.RegisterCustomComponent("ClanInvitationsList", typeof(GenericExpandeableListComponentView), typeof(InviteesListComponent));
		}
	}
}
