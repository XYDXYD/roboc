using Svelto.Command;
using Svelto.Context;
using Svelto.IoC;

namespace Simulation
{
	internal class ChatCommandsSimulation : IInitialize, IWaitForFrameworkDestruction
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
			chatCommands.RegisterCommand("groupinvite", ChatCommands.CommandData.AccessLevel.All, StringTableBase<StringTable>.Instance.GetString("strInviteCommandUsage"), InviteToParty);
		}

		public void OnFrameworkDestroyed()
		{
			chatCommands.DeregisterCommand("groupinvite");
		}

		private bool InviteToParty(string player)
		{
			commandFactory.Build<ReportSocialEventCommand>().Inject(StringTableBase<StringTable>.Instance.GetString("strNoPartyInviteInQueueOrBattle")).Execute();
			return true;
		}
	}
}
