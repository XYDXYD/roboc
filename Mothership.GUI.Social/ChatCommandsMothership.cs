using Mothership.GUI.Party;
using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;

namespace Mothership.GUI.Social
{
	internal class ChatCommandsMothership : IInitialize, IWaitForFrameworkDestruction
	{
		[Inject]
		internal ChatCommands chatCommands
		{
			private get;
			set;
		}

		[Inject]
		internal ICommandFactory commandFactory
		{
			private get;
			set;
		}

		public void OnDependenciesInjected()
		{
			chatCommands.RegisterCommand("groupinvite", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strInviteCommandUsage"), InviteToParty, 1);
		}

		public void OnFrameworkDestroyed()
		{
			chatCommands.DeregisterCommand("groupinvite");
		}

		private bool InviteToParty(string player)
		{
			if (player != null && player != string.Empty)
			{
				commandFactory.Build<InviteToPartyCommand>().Inject(player).Execute();
				return true;
			}
			return false;
		}
	}
}
