using Svelto.IoC;

namespace Simulation
{
	internal class ChatPresenterSinglePlayer : ChatPresenter
	{
		[Inject]
		internal ChatChannelCommands ChatChannelCommands
		{
			private get;
			set;
		}

		public ChatPresenterSinglePlayer()
		{
			_chatStyle = ChatStyle.Battle;
		}

		protected override void Initialize()
		{
		}

		protected override bool GetInitialExpanding()
		{
			return false;
		}

		protected override void TearDown()
		{
		}

		protected override string GetChatLocation()
		{
			return WorldSwitching.GetGameModeType().ToString();
		}
	}
}
